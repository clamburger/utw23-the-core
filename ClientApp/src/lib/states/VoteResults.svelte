<script lang="ts">
    import {connection, changeState} from "../../stores";
    import {DisplayState} from "../../services/game";
    import {onMount} from "svelte";
    import wretch from "wretch";

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
                
                results = Object.keys(options).map(number => {
                    return {
                        option: options[number],
                        votes: votes.filter(v => v.option.number == number).length
                    };
                });
            });
    }
    
    $: console.log(results);
</script>

<div class="card xl:w-1/2 overflow-hidden">
    <div class="table-container">
        <table class="table table-interactive table-compact">
            <thead>
            <tr>
                <th>Elective</th>
                <th>Participant Count</th>
                <th>Full?</th>
            </tr>
            </thead>
            <tbody>
            {#each results as result}
                <tr>
                    <td>{result.option.name}</td>
                    <td>{result.votes}</td>
                    <td>
                        {#if result.option.enabled}
                            <span class="text-green-500">No</span>
                        {:else}
                            <span class="text-red-500">Yes</span>
                        {/if}
                    </td>
                </tr>
            {/each}
            </tbody>
        </table>
    </div>
</div>

<div class="flex gap-4">
    <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
</div>
