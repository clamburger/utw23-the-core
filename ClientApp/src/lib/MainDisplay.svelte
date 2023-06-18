<script lang="ts">
    import { type Card, DisplayState } from '../services/game';
    import { setupSignalRConnection } from '../services/hub';
    import StateHeader from './StateHeader.svelte';
    import {
        alert,
        connection,
        state,
        addLog,
        cardRemoved,
        changeState,
        showAlert,
        clearAlert,
        cardInserted
    } from "../stores";
    import FirstTimeSetup from "./states/FirstTimeSetup.svelte";
    import AdminDashboard from "./states/AdminDashboard.svelte";
    import ResettingCards from "./states/ResettingCards.svelte";
    import RegisteringCards from "./states/RegisteringCards.svelte";
    import Disconnected from "./states/Disconnected.svelte";
    import Ready from "./states/Ready.svelte";
    import TeamManagement from "./states/TeamManagement.svelte";

    $connection = setupSignalRConnection('/api/hub', {});

    $connection.on('ScreenRegistered', (firstTimeSetup: boolean) => {
        addLog("Registered as the primary screen");

        if (firstTimeSetup) {
            changeState(DisplayState.RegisteringCards);
            addLog("No admin cards registered - entering first time setup");
        } else {
            changeState(DisplayState.Ready);
        }
    });

    $connection.on('ScreenDeregistered', () => {
        changeState(DisplayState.Disconnected);
        addLog("Disconnected: another screen has become the primary");
    });

    $connection.on('SystemMessage', (message: string) => {
        addLog(message);
    });

    $connection.on('SystemError', (message: string) => {
        showAlert("error", message);
    });

    $connection.on('SystemSuccess', (message: string) => {
        showAlert("success", message);
    });
    
    $connection.on('CardInserted', (_card: Card) => {
        cardInserted(_card);
        addLog(`Card scanned: ${_card.uid}`);
    });

    $connection.on('CardRemoved', () => {
        cardRemoved();
        // addLog('Card removed');
        clearAlert();
    });
</script>

<div class="flex flex-col flex-grow space-y-6 items-center">
    <StateHeader />

    {#if $state === DisplayState.FirstTimeSetup}
        <FirstTimeSetup />
    {:else if $state === DisplayState.Ready}
        <Ready />
    {:else if $state === DisplayState.AdminDashboard}
        <AdminDashboard />
    {:else if $state === DisplayState.ResettingCards}
        <ResettingCards />
    {:else if $state === DisplayState.RegisteringCards}
        <RegisteringCards />
    {:else if $state === DisplayState.TeamManagement}
        <TeamManagement />
    {:else if $state === DisplayState.Disconnected}
        <Disconnected />
    {/if}
</div>
