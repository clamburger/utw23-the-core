<script lang="ts">
  import { DisplayState } from '../services/game';
  import { setupSignalRConnection } from '../services/hub';
  import Card from './Card.svelte';
  import StateHeader from './StateHeader.svelte';
  import { ProgressRadial } from '@skeletonlabs/skeleton';

    export const connection = setupSignalRConnection('/api/hub', {});

    let state: DisplayState = DisplayState.Connecting;

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

        // Create an interval that increases the progress bar from 0 to 100
        // over the course of 3 seconds
        progress = 0;
        var intervalFunc = intervalBuilder(uid);
        interval = setInterval(intervalFunc, 1000 / 10);
        intervalFunc();

        connection.invoke('RegisterFirstAdminCard', uid);
    });

    function intervalBuilder(uid: string)
    {
        return () => {
            progress = progress + (100 / 10);
        
            if (progress >= 100) {
                progress = 100;
                card = uid;
                clearInterval(interval);
            }
        }
    }

    connection.on('CardRemoved', () => {
        card = null;
        messages = [...messages, 'Card removed'];

        progress = 0;
        clearInterval(interval);
    });

    let messages = [''];
    let card = null;
    let progress = 25;
    
    let interval;
</script>

<style lang="postcss">
    :global(.glow) {
        @apply bg-success-700/25;
        box-shadow: 0 0 50px theme('colors.success.700');
    }
</style>

<div class='space-y-6'>
    <StateHeader {state} />

    {#if state === DisplayState.FirstTimeSetup}
        <Card {card} />

        <ProgressRadial class="mx-auto {progress == 100 ? 'glow' : ''}" width="w-12" stroke={100} meter={progress < 100 ? "stroke-primary-500" : "stroke-success-500"} value={Math.min(100, progress)} max={100} />

        {#if progress === 0}
            <div>Scan a card to register it as an admin card.</div>
        {:else if progress < 100}
            <div>Wait...</div>
        {:else}
            <div>Admin card registered.</div>
        {/if}
    {/if}

    <div class='text-left shadow bg-neutral-900/90 rounded-container-token p-4'>
        <div class='text-sm border-b border-b-neutral-500 mb-2 pb-1' style='    font-variant-caps: small-caps;'>
            System Log
        </div>
        <div class='font-mono'>
            {#each messages as message}
                <div>{message}</div>
            {/each}
        </div>
    </div>
</div>
