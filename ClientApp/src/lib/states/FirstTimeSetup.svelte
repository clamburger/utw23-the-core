<script lang="ts">
    import {card, changeState, clearAlert, connection} from "../../stores";
    import {type Card, DisplayState} from "../../services/game";
    import {onMount} from "svelte";

    function cardInsertedHandler(_card: Card) {
        clearAlert();
        $connection.invoke('RegisterFirstAdminCard', uid);
    }
    
    function cardRegisteredHandler(_card: Card) {
        changeState(DisplayState.AdminDashboard);
    }
    
    onMount(() => {
        $connection.on('CardInserted', cardInsertedHandler);
        $connection.on('CardRegistered', cardRegisteredHandler);
        
        return () => {
            $connection.off('CardInserted', cardInsertedHandler);
            $connection.off('CardRegistered', cardRegisteredHandler);
        }
    });
</script>

{#if !$card}
    <div>Scan a card to register it as an admin card.</div>
{:else}
    <div>Admin card registered.</div>
{/if}
