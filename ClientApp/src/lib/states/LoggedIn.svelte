<script lang="ts">
    import {card, team, changeState, clearAlert, connection, showAlert, signOut, updateUser, user} from "../../stores";
    import {type Card, CardType, DisplayState, redeemable} from "../../services/game";
    import {onMount} from "svelte";

    function cardInsertedHandler(_card: Card) {
        clearAlert();
        
        if (_card.type === CardType.Person) {
            updateUser(_card.user);
        } else if (!$team) {
            showAlert("error", "Cannot redeem card - user has no team.", "no-team");
        } else if (_card.redeemed) {
            showAlert("error", "Cannot redeem card - already redeemed.", "already-redeemed");
        } else if (!redeemable(_card.type)) {
            showAlert("error", "Invalid card type.", "invalid-card-type");
        } else {
            $connection.invoke('RedeemCard', _card.uid, $user.id);
        }
    }

    onMount(() => {
        $connection.on('CardInserted', cardInsertedHandler);
    
        return () => {
            $connection.off('CardInserted', cardInsertedHandler);
        }
    });
</script>

{#if $team}
    <div class="text-3xl font-semibold">{$team.name}</div>
    <div class="text-3xl">Balance: <span class="text-yellow-300 font-black">{$team.balance}</span></div>
{/if}

<div class="flex gap-4">
    {#if $user?.leader}
        <button class="btn variant-filled-primary" on:click={() => changeState(DisplayState.AdminDashboard)}>Admin Management</button>
    {/if}
    <button class="btn variant-filled-secondary" on:click={() => signOut()}>Sign Out</button>
</div>

