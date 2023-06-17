<script lang="ts">
    import { afterUpdate } from "svelte";
    import { systemLog } from "../stores"
    import dayjs from "dayjs";
    let element;

    const scrollToBottom = async (node: HTMLElement) => {
        node.scroll({ top: node.scrollHeight, behavior: "smooth" });
    };
    
    afterUpdate(() => {
        scrollToBottom(element);
    });
</script>

<style lang="scss">
    .log {
        height: 210px;
        width: 1000px;
        overflow-y: scroll;
    }
</style>

<div class='text-left shadow bg-neutral-900/90 rounded-container-token p-4'>
    <div class='text-sm border-b border-b-neutral-500 mb-2 pb-1' style='font-variant-caps: small-caps;'>
        System Log
    </div>
    <div class='font-mono log pr-4' bind:this={element}>
        {#each $systemLog as {date, message}}
            <div>
                <span class="date text-surface-500">{dayjs(date).format('HH:mm:ss')}</span>
                <span class="message">{message}</span>
            </div>
        {/each}
    </div>
</div>
