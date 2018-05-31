using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.IO;
using System.Linq;

namespace TestMod
{
    public class CustomGuildChallengeMod : Mod
    {
        protected AdventureGuild adventureGuild;
        protected CustomAdventureGuild customAdventureGuild;

        public ModConfig Config { get; set; }

        public override void Entry(IModHelper helper)
        {
            Monitor.Log("Starting entry...");

            Config = helper.ReadConfig<ModConfig>();

            // Create config file using vanilla challenges
            if(Config == null || Config.Challenges == null || Config.Challenges.Count == 0)
            {
                Config = new ModConfig()
                {
                    CustomChallengesEnabled = false,
                    Challenges = CustomAdventureGuild.GetVanillaSlayerChallenges().ToList()
                };

                Monitor.Log("Config has " + Config.Challenges.Count + " challenges!");

                helper.WriteConfig(Config);
            }
            // Use vanilla challenges but do not overwrite the config
            else if(!Config.CustomChallengesEnabled)
            {
                Config.Challenges = CustomAdventureGuild.GetVanillaSlayerChallenges().ToList();
            }
            
            Monitor.Log("Config created");

            adventureGuild = new AdventureGuild(CustomAdventureGuild.MapPath, CustomAdventureGuild.MapName);
            customAdventureGuild = new CustomAdventureGuild(Config.Challenges);

            Monitor.Log("Custom guild created");

            void injectGuild(object sender, EventArgs e)
            {
                string saveDataPath = Path.Combine("saveData", Constants.SaveFolderName + ".json");
                var saveData = helper.ReadJsonFile<SaveData>(saveDataPath) ?? new SaveData();

                Monitor.Log("Read " + saveData.Challenges.Count + " challenges from save file");

                foreach(var savedChallenge in saveData.Challenges)
                {
                    foreach(var slayerChallenge in customAdventureGuild.ChallengeList)
                    {
                        if(savedChallenge.ChallengeName == slayerChallenge.Info.ChallengeName)
                        {
                            slayerChallenge.Collected = savedChallenge.Collected;
                            break;
                        }
                    }
                }

                // Kill old guild, replace with new guild
                for(int i = 0; i < Game1.locations.Count; i++)
                {
                    if(Game1.locations[i].Name == CustomAdventureGuild.MapName)
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
                        Collected = slayerChallenge.Collected
                    };

                    saveData.Challenges.Add(save);
                }

                helper.WriteJsonFile(saveDataPath, saveData);
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

            //void saveLocalData(object sender, EventArgs e)
            //{

                

            //    // Kill old guild, replace with new guild
            //    for (int i = 0; i < Game1.locations.Count; i++)
            //    {
            //        if (Game1.locations[i].Name == CustomAdventureGuild.MapName)
            //        {
            //            Game1.locations.RemoveAt(i);
            //            Game1.locations.Add(customAdventureGuild);

            //            Monitor.Log("CUSTOM GUILD INJECTED!!");
            //        }
            //    }

            //    Monitor.Log("DATA SAVED!");
            //}

            SaveEvents.BeforeSave += presaveData;
            SaveEvents.AfterLoad += injectGuild;
            SaveEvents.AfterCreate += injectGuild;
            SaveEvents.AfterSave += injectGuild;

            helper.ConsoleCommands.Add("setkills", "", (command, arguments) =>
            {
                Monitor.Log(arguments[0]);
                Monitor.Log(arguments[1]);
                Game1.stats.specificMonstersKilled[arguments[0]] = int.Parse(arguments[1]);
            });

            helper.ConsoleCommands.Add("outputstrings", "", (command, arguments) =>
            {
                int killCount = 100;
                int target = 200;

                string monsterNamePlural = Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_" + "Slimes");
                Monitor.Log("Plural: " + monsterNamePlural);
                Monitor.Log("'" + Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat_None", killCount, target, monsterNamePlural) + "^'");
                Monitor.Log("'" + Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat_OverTarget", killCount, target, monsterNamePlural) + "^'");
                Monitor.Log("'" + Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat", killCount, target, monsterNamePlural) + "^'");
            });

            string log = Config.CustomChallengesEnabled ?
                "Initialized (" + Config.Challenges.Count + " custom challenges uploaded)" :
                "Initialized (Vanilla challenges loaded)";

            Monitor.Log(log, LogLevel.Info);
        }

        

        public override object GetApi()
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {

        }
    }
}
