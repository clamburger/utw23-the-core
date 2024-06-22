<script lang="ts">
    import {connection, changeState, showVoters} from "../../stores";
    import {DisplayState} from "../../services/game";
    import {onMount} from "svelte";
    import wretch from "wretch";
    import {SlideToggle} from "@skeletonlabs/skeleton";

    let options = {};
    let votes = [];
    let results = [];
    
    onMount(() => {
        fetchResults();
    });

    $connection.on('ScannerUpdate', () => {
        fetchResults();
    });

    function fetchResults() {
        wretch('/api/Admin/vote-results')
            .get()
            .json(result => {
                options = result.options.reduce((prev, cur) => {
                    prev[cur.id] = cur;
                    return prev;
                }, {});
                
                votes = result.votes;
                
                results = Object.keys(options).map(id => {
                    return {
                        option: options[id],
                        votes: votes.filter(v => v.option.id == id)
                    };
                });
            });
    }
    
    function listVoters(votes) {
        return votes.map(vote => vote.user.name).sort().join(', ');
    }
    
    function clearVotes() {
        $connection.invoke("ClearVotes");
    }
    
    $: console.log(results);
</script>

<div class="card w-full overflow-hidden">
    <div class="table-container">
        <table class="table table-interactive table-compact">
            <thead>
            <tr class="!text-2xl">
                <th class="w-1/4">Elective</th>
                <th class="w-1/2">
                    <SlideToggle name="slider-label" bind:checked={$showVoters} active="bg-primary-500">Voters</SlideToggle>
                 </th>
                <th class="w-1/4">Available?</th>
            </tr>
            </thead>
            <tbody>
            {#each results as result}
                <tr>
                    <td class="!text-2xl">
                        {result.option.name}
                    </td>
                    <td>
                        <div class="text-2xl">{result.votes.length}</div>
                        {#if $showVoters}
                            <div class="mt-2">{listVoters(result.votes)}</div>
                        {/if}
                    </td>
                    <td class="!text-2xl">
                        {#if result.option.enabled}
                            <span class="text-green-500">Yes</span>
                            {#if result.option.limit !== null}
                                ({Math.max(1, result.option.limit - result.votes.length)} left)
                            {/if}
                        {:else}
                            <span class="text-gray-500">No</span>
                        {/if}
                    </td>
                </tr>
            {/each}
            </tbody>
        </table>
    </div>
</div>

<div class="flex gap-4">
    <button class="btn variant-filled-primary" on:click={fetchResults}>Refresh</button>
    <button class="btn variant-filled-error" on:click={() => confirm('Clear all votes?') && clearVotes()}>Clear Votes</button>
    <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
</div>
