
----------------------------------------------------------------------------------
Custom Adventure Guild Challenges
----------------------------------------------------------------------------------

QUICK START

The mod comes with custom challenges and rewards, no additional setup is required! The rest of the README is for those
who want to customize the challenges and rewards and how to do so.

To change back to vanilla challenges, change the following line in the config.json file:

"CustomChallengesEnabled": true

	- TO -

"CustomChallengesEnabled": false

Your save data will remain the same so if you switch back to custom challenges or play with the config file, you can pick up right where you left off!


INTRODUCTION

Thank you for downloading the mod! With a little work, you can customize your guild goals and rewards as you see fit!
If you are unfamiliar with JSON files, check out a tutorial (google "JSON tutorial") before proceeding.

You can always use the VanillaConfig.json file as a reference or backup in case something goes wrong. This file has the vanilla challenges
in the mod's format.

Known Issues:
	- Non-vanilla challenges do not show the challenge complete message
	- Steam Achievement is activated when vanilla challenges are complete even if vanilla challenges are removed from config

To workaround these issues, leave the vanilla challenges in the config. You can still update their reward and display name,
but the monster types and the kill count must remain the same.


I. CONFIG FILE INTRODUCTION

The config file is global, meaning all farms will share the same goals and rewards. The mod never writes back to this file,
so any changes you make are never updated or deleted.

"CustomChallengesEnabled"
Set to false if you want to keep the vanilla challenges. You do not have to modify the rest of the config if this is set to false. The mod will ignore it.
If you want to use the challenges listed in the config instead of the vanilla challenges, change 'false' to 'true' (without the single quotes).

"CountKillsOnFarm"
By default, kills on the farm do not count toward the challenge goals. This mod enables kills for Wilderness Golems on farms because that's the only place
they show up. If you want all other monster kills to count toward the goals too, change 'false' to 'true' (without the single quotes).

"GilNoRewardDialogue"
This is what Gil says first time you talk to him in the guild and he has no rewards for you. You can make him as nice or as mean as you want.

"GilSleepingDialogue"
This is what Gil says after the first time you talk to him. Normally he snores, but now you decide what he says.

"GilSpecialGiftDialogue"
This is what Gil says when you get a special reward (Stardrop or any special wallet item)

"Challenges" - This is an array of challenges, meaning there are multiple challenges in this section.
Here are the fields for a single challenge:

	  "ChallengeName" - Display name of the challenge (eg. Slimes, Duggies, Bats, etc.)
      "RequiredKillCount" - How many monsters you need to kill to claim the reward
      "RewardType" - There are different types of rewards: Rings, Hats, Weapons, and even Recipes (see section III for available RewardTypes)
      "RewardItemNumber" - Each item has an in-game number. Find that number in section III
	  "MonsterNames" - These are the monsters that count toward the Required Kill Count. The Slimes challenge counts Green Slimes, Sludges (red slimes), and Frost Jellies toward the total.
		See section II for monster names


II. Monster Information

The following values are valid for MonsterNames:

		"Green Slime"
		"Frost Jelly"
		"Sludge"
		"Big Slime"

		"Bug"
		"Grub"
		"Fly"

		"Skeleton"
		"Skeleton Mage"
		"Skeleton Warrior"
        
		"Bat"
		"Frost Bat"
		"Lava Bat"
		"Iridium Bat"

		"Rock Crab"
		"Lava Crab"
		"Iridium Crab"

		"Shadow Guy"
		"Shadow Brute"
		"Shadow Shaman"

		"Ghost"
		"Carbon Ghost"

		"Stone Golem"
		"Wilderness Golem"

		"Duggy"
		"Dust Spirit"

		"Mummy"
		"Serpent"

		"Metal Head"
		"Squid Kid"

		"Spiker"	NOTE: This is an unused monster
		"Cat"		NOTE: This is an unused monster

Example: I want the Slimes challenge to include Green, Frost, and Red Slimes (a.k.a. Sludge), and Big Slimes.
Result:
	"MonsterNames": [
			"Green Slime",
			"Frost Jelly",
			"Sludge",
			"Big Slime"
		  ]


III. Reward Information

Rewards are broken into two numbers: the Reward Item Type and the Reward Item Number. To create a reward, both these numbers
must correspond to an in game item. Select a Reward Item Type using the below table and look up the Reward Item Number from
the corresponding item table below.

Example: I want the reward for Slimes challenge to be a Slime Charmer Ring.
My Reward Item Type is 7 for Ring with my Reward Item Number being 520 for Slime Charmer Ring.

Reward Item Types:
		Regular = 0 - Regular items like common materials. Reward will only output a single stack
			Note: You will have to manually research the item number as that is not included in this file
        BigCraftable = 1 - kegs, bee hives, crystalariums, etc.
        MeleeWeapon = 2 - Weapons
		SpecialItems = 3 - Special Items that end up in the farmer's wallet
        RegularObjectRecipe = 4 - Blueprints for items of item type 0. Use the same id to get the blueprint for it
        BigCraftableRecipe = 5 - Blueprints for items of item type 1. Use the same id to get the blueprint for it
        Hat = 6
        Ring = 7
		Boots = 8

MeleeWeapons - 2:
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

Special - 3:
		ClubCard = 2,
        SpecialCharm = 3,
        SkullKey = 4,
        MagnifyingGlass = 5,
        DarkTalisman = 6,
        MagicInk = 7,
        LargePack = 99		//NOTE: I don't know exactly how this works. It does not seem to affect
							//		the inventory size.

Rings - 6:
		SmallGlowRing = 516,
        GlowRing = 517,
        SmallMagnetRing = 518,
        MagnetRing = 519,
        SlimeCharmerRing = 520,
        VampireRing = 522,
        SavageRing = 523,
        YobaRing = 524,
        SturdyRing = 525,
        BurglarsRing = 526,
        IridiumBand = 527,
        JukeboxRing = 528,
        AmethystRing = 529,
        TopazRing = 530,
        AquamarineRing = 531,
        JadeRing = 532,
        EmeraldRing = 533,
        RubyRing = 534

Hats - 7:
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

Boots - 8:
		Sneakers = 504,
        RubberBoots = 505,
        LeatherBoots = 506,
        WorkBoots = 507,
        CombatBoots = 508,
        TundraBoots = 509,
        ThermalBoots = 510,
        DarkBoots = 511,
        FirewalkerBoots = 512,
        GenieShoes = 513,
        SpaceBoots = 514,
        CowboyBoots = 515

BigCraftable - 1:
        HousePlant1 = 0,
        HousePlant2 = 1,
        HousePlant3 = 2,
        HousePlant4 = 3,
        HousePlant5 = 4,
        HousePlant6 = 5,
        HousePlant7 = 6,
        HousePlant8 = 7,
        Scarecrow = 8,
        LightningRod = 9,
        BeeHouse = 10,
        Keg = 12,
        Furnace = 13,
        PreservesJar = 15,
        CheesePress = 16,
        Loom = 17,
        OilMaker = 19,
        RecyclingMachine = 20,
        Crystalarium = 21,
        TablePieceLeft = 22,
        TablePieceRight = 23,
        MayonnaiseMachine = 24,
        SeedMaker = 25,
        WoodChair1 = 26,
        WoodChair2 = 27,
        SkeletonModel = 28,
        Obelisk = 29,
        ChickenStatue = 31,
        StoneCairn = 32,
        SuitOfArmor = 33,
        SignOfTheVessel = 34,
        BasicLog = 35,
        LawnFlamingo = 36,
        WoodSign = 37,
        StoneSign = 38,
        BigGreenCane = 40,
        GreenCanes = 41,
        MixedCane = 42,
        RedCanes = 43,
        BigRedCane = 44,
        OrnamentalHayBale = 45,
        LogSection = 46,
        GraveStone = 47,
        SeasonalDecor = 48,
        StoneFrog = 52,
        StoneParrot = 53,
        StoneOwl1 = 54,
        StoneJunimo = 55,
        SlimeBall = 56,
        GardenPot = 62,
        Bookcase = 64,
        FancyTable = 65,
        AncientTable = 66,
        AncientStool = 67,
        GrandfatherClock = 68,
        TeddyTimer = 69,
        DeadTree = 70,
        Staircase = 71,
        TallTorch = 72,
        RitualMask = 73,
        Bonfire = 74,
        Bongo = 75,
        DecorativeSpears = 76,
        Boulder = 78,
        Door1 = 79,
        Door2 = 80,
        LockedDoor1 = 81,
        LockedDoor2 = 82,
        WickedStatue1 = 83,
        WickedStatue2 = 84,
        SlothSkeletonLeft = 85,
        SlothSkeletonMiddle = 86,
        SlothSkeletonRight = 87,
        StandingGeode = 88,
        ObsidianVase = 89,
        CrystalChair = 90,
        SingingStone = 94,
        StoneOwl2 = 95,
        StrangeCapsule = 96,
        EmptyCapsule = 98,
        FeedHopper = 99,
        Incubator = 101,
        Heater = 104,
        Tapper = 105,
        Camera = 106,
        PlushBunny = 107,
        TubOFlowers1 = 108,
        TubOFlowers2 = 109,
        Rarecrow1 = 110,
        DecorativePitcher = 111,
        DriedSunflowerSeeds = 112,
        Rarecrow2 = 113,
        CharcoalKiln = 114,
        StardewHeroTrophy = 116,
        SodaMachine = 117,
        Barrel1 = 118,
        Crate1 = 119,
        Barrel2 = 120,
        Crate2 = 121,
        Barrel3 = 122,
        Crate3 = 123,
        Barrel4 = 124,
        Crate4 = 125,
        Rarecrow3 = 126,
        StatueOfEndlessFortune = 127,
        MushroomBox = 128,
        Chest = 130,
        Rarecrow4 = 136,
        Rarecrow5 = 137,
        Rarecrow6 = 138,
        Rarecrow7 = 139,
        Rarecrow8 = 140,
        PraireKingArcadeSystem = 141,
        WoodenBrazier = 143,
        StoneBrazier = 144,
        GoldBrazier = 145,
        Campfire = 146,
        StumpBrazier = 147,
        CarvedBrazier = 148,
        SkullBrazier = 149,
        BarrelBrazier = 150,
        MarbleBrazier = 151,
        WoodLamppost = 152,
        IronLamppost = 153,
        WormBin = 154,
        HMTGF = 155, // Listed as ??HMTGF??
        SlimeIncubator = 156,
        SlimeEggPress = 158,
        JunimoKartArcadeSystem = 159,
        StatueOfPerfection = 160,
        PinkyLemon = 161, // Listed as ??PinkyLemon??
        Foroguemon = 162, // Listed as ??Foroguemon??
        Cask = 163,
        SolidGoldLewis = 164,
        AutoGrabber = 165,
        SeasonalPlant1 = 184,
        SeasonalPlant2 = 188,
        SeasonalPlant3 = 192,
        SeasonalPlant4 = 196,
        SeasonalPlant5 = 200,
        SeasonalPlant6 = 204

IV. Final

If you have issues or would like help creating a custom list, reach out to me on Nexus Mods (DefenTheNation) and I
would be more than happy to help!
