<script lang="ts">
    import {changeState, clearAlert, connection, showAlert, team, updateUser, user} from "../../stores";
    import {type Card, CardType, DisplayState, redeemable} from "../../services/game";
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
            $connection.invoke('ResetCard', _card.uid);
        }
    }

    onMount(() => {
        $connection.on('CardInserted', cardInsertedHandler);

        return () => {
            $connection.off('CardInserted', cardInsertedHandler);
        }
    });
</script>

<button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
