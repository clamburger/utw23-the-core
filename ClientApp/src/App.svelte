<script lang="ts">
  import MainDisplay from './lib/MainDisplay.svelte';
  import { AppShell } from '@skeletonlabs/skeleton';
  import SystemLog from "./lib/SystemLog.svelte";
  import CardReader from "./lib/CardReader.svelte";
  import Logo from "./lib/svg/Logo.svelte";
  import CardEmulator from "./lib/CardEmulator.svelte";
  import {DisplayState, inShop} from "./services/game";
  import {state} from "./stores";
  import LogoShop from "./lib/svg/LogoShop.svelte";
</script>

<style lang="scss">
    :global(.SystemLog .log) {
        height: 60px;
    }
    
    .logo {
        max-width: 400px;
    }
    
    :global(.footer > *) {
        flex-grow: 1;
    }
</style>

<AppShell>
    <!-- <div class="logo mx-auto my-4">
        {#if inShop($state)}
            <LogoShop />
        {:else}
            <Logo />
        {/if}
    </div> -->

    <div class="w-full h-full flex flex-col justify-center">
        <div class="p-4">
            {#if $state !== DisplayState.Shop}
                <CardReader />
            {/if}
        </div>
        
        <div class="container mx-auto flex justify-center px-8 pb-2 pt-0">
            <MainDisplay />
        </div>
    </div>

    <!-- <svelte:fragment slot="footer"> -->
        <!-- <div class="flex footer"> -->
            <!-- <SystemLog /> -->
            <!-- <CardEmulator /> -->
        <!-- </div> -->
    <!-- </svelte:fragment> -->
</AppShell>
