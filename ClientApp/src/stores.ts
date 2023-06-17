import { writable } from "svelte/store";

export const systemLog = writable([]);

export function addLog(message: string): void {
    systemLog.update(messages => {
        messages.push({
            date: new Date(),
            message
        });
        return messages;
    });
}

