<script lang="ts">
    import {type Card, CardType, DisplayState} from "../../services/game";
    import {
        changeState,
        clearAlert,
        connection,
        showAlert, updateUser
    } from "../../stores";
    import {onMount} from "svelte";

    function cardInsertedHandler(_card: Card) {
        clearAlert();
        
        if (_card?.id && !_card.enabled) {
            return;
        }
        
        if (_card.type === CardType.Admin) {
            changeState(DisplayState.AdminDashboard);
        } else if (_card.type === CardType.Person) {
            updateUser(_card.user);
            changeState(DisplayState.LoggedIn);
            $connection.invoke('LoggedIn', _card.uid, _card.user.id);
        } else if (_card.redeemed) {
            showAlert('info', '', 'already-redeemed');
        }
    }

    onMount(() => {
        $connection.on('CardInserted', cardInsertedHandler);

        return () => {
            $connection.off('CardInserted', cardInsertedHandler);
        }
    });
</script>
