<script lang="ts">
  import Messenger from './lib/Messenger.svelte';
  import { AppShell } from '@skeletonlabs/skeleton';
</script>

<AppShell>
  <div class="container h-full mx-auto flex justify-center items-center">
    <div class="text-center flex flex-col items-center">
        <Messenger />  
    </div>
  </div>
</AppShell>
