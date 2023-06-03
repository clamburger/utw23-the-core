<script lang="ts">
  import { CardType, DisplayState } from '../services/game';
  import { setupSignalRConnection } from '../services/hub';
  import Card from './Card.svelte';
  import StateHeader from './StateHeader.svelte';
  import { ProgressRadial } from '@skeletonlabs/skeleton';

    export const connection = setupSignalRConnection('/api/hub', {});

    let state: DisplayState = DisplayState.Connecting;
    let systemError: string;
    let systemSuccess: string;

    connection.on('ScreenRegistered', (firstTimeSetup: boolean) => {
        messages = [...messages, "Registered as the primary screen"]

        if (firstTimeSetup) {
            state = DisplayState.FirstTimeSetup;
            messages = [...messages, "No admin cards registered - entering first time setup"];
        } else {
            state = DisplayState.Ready;
        }
    });

    connection.on('ScreenDeregistered', () => {
        state = DisplayState.Disconnected;
        messages = [...messages, "Disconnected: another screen has become the primary"]
    });

    connection.on('SystemMessage', (message: string) => {
      messages.push(message);
      messages = messages;
    });

    connection.on('CardInserted', (_card, uid) => {
        //card = uid;
        messages = [...messages, `Card inserted: ${uid}`];
        systemError = undefined;
        systemSuccess = undefined;

        // Create an interval that increases the progress bar from 0 to 100
        // over the course of 3 seconds
        progress = 0;

        var callback = (_card) => {};

        if (state === DisplayState.FirstTimeSetup) {
            callback = (_card) => connection.invoke('RegisterFirstAdminCard', uid);
        } else if (state === DisplayState.Ready) {
            callback = (_card) => {
                if (_card.type === CardType.Admin) {
                    state = DisplayState.AdminDashboard;
                    card = null;
                    uid = null;
                } else if (_card.type === CardType.Team) {
                    state = DisplayState.TeamDashboard;
                } else {
                    systemError = "Unknown card";
                }
            }
        } else if (state === DisplayState.RegisteringCards) {
            callback = (_card) => {
                if (newCardType === CardType.Admin) {
                    connection.invoke('RegisterCard', CardType.Admin);
                } else if (newCardType === CardType.Credits) {
                    connection.invoke('RegisterCard', CardType.Credits, newCardValue);
                // } else if (newCardType === CardType.Team) {
                    // connection.invoke('RegisterCard', CardType.Team, newTeamName, newTeamColour);
                } else {
                    systemError = "Unknown card type";
                }
            };connection.invoke('RegisterCard', uid, )
        }

        var intervalFunc = intervalBuilder(_card, uid, callback);
        interval = setInterval(intervalFunc, 1000 / 10);
        intervalFunc();
    });

    connection.on('SystemError', (message: string) => {
        systemError = message;
    });

    connection.on('SystemSuccess', (message: string) => {
        systemSuccess = message;
    });

    connection.on('CardRegistered', () => {
        if (state === DisplayState.FirstTimeSetup) {
            state = DisplayState.Ready;
        }
    });

    function intervalBuilder(_card: any, _uid: string, callback: (_card: any) => void)
    {
        return () => {
            progress = progress + (100 / 10);
        
            if (progress >= 100) {
                progress = 100;
                card = _card;
                uid = _uid;
                clearInterval(interval);
                messages.push(JSON.stringify(_card));
                messages = messages;
                callback(_card);
            }
        }
    }

    connection.on('CardRemoved', () => {
        card = null;
        uid = null;
        messages = [...messages, 'Card removed'];
        systemError = undefined;
        systemSuccess = undefined;

        progress = 0;
        clearInterval(interval);
    });

    function backToReady()
    {
        state = DisplayState.Ready;
    }

    function changeState(_state: DisplayState)
    {
        state = _state;
    }

    let messages = [''];
    let card = null;
    let uid: string|null = null;
    let progress = 0;
    let invalid: boolean = false;
    
    let interval;
    let showCardReader = false;
    let newCardType;
    let newCardValue;
    let newTeamName;
    let newTeamColour;

    $: showCardReader = [DisplayState.FirstTimeSetup, DisplayState.Ready, DisplayState.RegisteringCards, DisplayState.ResettingCards, DisplayState.TeamDashboard].includes(state);
    $: invalid = uid && !card && ![DisplayState.FirstTimeSetup, DisplayState.RegisteringCards].includes(state);
</script>

<style lang="postcss">
    :global(.glow) {
        @apply bg-success-700/25;
        box-shadow: 0 0 50px theme('colors.success.700');
    }
</style>

<div class='space-y-6'>
    <StateHeader {state} />

    {#if showCardReader}
        <Card {card} {uid} {invalid} />
        <ProgressRadial class="mx-auto {progress == 100 ? 'glow' : ''}" width="w-12" stroke={100} meter={progress < 100 ? "stroke-primary-500" : "stroke-success-500"} value={Math.min(100, progress)} max={100} />
    {/if}

    {#if state === DisplayState.FirstTimeSetup}
        {#if progress === 0}
            <div>Scan a card to register it as an admin card.</div>
        {:else if progress < 100}
            <div>Wait...</div>
        {:else}
            <div>Admin card registered.</div>
        {/if}
    {:else if state === DisplayState.AdminDashboard}
        <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.ResettingCards)}>Reset Redeemed Cards</button>    
        <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.RegisteringCards)}>Register New Cards</button>
        <button class="btn variant-filled-error">Delete All Non-Admin Cards</button>
        <button class="btn variant-filled-secondary" on:click={backToReady}>Exit Admin Dashboard</button>
    {:else if state === DisplayState.ResettingCards}
        <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
    {:else if state === DisplayState.RegisteringCards}
        <div>
            <select class="select" bind:value={newCardType}>
                <option value="">-- Select a card type --</option>
                <option value={CardType.Admin}>Admin</option>
                <option value={CardType.Team}>Team</option>
                <option value={CardType.Credits}>Credits</option>
            </select>
            {#if newCardType === CardType.Credits}
                <input class="input" type="number" bind:value={newCardValue} placeholder="Credits">
            {:else if newCardType === CardType.Team}
                <input class="input" type="text" bind:value={newTeamName} placeholder="Team Name">
                <input class="input" type="text" bind:value={newTeamColour} placeholder="Team Colour">
            {/if}
        </div>
        <div>
            <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
        </div>
    {:else if state === DisplayState.Disconnected}
        <button class="btn variant-filled-error" on:click={() => window.location.reload()}>Reload Page</button>
    {/if}

    {#if systemError}
        <aside class="alert variant-filled-error">
            <div><strong>Error:</strong> {systemError}</div>
        </aside>
    {/if}

    {#if systemSuccess}
        <aside class="alert variant-filled-success">
            <div>{systemSuccess}</div>
        </aside>
    {/if}

    <div class='text-left shadow bg-neutral-900/90 rounded-container-token p-4'>
        <div class='text-sm border-b border-b-neutral-500 mb-2 pb-1' style='font-variant-caps: small-caps;'>
            System Log
        </div>
        <div class='font-mono'>
            {#each messages as message}
                <div>{message}</div>
            {/each}
        </div>
    </div>
</div>
