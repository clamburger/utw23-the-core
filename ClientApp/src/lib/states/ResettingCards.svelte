<script lang="ts">
    import {changeState} from "../../stores";
    import {DisplayState} from "../../services/game";
</script>

<button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
