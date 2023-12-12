<script lang='ts'>
    import { DisplayState } from "../services/game";
    import { state, user, team } from "../stores";
</script>

<style lang="scss">
    .header {
        @apply bg-gradient-to-br bg-clip-text text-transparent box-decoration-clone;
    }
</style>

<h1 class="text-5xl font-bold">
    {#if $state === DisplayState.Connecting}
        <span class='header from-red-500 to-red-300'>
            Connecting...
        </span>
    {:else if $state === DisplayState.Disconnected}
        <span class='header from-red-500 to-red-700'>
            Not Connected
        </span>
    {:else if $state === DisplayState.FirstTimeSetup}
        <span class='header from-neutral-50 to-stone-500'>
            First Time Setup
        </span>
    {:else if $state === DisplayState.Ready}
        <div class='header from-lime-300 to-emerald-500 text-center' style="line-height: 1.3" >
            Please scan to register your enjoyment of camp.
        </div>
    {:else if $state === DisplayState.LoggedIn}
        <span class='header from-blue-300 to-green-500'>
            Welcome, {$user.name}.
        </span>
    {:else if $state === DisplayState.Shop}
        <span class='header from-yellow-400 to-blue-400'>
            Welcome, {$user.name}.
        </span>
    {:else if $state === DisplayState.ResettingCards}
        <span class='header from-orange-500 to-yellow-300'>
            Resetting Cards
        </span>
    {:else if $state === DisplayState.RegisteringCards}
        <span class='header from-yellow-500 to-orange-300'>
            Registering Cards
        </span>
    {:else if $state === DisplayState.AdminDashboard}
        <span class='header from-blue-500 to-blue-300'>
            Admin Dashboard
        </span>
    {:else if $state === DisplayState.TeamManagement}
        <span class='header from-blue-100 to-blue-500'>
            Team Management
        </span>
    {:else if $state === DisplayState.ShopManagement}
        <span class="header from-yellow-400 to-blue-400">
            Shop Management
        </span>
    {:else if $state === DisplayState.ConfirmPurchase}
        <span class="header from-yellow-400 to-blue-400">
            Tap Leader Card to Confirm Purchase
        </span>
    {:else if $state === DisplayState.TeamSummary}
        <span class="header from-blue-50 to-blue-400">
            Team Summary
        </span>
    {:else}
        <span class='header from-stone-500 to-slate-300'>
            Unknown State    
        </span>
    {/if}
</h1>
