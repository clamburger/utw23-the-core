export var DisplayState;
(function (DisplayState) {
    DisplayState[DisplayState["Connecting"] = 0] = "Connecting";
    DisplayState[DisplayState["Disconnected"] = 1] = "Disconnected";
    DisplayState[DisplayState["Ready"] = 2] = "Ready";
    DisplayState[DisplayState["FirstTimeSetup"] = 3] = "FirstTimeSetup";
    DisplayState[DisplayState["TeamDashboard"] = 4] = "TeamDashboard";
    DisplayState[DisplayState["AdminDashboard"] = 5] = "AdminDashboard";
    DisplayState[DisplayState["RegisteringCards"] = 6] = "RegisteringCards";
    DisplayState[DisplayState["ResettingCards"] = 7] = "ResettingCards";
})(DisplayState || (DisplayState = {}));
export var CardType;
(function (CardType) {
    CardType[CardType["Admin"] = 0] = "Admin";
    CardType[CardType["Team"] = 1] = "Team";
    CardType[CardType["Credits"] = 2] = "Credits";
    CardType[CardType["Reward"] = 3] = "Reward";
    CardType[CardType["Special"] = 4] = "Special";
})(CardType || (CardType = {}));
//# sourceMappingURL=game.js.map