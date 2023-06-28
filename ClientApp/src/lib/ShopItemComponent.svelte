<script lang="ts">
    import {ItemStatus, itemStatus as itemStatusCall, type ShopItem, ShopItemType} from "../services/game.js";
    import {team} from "../stores";

    export let item: ShopItem;
    
    $: itemStatus = item ? itemStatusCall(item, $team) : null;
    
    export let onClick: () => void = null;
    
    $: purchasable = [ItemStatus.Purchasable, ItemStatus.ReservedForYou].includes(itemStatus);
    $: owned = itemStatus === ItemStatus.OwnedByYou;
</script>

<style lang="scss">
    .image {
        aspect-ratio: 16/9;
        object-fit: contain;
    }

    .unavailable {
        opacity: 0.5;
        cursor: not-allowed;
    }
    
    .owner {
        @apply border-green-700 bg-green-500 border-2 text-black;
    }

    :global(.card-hover) {
        cursor: pointer;
    }
</style>



<div class="card card-hover shop-item overflow-hidden"
     class:owner={owned}
     class:unavailable={!purchasable && !owned}
     class:card-hover={purchasable}
     on:click={() => onClick && onClick()}>
    {#if item.type === ShopItemType.StandardLego}
        <header style="background-color: #EDEDED">
            <img src="/item-lego-standard.jpeg" class="image">
        </header>
    {:else if item.type === ShopItemType.SpecialLego}
        <header style="background-color: #FFFFFF;">
            <img src="/item-lego-special.png" class="image">
        </header>
    {:else if item.type === ShopItemType.Minifig}
        <header style="background-color: #B8B8B8">
            <img src="/item-lego-minifig.jpep" class="image">    
        </header>
    {/if}
    <div class="px-4 py-2 flex justify-between">
        <span>{item.name}</span>
        <!--{#if itemStatus === ItemStatus.OwnedByYou}-->
        <!--    <span class="text-green-500 text-white font-bold">Owned</span>-->
        <!--{:else if itemStatus === ItemStatus.OwnedByOtherTeam}-->
        <!--    <span class="text-gray-400 font-bold">Sold</span>-->
        {#if itemStatus === ItemStatus.ReservedForYou}
            <span class="text-green-500 font-bold">Free</span>
        <!--{:else if itemStatus === ItemStatus.ReservedForOtherTeam || itemStatus === ItemStatus.UnclaimedReward}-->
        <!--    <span class="text-gray-400 font-bold">Reward</span>-->
        <!--{:else if itemStatus === ItemStatus.FutureUnlock}-->
        <!--    <span class="text-gray-400 font-bold">Locked</span>-->
        {:else if itemStatus === ItemStatus.PurchasableTooExpensive}
            <span class="text-red-500 font-bold">{item.price} cr</span>
        {:else if itemStatus === ItemStatus.Purchasable}
            <span class="text-yellow-300 font-bold">{item.price} cr</span>
        {/if}
    </div>
    {#if item.owner || item.rewardCard}
        <div class="px-4 py-2 flex justify-between">
            {#if item.redeemed}
                Owned by {item.owner.name}
            {:else if item.owner}
                Reward for {item.owner.name}
            {:else}
                Team Challenge Reward
            {/if}
        </div>
    {/if}
</div>
