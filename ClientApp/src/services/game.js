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
export function redeemable(type) {
    return [
        CardType.Credits,
        CardType.ProofOfTask,
        CardType.SpecialReward
    ].includes(type);
}
//# sourceMappingURL=game.js.map