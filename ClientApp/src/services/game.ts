export enum DisplayState {
    Connecting,
    Disconnected,
    Ready,
    FirstTimeSetup,
    LoggedIn,
    AdminDashboard,
    RegisteringCards,
    ResettingCards,
    TeamManagement
}

export enum CardType {
    Admin,
    Person,
    Team,
    Credits,
    SpecialReward,
    ProofOfTask
}

export interface Card {
    uid: string,
} 

export interface RegisteredCard extends Card {
    id: number,
    number?: string,
    type: CardType,
    redeemed: boolean,
    pin?: string,
    data?: any,
    enabled: boolean,
    user?: User
}

export interface User {
    id: number;
    team?: Team;
    leader: boolean;
}

export interface Team {
    id: number;
    name: string;
    colour?: string
    pin?: string;
    balance: number;
}

export function redeemable(type: CardType): boolean {
    return [
        CardType.Credits,
        CardType.ProofOfTask,
        CardType.SpecialReward
    ].includes(type);
}
