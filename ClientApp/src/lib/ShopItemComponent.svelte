<script lang="ts">
    import {type ShopItem, ShopItemType} from "../services/game.js";
    import {team} from "../stores";

    export let item: ShopItem;
    
    export let onClick: () => void = null;
    
    $: purchasable =
        ((!item.redeemed && item.owner?.id === $team.id)
        || (item.available && !item.owner && !item.redeemed && $team.balance > item.price))
         && $team;
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

    :global(.card-hover) {
        cursor: pointer;
    }
</style>



<div class="card card-hover shop-item overflow-hidden" class:unavailable={!purchasable} class:card-hover={purchasable} on:click={() => onClick && onClick()}>
    {#if item.type === ShopItemType.StandardLego}
        <header style="background-color: #EDEDED">
            <img src="/item-lego-standard.jpeg" class="image">
        </header>
    {:else if item.type === ShopItemType.SpecialLego}
        <header style="background-color: #FFFFFF;">
            <img src="/item-lego-special.png" class="image">
        </header>
    {/if}
    <div class="p-4 flex justify-between">
        <span>{item.name}</span>
        {#if item.redeemed}
            <span class="text-gray-400 font-bold">Sold</span>
        {:else if !item.redeemed && item.owner?.id === $team.id}
            <span class="text-green-500 font-bold">Free</span>
        {:else if !item.available}
            <span class="text-gray-400 font-bold">Locked</span>
        {:else if item.price > $team.balance}
            <span class="text-red-500 font-bold">{item.price} cr</span>
        {:else}
            <span class="text-yellow-300 font-bold">{item.price} cr</span>
        {/if}
    </div>
</div>
