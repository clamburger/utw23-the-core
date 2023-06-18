<script lang="ts">
    import {connection, changeState} from "../../stores";
    import {DisplayState} from "../../services/game";
    
    function unregisterAllCards(): void {
        $connection.invoke("UnregisterAllCards");
    }
</script>

<div class="flex gap-4">
    <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.ResettingCards)}>Reset Redeemed Cards</button>
    <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.RegisteringCards)}>Register New Cards</button>
    <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.TeamManagement)}>Team Management</button>
    <button class="btn variant-filled-error" on:click={unregisterAllCards}>Unregister All Cards</button>
    <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.Ready)}>Exit Admin Dashboard</button>
</div>
