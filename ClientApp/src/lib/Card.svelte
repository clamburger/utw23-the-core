<style lang="scss">
    .nfc-card {
        aspect-ratio: 86/54;
        max-width: 300px;

        :global(.dash) {
            @apply font-normal;
            @apply opacity-50;
        }
    }

    .active {
        @apply bg-success-500 text-on-success-token font-bold shadow-inner;

        box-shadow: 0 0 50px theme('colors.success.700');
    }

    .invalid {
        @apply bg-error-500 text-on-error-token font-bold shadow-inner;

        box-shadow: 0 0 50px theme('colors.error.700');
    }

    .dash {
        @apply font-thin;
    }


	@keyframes pulse {
		50% {
			transform: scale(1.5);
		}
	}
</style>

<script lang="ts">
    export let card;
    export let uid: string;
    export let invalid: boolean = false;

    function formatUid(uid: string): string
    {
        if (!uid) {
            return '';
        }

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

<div class='nfc-card card flex flex-col justify-center items-center mx-auto font-mono transition' class:active="{uid && !invalid}" class:invalid="{uid && invalid}">
    {#if invalid}
        <div>Unregistered card</div>
    {/if}
    <div>{@html uid ? formatUid(uid) : 'Touch card to reader'}</div>
</div>
