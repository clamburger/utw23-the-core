import {type Writable, writable} from "svelte/store";
import type {Card, RegisteredCard} from "./services/game";
import {DisplayState} from "./services/game";
import {HubConnection} from "@microsoft/signalr";

export const systemLog = writable([]);
export const card: Writable<Card|RegisteredCard> = writable();
export const state = writable(DisplayState.Connecting);
export const connection: Writable<HubConnection> = writable();
export const alert: Writable<Alert> = writable();

export interface Alert {
    type: 'success' | 'warning' | 'error';
    message: string;
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

export function cardInserted(_card: Card): void {
    card.set(_card);
}

export function cardRemoved(): void {
    card.set(undefined);
}

export function changeState(_state: DisplayState): void {
    state.set(_state);
}

export function showAlert(type: 'success' | 'warning' | 'error', message: string): void {
    alert.set({type, message})
}

export function clearAlert(): void {
    alert.set(undefined);
}
