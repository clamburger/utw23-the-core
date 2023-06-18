export enum DisplayState {
    Connecting,
    Disconnected,
    Ready,
    FirstTimeSetup,
    TeamDashboard,
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
    Reward,
    Special
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
    enabled: boolean
}
