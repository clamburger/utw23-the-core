<script lang="ts">
    import {
        addLog,
        changeState,
        clearAlert,
        connection,
        showAlert,
        signOut,
        team,
        updateUser,
        user
    } from "../../stores";
    import {type Card, CardType, DisplayState, label, redeemable} from "../../services/game";
    import {onMount} from "svelte";
    
    function cardInsertedHandler(_card: Card) {
        clearAlert();

        if (!_card.id) {
            showAlert("warning", "", "unregistered");
        } else if (!_card.enabled) {
            showAlert("error", "Card is disabled.", "disabled");
        } else if (!redeemable(_card.type)) {
            showAlert("error", "Unsupported card type.", "unsupported");
        } else if (!_card.redeemed) {
            showAlert("success", "Card not redeemed - no need to reset.", "unsupported");
        } else {
            $connection.invoke('ResetCard', _card.uid, $user.id)
            addLog(`Card reset (${label(_card)})`);
        }
    }

    onMount(() => {
        $connection.on('CardInserted', cardInsertedHandler);

        return () => {
            $connection.off('CardInserted', cardInsertedHandler);
        }
    });
</script>

<div class="flex gap-4">
    <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
    <button class="btn variant-filled-error" on:click={() => signOut()}>Sign Out</button>
</div>
