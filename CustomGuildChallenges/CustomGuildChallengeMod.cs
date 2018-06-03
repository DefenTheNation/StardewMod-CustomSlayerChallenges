using CustomGuildChallenges.API;
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
            modHelper = helper;
            Config = helper.ReadConfig<ModConfig>();

            // Create config file using vanilla challenges
            if(Config == null || Config.Challenges == null || Config.Challenges.Count == 0)
            {
                Config = new ModConfig()
                {
                    CustomChallengesEnabled = false,
                    Challenges = GetVanillaSlayerChallenges().ToList(),
                    GilNoRewardDialogue = Game1.content.LoadString("Characters\\Dialogue\\Gil:ComeBackLater"),
                    GilSleepingDialogue = Game1.content.LoadString("Characters\\Dialogue\\Gil:Snoring")
                };

                helper.WriteConfig(Config);
            }
            // Use vanilla challenges but do not overwrite the config
            else if(!Config.CustomChallengesEnabled)
            {
                Config.Challenges = GetVanillaSlayerChallenges().ToList();
            }

            adventureGuild = new AdventureGuild(CustomAdventureGuild.MapPath, CustomAdventureGuild.MapName);
            customAdventureGuild = new CustomAdventureGuild(Config.Challenges);

            customAdventureGuild.GilNoRewardsText = Config.GilNoRewardDialogue;
            customAdventureGuild.GilNappingText = Config.GilSleepingDialogue;

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

        /// <summary>
        ///     Returns API object to add and remove challenges and update Gil's dialogue
        /// </summary>
        /// <returns>ConfigChallengeHelper</returns>
        public override object GetApi()
        {
            return new ConfigChallengeHelper(customAdventureGuild);
        }

        void injectGuild(object sender, EventArgs e)
        {
            string saveDataPath = Path.Combine("saveData", Constants.SaveFolderName + ".json");
            var saveData = modHelper.ReadJsonFile<SaveData>(saveDataPath) ?? new SaveData();

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
                RewardType = (int)ItemType.Ring,
                RewardItemNumber = (int)Rings.SlimeCharmerRing
            };

            var shadowChallenge = new ChallengeInfo()
            {
                ChallengeName = "Void Spirits",
                RequiredKillCount = 150,
                MonsterNames = { Monsters.ShadowGuy, Monsters.ShadowShaman, Monsters.ShadowBrute },
                RewardType = (int)ItemType.Ring,
                RewardItemNumber = (int)Rings.SavageRing
            };

            var skeletonChallenge = new ChallengeInfo()
            {
                ChallengeName = "Skeletons",
                RequiredKillCount = 50,
                MonsterNames = { Monsters.Skeleton, Monsters.SkeletonMage, Monsters.SkeletonWarrior },
                RewardType = (int)ItemType.Hat,
                RewardItemNumber = (int)Hats.SkeletonMask
            };

            var caveInsectsChallenge = new ChallengeInfo()
            {
                ChallengeName = "Cave Insects",
                RequiredKillCount = 125,
                MonsterNames = { Monsters.Bug, Monsters.Grub, Monsters.Fly },
                RewardType = (int)ItemType.MeleeWeapon,
                RewardItemNumber = (int)MeleeWeapons.InsectHead
            };

            var duggyChallenge = new ChallengeInfo()
            {
                ChallengeName = "Duggies",
                RequiredKillCount = 30,
                MonsterNames = { Monsters.Duggy },
                RewardType = (int)ItemType.Hat,
                RewardItemNumber = (int)Hats.HardHat
            };

            var batChallenge = new ChallengeInfo()
            {
                ChallengeName = "Bats",
                RequiredKillCount = 100,
                MonsterNames = { Monsters.Bat, Monsters.FrostBat, Monsters.LavaBat },
                RewardType = (int)ItemType.Ring,
                RewardItemNumber = (int)Rings.VampireRing
            };

            var dustSpiritChallenge = new ChallengeInfo()
            {
                ChallengeName = "Dust Spirits",
                RequiredKillCount = 500,
                MonsterNames = { Monsters.DustSpirit },
                RewardType = (int)ItemType.Ring,
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
