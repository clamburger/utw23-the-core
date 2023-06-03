<script lang='ts'>
    import { DisplayState } from "../services/game";

    export let state: DisplayState;
</script>

<h1 class="text-5xl font-bold">
    {#if state === DisplayState.Connecting}
        <span class='bg-gradient-to-br from-red-500 to-red-300 bg-clip-text text-transparent box-decoration-clone capitalize'>
            Connecting...
        </span>
    {:else if state === DisplayState.Disconnected}
        <span class='bg-gradient-to-br from-red-500 to-red-700 bg-clip-text text-transparent box-decoration-clone capitalize'>
            Not connected
        </span>
    {:else if state === DisplayState.FirstTimeSetup}
        <span class='bg-gradient-to-br from-neutral-50 to-stone-500 bg-clip-text text-transparent box-decoration-clone capitalize'>
            First time setup
        </span>
    {:else if state === DisplayState.Ready}
        <span class='bg-gradient-to-br from-lime-300 to-emerald-500 bg-clip-text text-transparent box-decoration-clone capitalize'>
            Ready to scan
        </span>
    {:else if state === DisplayState.ResettingCards}
        <span class='bg-gradient-to-br from-orange-500 to-yellow-300 bg-clip-text text-transparent box-decoration-clone capitalize'>
            Resetting cards
        </span>
    {:else if state === DisplayState.RegisteringCards}
        <span class='bg-gradient-to-br from-yellow-500 to-orange-300 bg-clip-text text-transparent box-decoration-clone capitalize'>
            Registering cards
        </span>
    {:else if state === DisplayState.AdminDashboard}
        <span class='bg-gradient-to-br from-blue-500 to-blue-300 bg-clip-text text-transparent box-decoration-clone capitalize'>
            Admin dashboard
        </span>
    {:else}
        <span class='bg-gradient-to-br from-stone-500 to-slate-300 bg-clip-text text-transparent box-decoration-clone capitalize'>
            Unknown state    
        </span>
    {/if}
</h1>
