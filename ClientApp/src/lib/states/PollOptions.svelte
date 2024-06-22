<script lang="ts">
    import {changeState, connection} from "../../stores.js";
    import {DisplayState} from "../../services/game.js";
    import {onMount} from "svelte";
    import wretch from "wretch";
    import {SlideToggle} from "@skeletonlabs/skeleton";

    let options = [];
    
    onMount(() => {
        fetchResults();
    });
    
    function fetchResults() {
        wretch('/api/Admin/poll-options')
            .get()
            .json(result => {
                options = result;
            });
    }
    
    function updateEnabled(option) {
        $connection.invoke("UpdateOptionEnabled", option.id, option.enabled);
    }
    
    function updateLabel(option) {
        $connection.invoke("UpdateOptionLabel", option.id, option.name);
    }

    function updateLimit(option) {
        $connection.invoke("UpdateOptionLimit", option.id, option.limit);
    }
    
    function optionUpdatedHandler(_option) {
        const index = options.findIndex(o => _option.id === o.id);
        options[index] = _option
        options = [...options]
        
        // console.log(options, _option)
    }
    
    function optionAddedHandler(_option) {
        options = [...options, _option];
    }
    
    function deleteOption(option) {
        $connection.invoke("DeleteOption", option.id);
        options = options.filter(o => o.id !== option.id);
    }
    
    function addOption() {
        $connection.invoke("AddOption");
    }

    onMount(() => {
        $connection.on('OptionUpdated', optionUpdatedHandler);
        $connection.on('OptionAdded', optionAddedHandler);

        return () => {
            $connection.off('OptionUpdated', optionUpdatedHandler);
            $connection.off('OptionAdded', optionAddedHandler);
        }
    });
    
    // $: console.log(options);
</script>

<style lang="scss">
    .input-group-custom {
        display: flex;
        
        .input {
            border-top-right-radius: 0;
            border-bottom-right-radius: 0;
        }
        
        .btn {
            border-top-left-radius: 0;
            border-bottom-left-radius: 0;
        }
    }
</style>

<div class="card xl:w-1/2 overflow-hidden">
    <div class="table-container">
        <table class="table table-interactive table-compact">
            <thead>
            <tr>
                <th></th>
                <th>Option</th>
                <th>Limit</th>
                <th>Enabled</th>
                <th></th>
            </tr>
            </thead>
            <tbody>
            {#each options as option, index}
                <tr>
                    <td>
                        <span class="text-3xl font-bold">{index + 1}</span>
                    </td>
                    <td>
                        <div class="input-group-custom">
                            <input class="input with-button w-auto" type="text" bind:value={option.name} required>
                            <button class="btn variant-filled-success" on:click={() => updateLabel(option)}>✓</button>
                        </div>
                    </td>
                    <td>
                        <div class="input-group-custom">
                            <input class="input w-[100px]" type="number" min="0" bind:value={option.limit}>
                            <button class="btn variant-filled-success" on:click={() => updateLimit(option)}>✓</button>
                        </div>
                    </td>
                    <td class="!align-middle">
                        <SlideToggle name="slider-label" active="bg-primary-500" bind:checked={option.enabled} on:change={() => updateEnabled(option)}/>
                    </td>
                    <td>
                        <button class="btn variant-filled-error" on:click={() => confirm('Remove this option?') && deleteOption(option)}>-</button>
                    </td>
                </tr>
            {/each}
            </tbody>
        </table>

<!--        <footer class="card-footer p-4 flex justify-between items-center">-->
<!--            <SlideToggle name="slider-label" active="bg-primary-500"  value>Submit automatically on scan</SlideToggle>-->
<!--        </footer>-->
    </div>
</div>

<div class="flex gap-4">
    <button class="btn variant-filled-primary" on:click={() => addOption()} disabled={options.length >= 9}>
        {options.length >= 9 ? 'Maximum 9 Options' : 'Add Option'}
    </button>
    <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
</div>
