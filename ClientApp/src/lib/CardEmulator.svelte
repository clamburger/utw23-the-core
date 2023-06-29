<script lang="ts">
    import {connection} from "../stores";

    let id;
    let label = '';
    let uid = '';
    
    function scanById() {
        if (id) {
            $connection.invoke("EmulateScanById", id);
            setTimeout(removeCard, 2000);
        } else {
            removeCard();
        }
    }
    
    function scanByLabel() {
        if (label) {
            $connection.invoke("EmulateScanByLabel", label);
            setTimeout(removeCard, 2000);
        } else {
            removeCard();
        }
    }
    
    function scanByUid() {
        if (uid) {
            $connection.invoke("EmulateScanByUid", uid);
            setTimeout(removeCard, 2000);
        } else {
            removeCard();
        }
    }
    
    function removeCard() {
        $connection.invoke("EmulateCardRemoved");
    }
</script>


<div class="shadow bg-neutral-900/90 px-2 py-1" style="width: 100px;">
    <form class="flex" on:submit|preventDefault={scanById}>
        <input type="number" placeholder="ID" class="input" bind:value={id} />
        <button class="btn">Scan</button>
    </form>
    <form class="flex" on:submit|preventDefault={scanByLabel}>
        <input type="text" placeholder="Label" class="input" bind:value={label} />
        <button class="btn">Scan</button>
    </form>
    <form class="flex" on:submit|preventDefault={scanByUid}>
        <input type="text" placeholder="UID" class="input" bind:value={uid} />
        <button class="btn">Scan</button>
    </form>
</div>
