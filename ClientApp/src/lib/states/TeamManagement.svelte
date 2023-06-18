<script lang="ts">
    import {changeState, connection} from "../../stores";
    import {DisplayState} from "../../services/game";

    let importData = '';
    
    $: console.log(importData.length);
    
    function removeAllUsers(): void {
        if (!confirm('Are you sure you want to remove all users and teams?')) {
            return;
        }
        $connection.invoke("RemoveAllUsers");
    }

    function recreateTeams(): void {
        if (!confirm('Are you sure you want to remove and recreate all teams?')) {
            return;
        }
        $connection.invoke("RecreateTeams");
    }
    
    function importUsers(): void {
        $connection.invoke("ImportUsers", importData);
    }
</script>

<div class="flex gap-4">
    <button class="btn variant-filled-error" on:click={removeAllUsers}>Remove all Users</button>
    <button class="btn variant-filled-error" on:click={recreateTeams}>Recreate Teams</button>
    <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
</div>

<div class="card w-full" on:submit|preventDefault={importUsers}>
    <form>
        <header class="card-header">
            <div class="font-bold">Import user data</div>
            <div>Copy the contents of the <em>Participants</em> sheet from the <em><a class="anchor" href="https://docs.google.com/spreadsheets/d/1ZMYN5d8O_2mWi-lBkJnEblvOrTVlOZ4J_3_cutLe9WI/edit#gid=97576828" target="_blank">Big Book of Camp Attendees</a></em> into the text box below.</div>
            <div>Users will only be created, not updated, so it's recommended that you remove all users before performing the import.</div>
        </header>
        <section class="p-4">
            <textarea required class="textarea" rows="5" placeholder="Paste the sheet data here." bind:value={importData}></textarea>
            <div class="text-xs">Size limit: {importData.length} / 30000 bytes</div>
        </section>
        <footer class="card-footer">
            <button type="submit" class="btn variant-filled-primary">Import Users</button>
        </footer>
    </form>
</div>
