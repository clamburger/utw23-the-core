export var DisplayState;
(function (DisplayState) {
    DisplayState[DisplayState["Connecting"] = 0] = "Connecting";
    DisplayState[DisplayState["Disconnected"] = 1] = "Disconnected";
    DisplayState[DisplayState["Ready"] = 2] = "Ready";
    DisplayState[DisplayState["FirstTimeSetup"] = 3] = "FirstTimeSetup";
    DisplayState[DisplayState["LoggedIn"] = 4] = "LoggedIn";
    DisplayState[DisplayState["AdminDashboard"] = 5] = "AdminDashboard";
    DisplayState[DisplayState["RegisteringCards"] = 6] = "RegisteringCards";
    DisplayState[DisplayState["ResettingCards"] = 7] = "ResettingCards";
    DisplayState[DisplayState["TeamManagement"] = 8] = "TeamManagement";
    DisplayState[DisplayState["Shop"] = 9] = "Shop";
    DisplayState[DisplayState["ShopManagement"] = 10] = "ShopManagement";
    DisplayState[DisplayState["ConfirmPurchase"] = 11] = "ConfirmPurchase";
    DisplayState[DisplayState["TeamSummary"] = 12] = "TeamSummary";
    DisplayState[DisplayState["ScannerInfo"] = 13] = "ScannerInfo";
    DisplayState[DisplayState["VoteResults"] = 14] = "VoteResults";
})(DisplayState || (DisplayState = {}));
export var CardType;
(function (CardType) {
    CardType[CardType["Admin"] = 0] = "Admin";
    CardType[CardType["Person"] = 1] = "Person";
    CardType[CardType["Team"] = 2] = "Team";
    CardType[CardType["Credits"] = 3] = "Credits";
    CardType[CardType["SpecialReward"] = 4] = "SpecialReward";
    CardType[CardType["ProofOfTask"] = 5] = "ProofOfTask";
})(CardType || (CardType = {}));
export var ScannerState;
(function (ScannerState) {
    ScannerState[ScannerState["Disabled"] = 1] = "Disabled";
    ScannerState[ScannerState["Ready"] = 2] = "Ready";
    ScannerState[ScannerState["InvalidCard"] = 3] = "InvalidCard";
    ScannerState[ScannerState["ReadyToSelect"] = 4] = "ReadyToSelect";
    ScannerState[ScannerState["OptionSelected"] = 5] = "OptionSelected";
})(ScannerState || (ScannerState = {}));
export function redeemable(type) {
    return [
        CardType.Credits,
        CardType.ProofOfTask,
        CardType.SpecialReward
    ].includes(type);
}
export function label(card) {
    if (!card.id) {
        return 'Unregistered Card';
    }
    let label;
    if (card.type === CardType.Admin) {
        label = 'Admin Card';
    }
    else if (card.type === CardType.Credits) {
        label = `${card.data} Credits`;
    }
    else if (card.type === CardType.Person) {
        label = `${card.user.name}`;
    }
    else if (card.type === CardType.SpecialReward) {
        label = 'Special Reward';
    }
    else {
        label = CardType[card.type];
    }
    return label;
}
export function inShop(state) {
    return [DisplayState.Shop, DisplayState.ConfirmPurchase].includes(state);
}
export var ShopItemType;
(function (ShopItemType) {
    ShopItemType[ShopItemType["StandardLego"] = 0] = "StandardLego";
    ShopItemType[ShopItemType["SpecialLego"] = 1] = "SpecialLego";
    ShopItemType[ShopItemType["SpecialReward"] = 2] = "SpecialReward";
    ShopItemType[ShopItemType["Minifig"] = 3] = "Minifig";
})(ShopItemType || (ShopItemType = {}));
export var ItemStatus;
(function (ItemStatus) {
    ItemStatus[ItemStatus["Purchasable"] = 0] = "Purchasable";
    ItemStatus[ItemStatus["PurchasableTooExpensive"] = 1] = "PurchasableTooExpensive";
    ItemStatus[ItemStatus["OwnedByYou"] = 2] = "OwnedByYou";
    ItemStatus[ItemStatus["OwnedByOtherTeam"] = 3] = "OwnedByOtherTeam";
    ItemStatus[ItemStatus["ReservedForYou"] = 4] = "ReservedForYou";
    ItemStatus[ItemStatus["ReservedForOtherTeam"] = 5] = "ReservedForOtherTeam";
    ItemStatus[ItemStatus["UnclaimedReward"] = 6] = "UnclaimedReward";
    ItemStatus[ItemStatus["FutureUnlock"] = 7] = "FutureUnlock";
})(ItemStatus || (ItemStatus = {}));
export function itemStatus(item, team) {
    if (item.redeemed) {
        if (item.owner?.id === team.id) {
            return ItemStatus.OwnedByYou;
        }
        else {
            return ItemStatus.OwnedByOtherTeam;
        }
    }
    if (item.rewardCard) {
        if (!item.owner) {
            return ItemStatus.UnclaimedReward;
        }
        else if (item.owner.id === team.id) {
            return ItemStatus.ReservedForYou;
        }
        else {
            return ItemStatus.ReservedForOtherTeam;
        }
    }
    if (!item.available) {
        return ItemStatus.FutureUnlock;
    }
    if (item.price > team.balance) {
        return ItemStatus.PurchasableTooExpensive;
    }
    else {
        return ItemStatus.Purchasable;
    }
}
//# sourceMappingURL=game.js.map