import { writable } from "svelte/store";
export const systemLog = writable([]);
export function addLog(message) {
    systemLog.update(messages => {
        messages.push({
            date: new Date(),
            message
        });
        return messages;
    });
}
//# sourceMappingURL=stores.js.map