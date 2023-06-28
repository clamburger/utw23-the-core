<script lang="ts">

    import {CardType, DisplayState, ShopItemType} from "../../services/game";
    import {RadioGroup, RadioItem, SlideToggle} from "@skeletonlabs/skeleton";
    import {card, connection, changeState} from "../../stores";

    let type = CardType.Admin;
    let name;
    let price = 1000;
    let available = false;
    
    function submitItem() {
        $connection.invoke("AddShopItem", name, type, price, available)
    }
</script>

<RadioGroup>
    <RadioItem bind:group={type} name="cardType" value={ShopItemType.StandardLego}>Standard Lego</RadioItem>
    <RadioItem bind:group={type} name="cardType" value={ShopItemType.SpecialLego}>Special Lego</RadioItem>
<!--    <RadioItem bind:group={type} name="cardType" value={ShopItemType.SpecialReward} disabled>Special Reward</RadioItem>-->
    <RadioItem bind:group={type} name="cardType" value={ShopItemType.Minifig}>Minifig</RadioItem>
</RadioGroup>

<div class="card w-1/2 overflow-hidden">
    <form on:submit|preventDefault={submitItem}>
        <section class="flex gap-4 p-4">
            <label class="label basis-2/6">
                <span>Item Name</span>
                <input class="input" type="text" bind:value={name} required>
            </label>
    
            <label class="label basis-2/6">
                <span>Price</span>
                <input class="input" type="number" bind:value={price} step="100" required>
            </label>
            
            <label class="label basis-2/6 self-end">
                <div>Available</div>
                <SlideToggle name="slider-label" bind:checked={available} active="bg-primary-500" />
            </label>
        </section>
        <hr>
        <footer class="card-footer p-4 flex justify-between items-center">
            <button class="btn variant-filled-primary" type="submit">
                Submit
            </button>
            <button class="btn variant-filled-secondary" type="button" on:click={() => changeState(DisplayState.AdminDashboard)}>
                Finished
            </button>
        </footer>
    </form>
</div>

