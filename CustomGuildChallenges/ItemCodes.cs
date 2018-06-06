namespace CustomGuildChallenges
{
    // Corresponds to ObjectFactory byte constants
    // Hat and Ring types added by this mod
    public enum ItemType
    {
        Regular = 0,                // Note: Stack count is hardcoded to 1
        BigCraftable = 1,
        MeleeWeapon = 2,
        SpecialItem = 3,            // Note: does not created in object factory
        RegularObjectRecipe = 4,
        BigCraftableRecipe = 5,
        Hat = 6,
        Ring = 7
    }

    // Item Type 7
    public enum Rings
    {
        SlimeCharmerRing = 520,
        VampireRing = 522,
        SavageRing = 523,
        YobaRing = 524,
        SturdyRing = 525,
        BurglarsRing = 526,
        IridiumBand = 527,
        JukeboxRing = 528
    }

    // Item Type 6
    public enum Hats
    {
        CowboyHat = 0,
        BowlerHat = 1,
        TopHat = 2,
        Sombrero = 3,
        StrawHat = 4,
        OfficialCap = 5,
        BlueBonnet = 6,
        PlumChapeau = 7,
        SkeletonMask = 8,
        GoblinMask = 9,
        ChickenMask = 10,
        Earmuffs = 11,
        DelicateBow = 12,
        Tropiclip = 13,
        ButterflyBow = 14,
        HuntersCap = 15,
        TruckerHast = 16,
        SailorsCap = 17,
        GoodOlCap = 18,
        Fedora = 19,
        CoolCap = 20,
        LuckyBow = 21,
        PolkaBow = 22,
        GnomesCap = 23,
        EyePatch = 24,
        SantaHat = 25,
        Tiara = 26,
        HardHat = 27,
        Souwester = 28,
        Daisy = 29,
        WatermelonBand = 30,
        MouseEars = 31,
        CatEars = 32,
        CowgalHat = 33,
        CowpokeHat = 34,
        ArchersCap = 35,
        PandaHat = 36,
        BlueCowboyHat = 37,
        RedCowboyHat = 38,
        ConeHat = 39
    }

    // Item Type 2
    public enum MeleeWeapons
    {
        RustySword = 0,
        SilverSaber = 1,
        DarkSword = 2,
        HolyBlade = 3,
        GalaxySword = 4,
        BoneSword = 5,
        IronEdge = 6,
        TemplarsBlade = 7,
        ObsidianEdge = 8,
        LavaKatana = 9,
        Claymore = 10,
        SteelSmallsword = 11,
        WoodenBlade = 12,
        InsectHead = 13,
        NeptunesGlaive = 14,
        ForestSword = 15,
        CarvingKnife = 16,
        IronDirk = 17,
        BurglarsShank = 18,
        ShadowDagger = 19,
        ElfBlade = 20,
        CrystalDagger = 21,
        WindSpire = 22,
        GalaxyDagger = 23,
        WoodClub = 24,
        AlexsBat = 25,
        LeadRod = 26,
        WoodMallet = 27,
        TheSlammer = 28,
        GalaxyHammer = 29,
        SamsOldGuitar = 30,
        Femur = 31,
        Slingshot = 32,
        MasterSlingshot = 33,
        GalaxySlingshot = 34,
        ElliottsPencil = 35,
        MarusWrench = 36,
        HarveysMallet = 37,
        PennysFryer = 38,
        LeahsWhittler = 39,
        AbbysPlanchette = 40,
        SebsLostMace = 41,
        HaleysIron = 42,
        PiratesSword = 43,
        Cutlass = 44,
        WickedKris = 45,
        Kudgel = 46,
        Scythe = 47,
        YetiTooth = 48,
        Rapier = 49,
        SteelFalchion = 50,
        BrokenTrident = 51,
        TemperedBroadsword = 52
    }
}
