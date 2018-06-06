using CustomGuildChallenges.API;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
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
        protected ConfigChallengeHelper challengeHelper;

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

            // Verify config
            for(int i = 0; i < Config.Challenges.Count; i++)
            {
                for(int j = 0; j < Config.Challenges[i].MonsterNames.Count; j++)
                {
                    if(!Monsters.MonsterList.Contains(Config.Challenges[i].MonsterNames[j]))
                    {
                        Monitor.Log("Warning: Invalid monster name '" + Config.Challenges[i].MonsterNames[j] +
                            "' found. " + Config.Challenges[i].ChallengeName + " challenge will display but " +
                            "cannot be completed until monster name is fixed! ", LogLevel.Warn);
                    }
                }

                // TODO: Validate items on startup
                //Monitor.Log("Getting item for " + Config.Challenges[i].ChallengeName);
                //Monitor.Log("RT: " + Config.Challenges[i].RewardType + ", RI: " + Config.Challenges[i].RewardItemNumber);
                //var item = customAdventureGuild.CreateReward(Config.Challenges[i].RewardType, Config.Challenges[i].RewardItemNumber);
                //if(item == null)
                //{
                //    Monitor.Log("Warning: Invalid item for " + Config.Challenges[i].ChallengeName + " challenge. Reward will " +
                //        "not be collectable until item values are fixed in the config! ");
                //}
            }

            adventureGuild = new AdventureGuild(CustomAdventureGuild.MapPath, CustomAdventureGuild.MapName);
            challengeHelper = new ConfigChallengeHelper(new CustomAdventureGuild(Config.Challenges));

            challengeHelper.customAdventureGuild.GilNoRewardsText = Config.GilNoRewardDialogue;
            challengeHelper.customAdventureGuild.GilNappingText = Config.GilSleepingDialogue;

            SaveEvents.BeforeSave += presaveData;
            SaveEvents.AfterSave += injectGuild;
            SaveEvents.AfterLoad += injectGuild;
            SaveEvents.AfterLoad += SaveEvents_SetupMonstersKilled;
            SaveEvents.AfterCreate += injectGuild;
            SaveEvents.AfterCreate += SaveEvents_SetupMonstersKilled;            

            modHelper.ConsoleCommands.Add("player_setkills", "", (command, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    Monitor.Log("Usage: player_setkills \"Monster Name\" integerKillCount ", LogLevel.Warn);
                }
                else if (!int.TryParse(arguments[1], out int killCount))
                {
                    Monitor.Log("Invalid kill count. Use an integer, like 50 or 100. Example: player_setkills \"Green Slime\" 100 ", LogLevel.Warn);
                }
                else
                {
                    int before = Game1.stats.getMonstersKilled(arguments[0]);
                    Game1.stats.specificMonstersKilled[arguments[0]] = killCount;

                    Monitor.Log(arguments[0] + " kills changed from " + before + " to " + killCount, LogLevel.Info);
                }
            });

            modHelper.ConsoleCommands.Add("player_getkills", "", (command, arguments) =>
            {
                if(arguments.Length == 0)
                {
                    Monitor.Log("Usage: player_getkills \"Monster Name\"", LogLevel.Warn);
                }
                else
                {
                    Monitor.Log(arguments[0] + "'s killed: " + Game1.stats.getMonstersKilled(arguments[0]), LogLevel.Info);
                }               
            });

            modHelper.ConsoleCommands.Add("player_giveitem", "", (command, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    Monitor.Log("Usage: player_giveitem itemNumber", LogLevel.Warn);
                }
                else if (!int.TryParse(arguments[0], out int itemNumber))
                {
                    Monitor.Log("Invalid item number. Use an integer, like 50 or 100. Example: player_giveitem 100 ", LogLevel.Warn);
                }
                else
                {
                    var item = challengeHelper.customAdventureGuild.CreateReward(0, itemNumber);

                    if (item == null)
                    {
                        Monitor.Log("Invalid item number:  " + itemNumber + ". No item was spawned.", LogLevel.Warn);
                    }
                    else
                    {
                        Game1.player.addItemsByMenuIfNecessary(new Item[] { item }.ToList());
                        Monitor.Log("Item " + item.DisplayName + " given to player.", LogLevel.Info);
                    }
                }
            });

            modHelper.ConsoleCommands.Add("player_getallkills", "", (command, arguments) =>
            {
                foreach(var item in Game1.player.stats.specificMonstersKilled)
                {
                    Monitor.Log(item.Key + "'s killed: " + item.Value);
                }
            });

            string log = Config.CustomChallengesEnabled ?
                "Initialized (" + Config.Challenges.Count + " custom challenges uploaded)" :
                "Initialized (Vanilla challenges loaded)";

            Monitor.Log(log, LogLevel.Debug);
        }

        private void SaveEvents_SetupMonstersKilled(object sender, EventArgs e)
        {
            challengeHelper.SetupMonsterKilledEvent();
            challengeHelper.MonsterKilled += Events_MonsterKilled;
        }

        private void Events_MonsterKilled(object sender, Monster e)
        {
            if (!(sender is GameLocation location)) return;

            if (location.IsFarm && (Config.CountKillsOnFarm || e.Name == Monsters.WildernessGolem))
            {
                Game1.player.stats.monsterKilled(e.Name);
            }
        }

        /// <summary>
        ///     Returns API object to add and remove challenges and update Gil's dialogue
        /// </summary>
        /// <returns>ConfigChallengeHelper</returns>
        public override object GetApi()
        {
            return new ConfigChallengeHelper(challengeHelper.customAdventureGuild);
        }

        void injectGuild(object sender, EventArgs e)
        {
            string saveDataPath = Path.Combine("saveData", Constants.SaveFolderName + ".json");
            var saveData = modHelper.ReadJsonFile<SaveData>(saveDataPath) ?? new SaveData();

            foreach (var savedChallenge in saveData.Challenges)
            {
                foreach (var slayerChallenge in challengeHelper.customAdventureGuild.ChallengeList)
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
                    Game1.locations.Add(challengeHelper.customAdventureGuild);
                }
            }
        }

        void presaveData(object sender, EventArgs e)
        {
            string saveDataPath = Path.Combine("saveData", Constants.SaveFolderName + ".json");
            var saveData = new SaveData();

            foreach (var slayerChallenge in challengeHelper.customAdventureGuild.ChallengeList)
            {
                var save = new ChallengeSave()
                {
                    ChallengeName = slayerChallenge.Info.ChallengeName,
                    Collected = slayerChallenge.CollectedReward
                };

                saveData.Challenges.Add(save);
            }

            modHelper.WriteJsonFile(saveDataPath, saveData);
            Monitor.Log("Saved " + challengeHelper.customAdventureGuild.ChallengeList.Count + " challenges.");

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
                RequiredKillCount = 200,
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
