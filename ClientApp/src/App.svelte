<script lang="ts">
  import MainDisplay from './lib/MainDisplay.svelte';
  import { AppShell } from '@skeletonlabs/skeleton';
  import SystemLog from "./lib/SystemLog.svelte";
  import CardReader from "./lib/CardReader.svelte";
  import Logo from "./lib/svg/Logo.svelte";
</script>

<style lang="scss">
    :global(.SystemLog .log) {
        height: 60px;
    }
    
    .logo {
        max-width: 400px;
    }
</style>

<AppShell>
    <div class="logo mx-auto my-4">
        <Logo />
    </div>
    
    <div class="p-4">
        <CardReader />
    </div>
    
    <div class="container mx-auto flex justify-center flex-grow px-8 pb-2 pt-0">
        <MainDisplay />
    </div>

    <svelte:fragment slot="footer">
        <SystemLog />
    </svelte:fragment>
</AppShell>
