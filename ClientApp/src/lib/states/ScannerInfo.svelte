<script lang="ts">
    import {connection, changeState} from "../../stores";
    import {DisplayState, type Scanner, ScannerState} from "../../services/game";
    import {onMount} from "svelte";
    import wretch from "wretch";
    import firstBy from "thenby";

    onMount(() => {
        fetchScannerInfo();
    });

    let scanners: Scanner[] = [];
    let selected: string[] = [];
    // let state;
    // let states = [
    //     {id: ScannerState.Disabled, label: "Disabled"},
    //     {id: ScannerState.Ready, label: "Ready to Scan"},
    //     {id: ScannerState.InvalidCard, label: "Invalid Card"},
    //     {id: ScannerState.ReadyToSelect, label: "Select Option"},
    //     {id: ScannerState.OptionSelected, label: "Option Selected"},
    // ];
    
    $connection.on('ScannerUpdate', () => {
        fetchScannerInfo();
    });
    
    function toggleSelection(connectionId) {
        if (selected.includes(connectionId)) {
            selected = selected.filter(id => id !== connectionId);
        } else {
            selected = [...selected, connectionId];
        }
    }
    
    function toggleAll() {
        if (selected.length === scanners.length) {
            selected = [];
        } else {
            selected = scanners.map(s => s.connectionId);
        }
    }
    
    function fetchScannerInfo() {
        // save existing scanners
        let existingScanners = scanners.map(s => s.connectionId);
        
        wretch('/api/Admin/scanners')
            .get()
            .json(result => {
                scanners = result.sort(firstBy('scannerId'));

                // Remove selected scanners that are no longer in the list
                selected = selected.filter(id => scanners.map(s => s.connectionId).includes(id));

                // Automatically select new scanners
                selected = [...selected, ...scanners.map(s => s.connectionId).filter(id => !existingScanners.includes(id))];
            });
    }

    function updateState(state) {
        $connection.invoke("UpdateScannerState", state)
    }
    
    function disconnectSelected() {
        $connection.invoke("DisconnectScanners", selected)
    }
</script>

<div class="card xl:w-1/2 overflow-hidden">
    <section class="flex gap-4">
        <div class="table-container">
            <table class="table table-interactive table-compact">
                <thead>
                    <tr>
                        <th class="!px-4">
                            <input type="checkbox" checked={selected.length === scanners.length} on:change={() => toggleAll()}>
                        </th>
                        <th>Station ID</th>
                        <th>IP Address</th>
                        <th>Connection ID</th>
                        <th>State</th>
<!--                        <th>Card UID</th>-->
                    </tr>
                </thead>
                <tbody>
                    {#each scanners as scanner}
                        <tr class:table-row-checked="{selected.includes(scanner.connectionId)}" on:click={() => toggleSelection(scanner.connectionId)}>
                            <td class="!px-4"><input type="checkbox" checked={selected.includes(scanner.connectionId)}></td>
                            <td>{scanner.stationId || ''}</td>
                            <td>{scanner.ipAddress}</td>
                            <td>{scanner.connectionId}</td>
                            <td>
                                {#if scanner.state === ScannerState.Disabled}
                                    <span class="text-red-500">Disabled</span>
                                {:else if scanner.state === ScannerState.Ready}
                                    <span class="text-blue-300">Awaiting Scan</span>
                                {:else if scanner.state === ScannerState.InvalidCard}
                                    <span class="text-yellow-500">Invalid Card</span>
                                {:else if scanner.state === ScannerState.ReadyToSelect}
                                    <span class="text-green-200">Voting Screen</span>
                                {:else if scanner.state === ScannerState.OptionSelected}
                                    <span class="text-green-400">Option Selected</span>
                                {:else if scanner.state === null}
                                    <span class="text-gray-500">No Heartbeat Yet</span>
                                {:else}
                                    Unknown ({scanner.state})
                                {/if}
                            </td>
<!--                            <td>{scanner.cardUid || ''}</td>-->
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    </section>
    <hr>
    <footer class="card-footer p-4 flex items-center gap-4">
<!--        <label>State</label>-->
<!--        <select class="select" bind:value={state}>-->
<!--            {#each states as state}-->
<!--                <option value={state.id}>{state.label}</option>-->
<!--            {/each}-->
<!--        </select>-->
        <button class="btn variant-filled-primary" disabled={selected.length === 0} on:click={() => updateState(ScannerState.Ready)}>
            Enable
        </button>
        <button class="btn variant-filled-primary" disabled={selected.length === 0} on:click={() => updateState(ScannerState.Disabled)}>
            Disable
        </button>
        <button class="btn variant-filled-error" disabled={selected.length === 0} on:click={() => disconnectSelected()}>
            Disconnect
        </button>
    </footer>
</div>

<div class="flex gap-4">
<!--    <button class="btn variant-filled-primary" on:click={() => fetchScannerInfo()}>Force Reload</button>-->
    <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
</div>
