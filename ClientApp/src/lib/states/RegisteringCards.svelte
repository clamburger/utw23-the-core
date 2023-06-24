<script lang="ts">
    import {type Card, CardType, DisplayState, type RegisteredCard} from "../../services/game";
    import {addLog, card, cardInserted, changeState, clearAlert, connection, showAlert} from "../../stores";
    import {onMount} from "svelte";
    import {RadioGroup, RadioItem, SlideToggle} from '@skeletonlabs/skeleton';
    import wretch from 'wretch';
    import firstBy from "thenby";

    let autoSubmit = true;
    let type = CardType.Admin;
    let label = '0x00';
    let credits = 100;
    
    let users = [];
    let selectedUser: number;
    
    onMount(() => {
        fetchUsers();
    });
    
    function fetchUsers() {
        wretch('/api/Admin/users')
            .get()
            .json(result => {
                users = result.sort(
                    firstBy((a) => a.team?.name)
                        .thenBy('leader', -1)
                        .thenBy('name')
                );
                if (selectedUser === undefined) {
                    selectedUser = users[0].id;
                } else {
                    var index = users.findIndex(u => u.id === selectedUser);
                    selectedUser = users[index + 1]?.id;
                }
            });
    }

    function performRegistration() {
        clearAlert();
        
        if (type === CardType.Admin) {
            $connection.invoke('RegisterAdminCard', $card.uid, label);
        } else if (type === CardType.Credits) {
            $connection.invoke('RegisterCreditsCard', $card.uid, credits, label);
        } else if (type === CardType.Person) {
            if (selectedUser === undefined) {
                showAlert("error", "No person selected.");
            } else {
                $connection.invoke('RegisterPersonCard', $card.uid, selectedUser);
            }
        } else {
            showAlert("error", "Unsupported card type selected.");
        }
    }
    
    function cardInsertedHandler(_card: Card) {
        clearAlert();

        if (autoSubmit) {
            performRegistration();
        }
    }
    
    function cardRegisteredHandler(_card: RegisteredCard) {
        cardInserted(_card);
        
        addLog(`${CardType[_card.type]} card registered [${_card.number || 'no label'}]`);
        
        if (_card.type === CardType.Person) {
            fetchUsers();
            return;
        }

        const matches = label.match(/^0x([0-9A-F]+)$/);
        if (!matches) {
            return;
        }

        // Convert from base 16 to base 10
        const number = parseInt(matches[1], 16);

        // Increment number, convert to base 16, and pad to same number of digits
        label = "0x" + (number + 1).toString(16).toUpperCase().padStart(matches[1].length, '0');
    }
    
    function selectUser(_user) {
        selectedUser = _user.id;
    }
    
    onMount(() => {
        $connection.on('CardInserted', cardInsertedHandler);
        $connection.on('CardRegistered', cardRegisteredHandler);

        return () => {
            $connection.off('CardInserted', cardInsertedHandler);
            $connection.off('CardRegistered', cardRegisteredHandler);
        }
    });
</script>

<RadioGroup>
    <RadioItem bind:group={type} name="cardType" value={CardType.Admin}>Admin</RadioItem>
    <RadioItem bind:group={type} name="cardType" value={CardType.Person}>Person</RadioItem>
<!--    <RadioItem bind:group={type} name="cardType" value={CardType.Team} disabled>Team</RadioItem>-->
    <RadioItem bind:group={type} name="cardType" value={CardType.Credits}>Credits</RadioItem>
<!--    <RadioItem bind:group={type} name="cardType" value={CardType.Reward} disabled>Reward</RadioItem>-->
<!--    <RadioItem bind:group={type} name="cardType" value={CardType.Special} disabled>Special</RadioItem>-->
</RadioGroup>

<div class="card w-1/2 overflow-hidden">
    <section class="flex gap-4" class:p-4={type !== CardType.Person}>
        {#if type === CardType.Person}
            <div class="table-container">
                <table class="table table-interactive table-compact">
                    <tbody>
                    {#each users as user}
                        <tr class:table-row-checked="{selectedUser === user.id}" on:click={() => selectUser(user)}>
                            <td>{user.name}</td>
                            <td>{user.leader ? 'Leader' : 'Camper'}</td>
                            {#if user.team}
                                <td style="background-color: {user.team.colour}" class="{['Helpers', 'Fixers'].includes(user.team.name) ? 'text-black' : ''}">
                                    {user.team.name}
                                </td>
                            {:else}
                                <td></td>
                            {/if}
                            <td>{user.cards.length} {user.cards.length === 1 ? 'card' : 'cards'}</td>
                        </tr>
                    {/each}
                    </tbody>
                </table>
            </div>
        {:else}
            <label class="label basis-2/6">
                <span>Label</span>
                <input class="input" type="text" bind:value={label}>
            </label>
        {/if}

        {#if type === CardType.Admin}
            <div class="basis-2/6"></div>
            <div class="basis-2/6"></div>
        {:else if type === CardType.Credits}
            <label class="label basis-2/6">
                <span>Credits Value</span>
                <input class="input" type="number" bind:value={credits}>
            </label>
            <div class="basis-2/6"></div>
        {/if}
    </section>
    <hr>
    <footer class="card-footer p-4 flex justify-between items-center">
        <button class="btn variant-filled-primary" disabled={autoSubmit || !$card} on:click={performRegistration}>
            {autoSubmit ? 'Auto-Submit On' : $card ? 'Submit Card' : 'No Card on Reader'}
        </button>
        <SlideToggle name="slider-label" active="bg-primary-500" bind:checked={autoSubmit} value>Submit automatically on scan</SlideToggle>
    </footer>
</div>

<div>
    <button class="btn variant-filled-secondary" on:click={() => changeState(DisplayState.AdminDashboard)}>Finished</button>
</div>
