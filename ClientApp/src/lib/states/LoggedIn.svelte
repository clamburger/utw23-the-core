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

<!-- {#if $team}
    <div class="text-3xl font-semibold">
        {$team.name} (<span class="text-yellow-300"><span class="font-extrabold">{$team.balance}</span> cr</span>)
    </div>
{/if} -->

<!-- <div class="flex gap-4">
    {#if $user.name === 'Sam Horn'}
        <button class="btn btn-xl variant-filled-primary" on:click={() => changeState(DisplayState.AdminDashboard)}>Admin Management</button>
    {/if}
    <button class="btn btn-xl variant-filled-tertiary" on:click={() => changeState(DisplayState.Shop)}>The Shop</button>
    <button class="btn btn-xl variant-filled-secondary" on:click={() => signOut()}>Sign Out</button>
</div> -->

