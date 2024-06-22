import {type Writable, writable} from "svelte/store";
import type {Card, RegisteredCard, ShopItem, Team, User} from "./services/game";
import {DisplayState, ShopItemType} from "./services/game";
import {HubConnection} from "@microsoft/signalr";


export const systemLog = writable([]);
export const card: Writable<Card|RegisteredCard> = writable();
export const state = writable(DisplayState.Connecting);
export const connection: Writable<HubConnection> = writable();
export const alert: Writable<Alert> = writable();
export const user: Writable<User> = writable();
export const team: Writable<Team> = writable();
export const shopItem: Writable<ShopItem> = writable();
export const showVoters: Writable<boolean> = writable(false);

export interface Alert {
    type: 'success' | 'warning' | 'error' | 'info';
    message: string;
    code?: string;
}

export function addLog(message: string): void {
    systemLog.update(messages => {
        messages.unshift({
            date: new Date(),
            message
        });
        return messages;
    });
}

export function updateCard(_card: Card): void {
    card.set(_card);
}

export function cardRemoved(): void {
    card.set(undefined);
}

export function changeState(_state: DisplayState): void {
    state.set(_state);
}

export function showAlert(type: 'success' | 'warning' | 'error' | 'info', message: string, code?: string): void {
    alert.set({type, message, code})
}

export function clearAlert(): void {
    alert.set(undefined);
}

export function updateTeam(_team: Team): void {
    team.set(_team);
}

export function updateUser(_user: User): void {
    user.set(_user);
    if (_user.team) {
        updateTeam(_user.team);
    }
}

export function updateShopItem(_item: ShopItem): void {
    shopItem.set(_item);
}

export function setShowVoters(value: boolean): void {
    showVoters.set(value);
}

export function signOut(): void {
    changeState(DisplayState.Ready);
    user.set(undefined);
    team.set(undefined);
}
