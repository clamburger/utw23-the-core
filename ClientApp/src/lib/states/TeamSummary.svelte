<script lang="ts">
    import {changeState, signOut} from "../../stores";
    import {DisplayState, itemStatus, type Team} from "../../services/game";
    import {onMount} from "svelte";
    import wretch from "wretch";
    import firstBy from "thenby";
    import ShopItemComponent from "../ShopItemComponent.svelte";
    
    onMount(() => {
        fetchTeams();
    });
    
    let teams: Team[] = [];
    
    $: console.log(teams);
    
    function fetchTeams() {
        wretch('/api/Admin/teams')
            .get()
            .json(result => {
                teams = result.filter(t => t.name !== 'Helpers')
                    .sort(firstBy('name'));
            });
    }
    
    function totalValue(team: Team) {
        return team.shopItems.reduce((prev, cur) => prev + cur.price, 0) + team.balance;
    }
</script>

<div class="flex gap-4">
    <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
    <button class="btn variant-filled-error" on:click={() => signOut()}>Sign Out</button>
</div>

{#each teams as team}
    <div class="font-semibold text-xl pt-8">
        {team.name} (<span class="text-yellow-300"><span class="font-extrabold">{team.balance}</span> cr</span>,
        total value <span class="text-green-300"><span class="font-extrabold">{totalValue(team)}</span> cr</span>)
    </div>
    <div class="grid grid-cols-5 gap-4">
        {#each team.shopItems as item}
            <ShopItemComponent {item} admin />
        {/each}
    </div>
{/each}
