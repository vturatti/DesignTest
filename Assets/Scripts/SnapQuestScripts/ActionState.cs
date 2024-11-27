namespace Code.NPCs
{
    public enum ActionState
    {
        //DO NOT CHANGE WHAT EXISTS, ONLY ADD NEW ONES WITH UNIQUE NUMBERS!
        // Save states depend on these not changing
        NoneOrDefault = 0, //this is NOT the same as an ANY and is actually a specific state
        Standing = 1,
        Running = 2,
        Eating = 3,
        Drinking = 4,
        Playing = 5,
        Talking = 6,
        Thinking = 7,
        Smiling = 8,
        Sleeping = 9,
        Upset = 10,
        Curious = 11,
        Dancing = 12,
        Guarding = 13,
        Frightened = 14,
        Wandering = 15,
        PlaytimeIsOver = 16,
        FoodFight = 17,
        ShakeShakeShake = 18,
        CanIHelpYou = 19,
        ActingSketch = 20,
        AuNatural = 21,
        BigYawn = 22,
        PerformingTheRitual = 23,
        OmNomNom = 24,
        Posing = 25,
        FishedOut = 26,
        Evicted = 27,
        SoSad = 28,
        MatingRitual = 29,
        Working = 30,
        Attacking = 31,
        Flying = 32,
        Blooming = 33,
        Giggling = 34,
        Relaxing = 35,
        Singing = 36,
        Performing = 37,
        Alert = 38,
        Small = 39,
        GrownUp = 40,
        Medium = 41,
        ChargingUp = 42,
        Dizzy = 43,
        Hiding = 44,
        Seeking = 45,
        Calm = 46,
        Charging = 47,

        //Only one per character should be ANY, as it will look at all the other ones on the page and if there
        //is a picture we have taken that is NOT any of the other ones, it will use the first one of those.
        // This will never be the state that a photo has, but it may be assigned to the notebook picture spot.
        AnyNbOnly = 10000,
        FavoriteSlotNbOnly = 10001
    }
}