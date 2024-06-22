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
    ConfirmPurchase,
    TeamSummary,
    ScannerInfo,
    VoteResults,
    PollOptions
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
    shopItems: (ShopItem|null)[];
}

export enum ScannerState {
    Disabled = 1,
    Ready,
    InvalidCard,
    ReadyToSelect,
    OptionSelected,
}

export interface Scanner {
    cardUid: string|null;
    connectionId: string;
    ipAddress: string;
    state: ScannerState|null;
    stationId: number|null;
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
        return 'Unregistered Card';
    }

    let label: string;

    if (card.type === CardType.Admin) {
        label = 'Admin Card';
    } else if (card.type === CardType.Credits) {
        label = `${card.data} Credits`;
    } else if (card.type === CardType.Person) {
        label = `${card.user.name}`;
    } else if (card.type === CardType.SpecialReward) {
        label = 'Special Reward';
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
    SpecialReward,
    Minifig
}

export interface ShopItem
{
    id: number,
    name: string,
    type: ShopItemType,
    price: number,
    owner: Team|null,
    available: boolean,
    redeemed: boolean,
    rewardCard: Card|null,
}

export enum ItemStatus
{
    Purchasable,
    PurchasableTooExpensive,
    OwnedByYou,
    OwnedByOtherTeam,
    ReservedForYou,
    ReservedForOtherTeam,
    UnclaimedReward,
    FutureUnlock,
}

export function itemStatus(item: ShopItem, team: Team): ItemStatus
{
    if (item.redeemed) {
        if (item.owner?.id === team.id) {
            return ItemStatus.OwnedByYou;
        } else {
            return ItemStatus.OwnedByOtherTeam;
        }
    }
    
    if (item.rewardCard) {
        if (!item.owner) {
            return ItemStatus.UnclaimedReward;
        } else if (item.owner.id === team.id) {
            return ItemStatus.ReservedForYou;
        } else {
            return ItemStatus.ReservedForOtherTeam;
        }
    }
    
    if (!item.available) {
        return ItemStatus.FutureUnlock;
    }
    
    if (item.price > team.balance) {
        return ItemStatus.PurchasableTooExpensive;
    } else {
        return ItemStatus.Purchasable;
    }
}
