<script lang="ts">
    import {connection, changeState, signOut} from "../../stores";
    import {DisplayState} from "../../services/game";
    
    function unregisterAllCards(): void {
        $connection.invoke("UnregisterAllCards");
    }
</script>

<div class="flex gap-4">
<!--    <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.ResettingCards)}>Reset Redeemed Cards</button>-->
    <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.ScannerInfo)}>Scanner Info</button>
    <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.VoteResults)}>Vote Results</button>
    <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.RegisteringCards)}>Register New Cards</button>
    <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.TeamManagement)}>Team Management</button>
<!--    <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.ShopManagement)}>Shop Management</button>-->
<!--    <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.TeamSummary)}>Team Summary</button>-->
    <button class="btn variant-filled-error" on:click={() => confirm('Unregister all cards?') && unregisterAllCards}>Unregister All Cards</button>
<!--    <button class="btn variant-filled-error" on:click={() => signOut()}>Sign Out</button>-->
</div>
