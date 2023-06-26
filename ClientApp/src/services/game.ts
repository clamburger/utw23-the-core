export enum DisplayState {
    Connecting,
    Disconnected,
    Ready,
    FirstTimeSetup,
    LoggedIn,
    AdminDashboard,
    RegisteringCards,
    ResettingCards,
    TeamManagement,
    Shop,
    ShopManagement,
    ConfirmPurchase
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
    name: string;
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

export function label(card: Card & RegisteredCard): string
{
    if (!card.id) {
        return 'Unregistered Card.';
    }

    let label: string;

    if (card.type === CardType.Admin) {
        label = 'Admin Card';
    } else if (card.type === CardType.Credits) {
        label = `${card.data} Credits`;
    } else if (card.type === CardType.Person) {
        label = `${card.user.name}`;
    } else {
        label = CardType[card.type];
    }

    return label;
}

export function inShop(state: DisplayState): boolean
{
    return [DisplayState.Shop, DisplayState.ConfirmPurchase].includes(state);
}

export enum ShopItemType
{
    StandardLego,
    SpecialLego,
    SpecialReward
}

export interface ShopItem
{
    id: number,
    name: string,
    type: ShopItemType,
    price: number,
    owner: Team|null,
    available: boolean,
}
