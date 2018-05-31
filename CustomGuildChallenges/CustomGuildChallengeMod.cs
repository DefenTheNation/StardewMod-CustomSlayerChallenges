using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomGuildChallenges
{
    /// Mod entry - handles all events and linking custom objects to game state
    public class CustomGuildChallengeMod : Mod
    {
        protected IModHelper modHelper;
        protected AdventureGuild adventureGuild;
        protected CustomAdventureGuild customAdventureGuild;

        public ModConfig Config { get; set; }

        public override void Entry(IModHelper helper)
        {
            Monitor.Log("Starting entry...");

            modHelper = helper;
            Config = helper.ReadConfig<ModConfig>();

            // Create config file using vanilla challenges
            if(Config == null || Config.Challenges == null || Config.Challenges.Count == 0)
            {
                Config = new ModConfig()
                {
                    CustomChallengesEnabled = false,
                    Challenges = GetVanillaSlayerChallenges().ToList()
                };

                Monitor.Log("Config has " + Config.Challenges.Count + " challenges!");

                helper.WriteConfig(Config);
            }
            // Use vanilla challenges but do not overwrite the config
            else if(!Config.CustomChallengesEnabled)
            {
                Config.Challenges = GetVanillaSlayerChallenges().ToList();
            }
            
            Monitor.Log("Config created");

            adventureGuild = new AdventureGuild(CustomAdventureGuild.MapPath, CustomAdventureGuild.MapName);
            customAdventureGuild = new CustomAdventureGuild(Config.Challenges);

            customAdventureGuild.GilNoRewardsText = Game1.content.LoadString("Characters\\Dialogue\\Gil:ComeBackLater");
            customAdventureGuild.GilNappingText = Game1.content.LoadString("Characters\\Dialogue\\Gil:Snoring");

            customAdventureGuild.GilNoRewardsText = "I ain't got any freebies anymore, ya damn hippie!";
            customAdventureGuild.GilNappingText = "I'm tryin to get some sleep, ya damn hippie! *grumble*";

            Monitor.Log("Custom guild created");

            SaveEvents.BeforeSave += presaveData;
            SaveEvents.AfterLoad += injectGuild;
            SaveEvents.AfterCreate += injectGuild;
            SaveEvents.AfterSave += injectGuild;

            modHelper.ConsoleCommands.Add("player_setkills", "", (command, arguments) =>
            {
                Game1.stats.specificMonstersKilled[arguments[0]] = int.Parse(arguments[1]);
            });

            string log = Config.CustomChallengesEnabled ?
                "Initialized (" + Config.Challenges.Count + " custom challenges uploaded)" :
                "Initialized (Vanilla challenges loaded)";

            Monitor.Log(log, LogLevel.Info);
        }

        void injectGuild(object sender, EventArgs e)
        {
            string saveDataPath = Path.Combine("saveData", Constants.SaveFolderName + ".json");
            var saveData = modHelper.ReadJsonFile<SaveData>(saveDataPath) ?? new SaveData();

            Monitor.Log("Read " + saveData.Challenges.Count + " challenges from save file");

            foreach (var savedChallenge in saveData.Challenges)
            {
                foreach (var slayerChallenge in customAdventureGuild.ChallengeList)
                {
                    if (savedChallenge.ChallengeName == slayerChallenge.Info.ChallengeName)
                    {
                        slayerChallenge.CollectedReward = savedChallenge.Collected;
                        break;
                    }
                }
            }

            // Kill old guild, replace with new guild
            for (int i = 0; i < Game1.locations.Count; i++)
            {
                if (Game1.locations[i].Name == CustomAdventureGuild.MapName)
                {
                    Game1.locations.RemoveAt(i);
                    Game1.locations.Add(customAdventureGuild);

                    Monitor.Log("CUSTOM GUILD INJECTED!!");
                }
            }
        }

        void presaveData(object sender, EventArgs e)
        {
            string saveDataPath = Path.Combine("saveData", Constants.SaveFolderName + ".json");
            var saveData = new SaveData();

            foreach (var slayerChallenge in customAdventureGuild.ChallengeList)
            {
                var save = new ChallengeSave()
                {
                    ChallengeName = slayerChallenge.Info.ChallengeName,
                    Collected = slayerChallenge.CollectedReward
                };

                saveData.Challenges.Add(save);
            }

            modHelper.WriteJsonFile(saveDataPath, saveData);
            Monitor.Log("Saved " + customAdventureGuild.ChallengeList.Count + " challenges.");

            for (int i = 0; i < Game1.locations.Count; i++)
            {
                if (Game1.locations[i].Name == CustomAdventureGuild.MapName)
                {
                    Game1.locations.RemoveAt(i);
                    Game1.locations.Add(adventureGuild);
                }
            }
        }

        public static IList<ChallengeInfo> GetVanillaSlayerChallenges()
        {
            var slimeChallenge = new ChallengeInfo()
            {
                ChallengeName = "Slimes",
                RequiredKillCount = 1000,
                MonsterNames = { Monsters.GreenSlime, Monsters.FrostJelly, Monsters.Sludge },
                RewardType = ItemType.Ring,
                RewardItemNumber = (int)Rings.SlimeCharmerRing
            };

            var shadowChallenge = new ChallengeInfo()
            {
                ChallengeName = "Void Spirits",
                RequiredKillCount = 150,
                MonsterNames = { Monsters.ShadowGuy, Monsters.ShadowShaman, Monsters.ShadowBrute },
                RewardType = ItemType.Ring,
                RewardItemNumber = (int)Rings.SavageRing
            };

            var skeletonChallenge = new ChallengeInfo()
            {
                ChallengeName = "Skeletons",
                RequiredKillCount = 50,
                MonsterNames = { Monsters.Skeleton, Monsters.SkeletonMage, Monsters.SkeletonWarrior },
                RewardType = ItemType.Hat,
                RewardItemNumber = (int)Hats.SkeletonMask
            };

            var caveInsectsChallenge = new ChallengeInfo()
            {
                ChallengeName = "Cave Insects",
                RequiredKillCount = 125,
                MonsterNames = { Monsters.Bug, Monsters.Grub, Monsters.Fly },
                RewardType = ItemType.MeleeWeapon,
                RewardItemNumber = (int)MeleeWeapons.InsectHead
            };

            var duggyChallenge = new ChallengeInfo()
            {
                ChallengeName = "Duggies",
                RequiredKillCount = 30,
                MonsterNames = { Monsters.Duggy },
                RewardType = ItemType.Hat,
                RewardItemNumber = (int)Hats.HardHat
            };

            var batChallenge = new ChallengeInfo()
            {
                ChallengeName = "Bats",
                RequiredKillCount = 100,
                MonsterNames = { Monsters.Bat, Monsters.FrostBat, Monsters.LavaBat },
                RewardType = ItemType.Ring,
                RewardItemNumber = (int)Rings.VampireRing
            };

            var dustSpiritChallenge = new ChallengeInfo()
            {
                ChallengeName = "Dust Spirits",
                RequiredKillCount = 500,
                MonsterNames = { Monsters.DustSpirit },
                RewardType = ItemType.Ring,
                RewardItemNumber = (int)Rings.BurglarsRing
            };

            return new List<ChallengeInfo>()
            {
                slimeChallenge,
                shadowChallenge,
                skeletonChallenge,
                caveInsectsChallenge,
                duggyChallenge,
                batChallenge,
                dustSpiritChallenge
            };
        }
    }
}
