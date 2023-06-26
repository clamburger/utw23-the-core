<script lang="ts">
    import {changeState, clearAlert, connection, shopItem, showAlert, team, user} from "../../stores"
    import ShopItemComponent from "../ShopItemComponent.svelte";
    import {type Card, CardType, DisplayState, type ShopItem} from "../../services/game";
    import {onMount} from "svelte";

    function cardInsertedHandler(_card: Card) {
        clearAlert();

        if (_card.type !== CardType.Person) {
            showAlert('error', "Invalid card type.");
            return;
        } else if (!_card.user.leader) {
            showAlert('error', "Must scan a leader card to confirm purchase.");
            return;
        } else {
            $connection.invoke('ConfirmPurchase', $user.id, _card.user.id, $shopItem.id);
        }
    }
    
    function purchaseSuccessfulHandler(_item: ShopItem) {
        showAlert('success', `Purchase of ${$shopItem.name} confirmed. Enjoy!`);
        changeState(DisplayState.LoggedIn);
    }

    onMount(() => {
        $connection.on('CardInserted', cardInsertedHandler);
        $connection.on('PurchaseSuccessful', purchaseSuccessfulHandler);

        return () => {
            $connection.off('CardInserted', cardInsertedHandler);
            $connection.off('PurchaseSuccessful', purchaseSuccessfulHandler);
        }
    });
</script>

<div class="flex gap-16 items-center">
    <div class="flex flex-col justify-center items-center gap-4">
        <div class="text-3xl font-semibold">
            {$team.name} (<span class="text-yellow-300"><span class="font-extrabold">{$team.balance}</span> cr</span>)
        </div>
        <div class="flex gap-4">
            <button class="btn btn-xl variant-filled-error" on:click={() => changeState(DisplayState.Shop)}>Cancel Purchase</button>
<!--            <button class="btn btn-xl variant-filled-secondary" on:click={() => signOut()}>Sign Out</button>-->
        </div>
    </div>
    <div style="width: 300px;">
        <ShopItemComponent item={$shopItem} />
    </div>
</div>

