using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using xTile.Dimensions;

namespace CustomGuildChallenges
{
    /// <summary>
    ///     Custom implementation of the adventure guild
    ///     Required in order to update the slayer list and rewards
    /// </summary>
    public class CustomAdventureGuild : AdventureGuild
    {
        public const string StandardMapPath = "Maps\\AdventureGuild";
        public const string StandardMapName = "AdventureGuild";

        public string MapName { get; set; }
        public string MapPath { get; set; }
        public string GilNoRewardsText { get; set; } = "";
        public string GilNappingText { get; set; } = "";
        public string GilSpecialRewardText { get; set; } = "";

        protected bool talkedToGil;
        protected readonly AdventureGuild adventureGuild;        
        protected readonly NPC Gil = new NPC(null, new Vector2(-1000f, -1000f), "AdventureGuild", 2, "Gil", false, null, Game1.content.Load<Texture2D>("Portraits\\Gil"));

        protected IModHelper modHelper;
        public IList<SlayerChallenge> ChallengeList { get; set; }

        #region Constructors

        public CustomAdventureGuild() : base(StandardMapPath, StandardMapName)
        {

        }

        /// <summary>
        ///     Loads custom slayer challenge list with vanilla map path and name
        /// </summary>
        /// <param name="customChallengeList"></param>
        public CustomAdventureGuild(IList<ChallengeInfo> customChallengeList, IModHelper helper) : base(StandardMapPath, StandardMapName)
        {
            var challenges = new List<SlayerChallenge>();
            foreach (var info in customChallengeList) challenges.Add(new SlayerChallenge() { Info = info });

            MapName = StandardMapName;
            MapPath = StandardMapPath;

            ChallengeList = challenges;
            modHelper = helper;
            adventureGuild = new AdventureGuild(StandardMapPath, StandardMapName);

            init();
        }

        /// <summary>
        ///     Loads custom slayer challenge list with custom map path and name
        /// </summary>
        /// <param name="map"></param>
        /// <param name="name"></param>
        /// <param name="customChallengeList"></param>
        public CustomAdventureGuild(IList<SlayerChallenge> customChallengeList, IModHelper helper, string map, string name) : base(map, name)
        {
            ChallengeList = customChallengeList;
            modHelper = helper;
            adventureGuild = new AdventureGuild(StandardMapPath, StandardMapName);

            init();
        }

        protected void init()
        {
            addCharacter(new NPC(new AnimatedSprite("Characters\\Marlon", 0, 16, 32), new Vector2(320f, 704f), "AdventureGuild", 2, "Marlon", false, null, Game1.content.Load<Texture2D>("Portraits\\Marlon")));

            

            SaveEvents.BeforeSave += presaveData;
            SaveEvents.AfterSave += injectGuild;
            SaveEvents.AfterLoad += injectGuild;
            SaveEvents.AfterCreate += injectGuild;
        }

        #endregion

        // Required to reimplement Monster Kill List and Gil's rewards
        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            switch ((map.GetLayer("Buildings").Tiles[tileLocation] != null) ? map.GetLayer("Buildings").Tiles[tileLocation].TileIndex : (-1))
            {
                case 1306:
                    ShowNewMonsterKillList();
                    return true;
                case 1291:
                case 1292:
                case 1355:
                case 1356:
                case 1357:
                case 1358:
                    TalkToGil();
                    return true;
                default:
                    return base.checkAction(tileLocation, viewport, who);
            }
        }

        /// <summary>
        ///     Creates the reward item using StardewValley.Objects.ObjectFactory
        /// </summary>
        /// <param name="rewardType"></param>
        /// <param name="rewardItemNumber"></param>
        /// <returns></returns>
        public virtual Item CreateReward(int rewardType, int rewardItemNumber, int rewardItemStack)
        {
            switch (rewardType)
            {
                case (int)ItemType.Hat:
                    return new Hat(rewardItemNumber);
                case (int)ItemType.Ring:
                    return new Ring(rewardItemNumber);
                case (int)ItemType.SpecialItem:
                    return new SpecialItem(rewardItemNumber);
                case (int)ItemType.Boots:
                    return new StardewValley.Objects.Boots(rewardItemNumber);
                default:
                    return ObjectFactory.getItemFromDescription((byte)rewardType, rewardItemNumber, rewardItemStack);
            }
        }

        // Required to reset talkedToGil flag
        protected override void resetLocalState()
        {
            base.resetLocalState();
            talkedToGil = false;

            if (!Game1.player.mailReceived.Contains("guildMember"))
            {
                Game1.player.mailReceived.Add("guildMember");
            }
        }

        /// <summary>
        ///     Build strings for display when viewing challenge list on guild wall
        /// </summary>
        protected virtual void ShowNewMonsterKillList()
        {
            if (!Game1.player.mailReceived.Contains("checkedMonsterBoard"))
            {
                Game1.player.mailReceived.Add("checkedMonsterBoard");
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_Header").Replace('\n', '^') + "^");

            foreach(var challenge in ChallengeList)
            {
                int kills = 0;
                foreach(var monsterName in challenge.Info.MonsterNames)
                {
                    kills += Game1.stats.getMonstersKilled(monsterName);
                }

                stringBuilder.Append(KillListLine(challenge.Info.ChallengeName, kills, challenge.Info.RequiredKillCount));
            }

            stringBuilder.Append(Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_Footer").Replace('\n', '^'));
            Game1.drawLetterMessage(stringBuilder.ToString());
        }

       /// <summary>
       ///      Checks to see if there are new rewards. If not, display a dialogue from Gil
       /// </summary>
        protected virtual void TalkToGil()
        {
            int specialItemsCollected = 0;
            List<Item> rewards = new List<Item>();
            List<SlayerChallenge> completedChallenges = new List<SlayerChallenge>();

            // Check for available rewards
            foreach(var challenge in ChallengeList)
            {
                if (challenge.CollectedReward) continue;

                int kills = 0;
                foreach (var monsterName in challenge.Info.MonsterNames)
                {
                    kills += Game1.stats.getMonstersKilled(monsterName);
                }

                if(kills >= challenge.Info.RequiredKillCount)
                {
                    var rewardItem = CreateReward(challenge.Info.RewardType, challenge.Info.RewardItemNumber, challenge.Info.RewardItemStack);

                    if (rewardItem == null)
                    {
                        throw new Exception("Invalid reward parameters for challenge " + challenge.Info.ChallengeName + ":\n" +
                            "Reward Type: " + challenge.Info.RewardType + "\n" +
                            "Reward Item Number: " + challenge.Info.RewardItemNumber + "\n");
                    }
                    else if(challenge.Info.RewardType == 0 && challenge.Info.RewardItemNumber == 434)   // Stardrop award
                    {
                        Game1.drawDialogue(Gil, GilSpecialRewardText);

                        Game1.player.holdUpItemThenMessage(rewardItem, true);
                        specialItemsCollected++;

                        challenge.CollectedReward = true;

                        break;
                    }
                    // Add special section for special item
                    else if (rewardItem is SpecialItem specialItem)
                    {
                        Game1.drawDialogue(Gil, GilSpecialRewardText);

                        Game1.player.holdUpItemThenMessage(specialItem, true);
                        specialItem.actionWhenReceived(Game1.player);

                        specialItemsCollected++;
                        challenge.CollectedReward = true;

                        break;
                    }
                    else
                    {
                        completedChallenges.Add(challenge);
                    }
                }
            }

            // Display rewards/dialogue for talking to Gil
            if(specialItemsCollected > 0)
            {
                return;
            }
            else if(completedChallenges.Count > 0)
            {
                Item rewardItem;
                foreach(var challenge in completedChallenges)
                {
                    rewardItem = CreateReward(challenge.Info.RewardType, challenge.Info.RewardItemNumber, challenge.Info.RewardItemStack);
                    if (rewardItem is StardewValley.Object)
                    {
                        rewardItem.specialItem = true;
                        rewards.Add(rewardItem);
                    }
                    else if (!Game1.player.hasOrWillReceiveMail("Gil_" + challenge.Info.ChallengeName + "_" + rewardItem.Name))
                    {
                        Game1.player.mailReceived.Add("Gil_" + challenge.Info.ChallengeName + "_" + rewardItem.Name);
                        rewards.Add(rewardItem);
                    }

                    challenge.CollectedReward = true;
                }

                Game1.activeClickableMenu = new ItemGrabMenu(rewards);
            }
            else if(talkedToGil)
            {
                Game1.drawDialogue(Gil, GilNappingText);
            }
            else
            {
                Game1.drawDialogue(Gil, GilNoRewardsText);
                talkedToGil = true;
            }
        }

        /// <summary>
        ///     Generates a single challenge line for the challenge board
        ///     Localization done in the config files
        /// </summary>
        /// <param name="challengeName"></param>
        /// <param name="killCount"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual string KillListLine(string challengeName, int killCount, int target)
        {
            if (killCount == 0)
            {
                return "0/" + target + " ????\n\n^";
            }
            else if (killCount >= target)
            {
                return killCount + " " + challengeName + " * \n\n ^";
            }
            else
            {
                return killCount + "/" + target + " " + challengeName + " \n\n ^";
            }
        }

        /// <summary>
        ///     Saves the status of challenges and switches the
        ///     CustomAdventureGuild with AdventureGuild to prevent
        ///     crashing the save process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void presaveData(object sender, EventArgs e)
        {
            string saveDataPath = Path.Combine("saveData", Constants.SaveFolderName + ".json");
            var saveData = new SaveData();

            foreach (var slayerChallenge in ChallengeList)
            {
                var save = new ChallengeSave()
                {
                    ChallengeName = slayerChallenge.Info.ChallengeName,
                    Collected = slayerChallenge.CollectedReward
                };

                saveData.Challenges.Add(save);
            }

            modHelper.WriteJsonFile(saveDataPath, saveData);

            for (int i = 0; i < Game1.locations.Count; i++)
            {
                if (Game1.locations[i].Name == MapName)
                {
                    Game1.locations.RemoveAt(i);
                    Game1.locations.Add(adventureGuild);
                }
            }
        }

        /// <summary>
        ///     Read the save data file and replace the AdventureGuild with
        ///     CustomAdventureGuild
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void injectGuild(object sender, EventArgs e)
        {
            string saveDataPath = Path.Combine("saveData", Constants.SaveFolderName + ".json");
            var saveData = modHelper.ReadJsonFile<SaveData>(saveDataPath) ?? new SaveData();

            foreach (var savedChallenge in saveData.Challenges)
            {
                foreach (var slayerChallenge in ChallengeList)
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
                if (Game1.locations[i].Name == StandardMapName)
                {
                    Game1.locations.RemoveAt(i);
                    Game1.locations.Add(this);
                }
            }
        }
    }
}
