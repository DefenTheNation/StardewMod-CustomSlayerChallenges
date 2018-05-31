
-------------------------------------
Custom Adventure Guild Challenges
-------------------------------------

Thank you for downloading the mod! With a little work, you can customize your guild goals and rewards as you see fit!
If you are unfamiliar with JSON files, check out a tutorial (google "JSON tutorial") before proceeding.

You can always use the VanillaConfig.json file as a reference or backup in case something goes wrong. This file has the vanilla challenges
in the mod's format.

Known Issues:
	- Non-vanilla challenges do not show the challenge complete message
	- Steam Achievement is activated when vanilla challenges are complete even if vanilla challenges are removed from config

To workaround these issues, leave the vanilla challenges in the config. You can still update their reward and display name,
but the monster types and the kill count must remain the same.


I. Config File Introduction

The config file is global, meaning all farms will share the same goals and rewards. The mod never writes back to this file,
so any changes you make are never updated or deleted.

"CustomChallengesEnabled"
Set to false if you want to keep the vanilla challenges. You do not have to modify the rest of the config if this is set to false. The mod will ignore it.
If you want to use the challenges listed in the config instead of the vanilla challenges, change 'false' to 'true' (without the single quotes).

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
		"FrostBat"
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
		"Spiker"
		"Cat"

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
        BigCraftable = 1 - unsure what these are
        MeleeWeapon = 2 - Weapons
        RegularObjectRecipe = 4 - unsure what these are
        BigCraftableRecipe = 5 - unsure what these are
        Hat = 6
        Ring = 7

Note: Item Type 3 is special items, but the game does not create these and they cannot be used as rewards.

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

Rings - 7:
		SlimeCharmerRing = 520,
        VampireRing = 522,
        SavageRing = 523,
        YobaRing = 524,
        SturdyRing = 525,
        BurglarsRing = 526,
        JukeboxRing = 528

Hats - 8:
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


IV. Final

If you have issues or would like help creating a custom list, reach out to me on Nexus Mods (DefenTheNation) and I
would be more than happy to help!
