<script lang="ts">
    import { alert, card } from '../stores'
    import {CardType} from "../services/game";

    $: registered = $card && $card.id;
    $: unregistered = $card && !$card.id;
    $: enabled = registered && $card.enabled;
    $: disabled = registered && !$card.enabled;
    $: redeemed = enabled && $card.redeemed;
    $: unredeemed = enabled && $card.redeemed === false;
    
    $: className = cardClass($card);
    
    function cardClass(): string
    {
        if (!$card) {
            return 'foo';
        }
        if (unregistered) {
            return 'unregistered';
        }
        if (disabled) {
            return 'disabled';
        }
        if (redeemed) {
            return 'redeemed';
        }
        if (unredeemed) {
            return 'unredeemed';
        }
        return 'active';
    }

    function cardLabel(): string
    {
        if (unregistered) {
            return formatUid($card.uid);
        }
        
        let label;
        
        if ($card.type === CardType.Admin) {
            label = 'Admin Card';
        } else if ($card.type === CardType.Credits) {
            label = `${$card.data} Credits`;
        } else if ($card.type === CardType.Person) {
            label = `User ID #${$card.data}`;
        } else {
            label = CardType[$card.type];
        }
        
        return label;
    }
    
    function formatUid(uid: string): string
    {
        // A UID is in this format: 04-E6-3D-AB-10-02-89
        // Return a HTML string that looks like this: <span class='byte'>04</span><span class='dash'>-</span> and so on

        let elements = [];
        let bytes = uid.split('-');

        for (const byte of bytes) {
            elements.push(`<span class='byte'>${byte}</span>`);
        }

        return elements.join('<span class="dash">-</span>');
    }
</script>

<style lang="scss">
    .nfc-card {
        aspect-ratio: 86/54;

        :global(.dash) {
            @apply font-normal;
            @apply opacity-50;
        }
    }

    .active, .redeemed {
        @apply bg-success-500 text-on-success-token shadow-inner;

        box-shadow: 0 0 50px theme('colors.success.700');
    }

    .unregistered {
        @apply bg-warning-500 text-on-warning-token shadow-inner;

        box-shadow: 0 0 50px theme('colors.warning.700');
    }

    .disabled, .error {
        @apply bg-error-500 text-on-error-token shadow-inner;

        box-shadow: 0 0 50px theme('colors.error.700');
    }
    
    .unredeemed {
        @apply bg-tertiary-500 text-on-tertiary-token shadow-inner;

        box-shadow: 0 0 50px theme('colors.tertiary.700');
    }

    @keyframes pulse {
        50% {
            transform: scale(1.5);
        }
    }
    
    .alert-container {
        height: 30px;
    }
</style>

<div class="w-[300px] mx-auto">
    <div class='nfc-card card mx-auto font-mono transition flex flex-col items-center text-center relative {className} pb-2'
         class:error={$alert?.type === 'error'}
    >
        <div class="flex flex-col flex-grow justify-center items-center">
            {#if !$card}
                <div>Touch card to reader</div>
            {:else if disabled}
                <div class="font-bold text-2xl line-through opacity-50">{cardLabel()}</div>
                <div class="my-2 font-bold bg-error-900 text-white py-1 px-2">This card is inoperative.<br>Please return it to the box.</div>
            {:else if unregistered}
                <div class="font-bold text-2xl opacity-50">Unregistered card</div>
            {:else}
                <div class="font-bold text-2xl">{cardLabel()}</div>
            {/if}

            {#if redeemed}
                <div class="my-2 font-bold bg-success-900 text-white py-1 px-2">Card has been redeemed.<br>Please return it to the box.</div>
            {/if}
        </div>
        
        {#if $card}
            <div class="absolute left-2 bottom-2 text-xs">{@html formatUid($card.uid)}</div>
            {#if $card.number}
                <div class="font-medium absolute right-2 bottom-1">{$card.number}</div>
            {/if}
        {/if}
    </div>
</div>

<div class="alert-container text-center flex justify-center items-end relative z-10">
    {#if $alert}
        <div class="text-white">{$alert.message}</div>
    {/if}
</div>
