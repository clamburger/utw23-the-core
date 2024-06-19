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
        <span class='header from-lime-300 to-emerald-500'>
            Ready To Scan
        </span>
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
    {:else if $state === DisplayState.ScannerInfo}
        <span class='header from-purple-600 to-purple-300'>
            Scanner Info
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
    {:else if $state === DisplayState.VoteResults}
        <span class="header from-rose-100 to-violet-600">
            Vote Results
        </span>
    {:else}
        <span class='header from-stone-500 to-slate-300'>
            Unknown State    
        </span>
    {/if}
</h1>
