<script lang="ts">
    import {type Card, DisplayState, label, type Team} from '../services/game';
    import { setupSignalRConnection } from '../services/hub';
    import StateHeader from './StateHeader.svelte';
    import {
        alert,
        team,
        connection,
        state,
        addLog,
        cardRemoved,
        changeState,
        showAlert,
        clearAlert,
        updateCard, updateTeam, card
    } from "../stores";
    import FirstTimeSetup from "./states/FirstTimeSetup.svelte";
    import AdminDashboard from "./states/AdminDashboard.svelte";
    import ResettingCards from "./states/ResettingCards.svelte";
    import RegisteringCards from "./states/RegisteringCards.svelte";
    import Disconnected from "./states/Disconnected.svelte";
    import Ready from "./states/Ready.svelte";
    import TeamManagement from "./states/TeamManagement.svelte";
    import LoggedIn from "./states/LoggedIn.svelte";
    import ShopManagement from "./states/ShopManagement.svelte";
    import Shop from "./states/Shop.svelte";
    import ConfirmPurchase from "./states/ConfirmPurchase.svelte";
    import TeamSummary from "./states/TeamSummary.svelte";

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
        updateCard(_card);
        addLog(`Card scanned: ${_card.uid} (${label(_card)})`);
    });

    $connection.on('CardRemoved', () => {
        cardRemoved();
        // addLog('Card removed');
        clearAlert();
    });
    
    $connection.on('TeamUpdate', (_team: Team) => {
       if (_team.id === $team?.id) {
           updateTeam(_team);
       } 
    });
    
    $connection.on('CardUpdate', (_card: Card) => {
        if (_card.id === $card?.id) {
            updateCard(_card);
        }
    })
</script>

<div class="flex flex-col flex-grow space-y-6 items-center">
    <StateHeader />

    {#if $state === DisplayState.FirstTimeSetup}
        <FirstTimeSetup />
    {:else if $state === DisplayState.Ready}
        <Ready />
    {:else if $state === DisplayState.LoggedIn}
        <LoggedIn />
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
    {:else if $state === DisplayState.ShopManagement}
        <ShopManagement />
    {:else if $state === DisplayState.Shop}
        <Shop />
    {:else if $state === DisplayState.ConfirmPurchase}
        <ConfirmPurchase />
    {:else if $state === DisplayState.TeamSummary}
        <TeamSummary />
    {/if}
</div>
