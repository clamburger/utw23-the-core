<script lang="ts">
    import {onMount} from "svelte";
    import firstBy from "thenby";
    import wretch from "wretch";
    import {DisplayState, type ShopItem} from "../../services/game";
    import {changeState, signOut, team, updateShopItem, user} from "../../stores";
    import ShopItemComponent from "../ShopItemComponent.svelte";

    let items: ShopItem[] = [];
    
    onMount(() => {
        fetchUsers();
    });
    
    function fetchUsers() {
        wretch('/api/Admin/shop-items')
            .get()
            .json(result => {
                items = result.sort(
                    firstBy('redeemed', -1)
                        .thenBy('available', -1)
                        .thenBy('type')
                        .thenBy((a, b) => a.name.localeCompare(b.name, undefined, {numeric: true}))
                );
            });
    }
    
    function itemClicked(item: ShopItem) {
        if (item.redeemed) {
            return;
        }
        
        if (!item.owner && !item.available) {
            return;
        }
        
        if (item.owner && item.owner.id !== $team.id) {
            return;
        }
        
        if (item.price > $team.balance && (item.owner.id !== $team.id)) {
            return;
        }
        
        updateShopItem(item);
        changeState(DisplayState.ConfirmPurchase);
    }
</script>



{#if $team}
    <div class="text-3xl font-semibold">
        {$team.name} (<span class="text-yellow-300"><span class="font-extrabold">{$team.balance}</span> cr</span>)
    </div>
    <div class="text-3xl">Select an item to purchase it. Scroll to view all items.</div>
{/if}

<div class="flex gap-4">
    <button class="btn btn-xl variant-filled-tertiary" on:click={() => changeState(DisplayState.LoggedIn)}>Back to Card Scanner</button>
    <button class="btn btn-xl variant-filled-secondary" on:click={() => signOut()}>Sign Out</button>
</div>

<div class="grid grid-cols-5 gap-4">
    {#each items as item}
        <ShopItemComponent {item} onClick={() => itemClicked(item)} />
    {/each}
</div>
