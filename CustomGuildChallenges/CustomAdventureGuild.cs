using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using xTile.Dimensions;

namespace CustomGuildChallenges
{
    /// <summary>
    ///     Custom implementation of the adventure guild
    ///     Required in order to update the slayer list and rewards
    /// </summary>
    public class CustomAdventureGuild : AdventureGuild
    {
        public const string MapPath = "Maps\\AdventureGuild";
        public const string MapName = "AdventureGuild";

        public string GilNoRewardsText { get; set; } = "";
        public string GilNappingText { get; set; } = "";

        protected bool talkedToGil;
        protected readonly NPC Gil = new NPC(null, new Vector2(-1000f, -1000f), "AdventureGuild", 2, "Gil", false, null, Game1.content.Load<Texture2D>("Portraits\\Gil"));

        public IList<SlayerChallenge> ChallengeList { get; set; }

        #region Constructors

        /// <summary>
        ///     Loads custom slayer challenge list with vanilla map path and name
        /// </summary>
        /// <param name="customChallengeList"></param>
        public CustomAdventureGuild(IList<ChallengeInfo> customChallengeList) : base("Maps\\AdventureGuild", "AdventureGuild")
        {
            var challenges = new List<SlayerChallenge>();
            foreach (var info in customChallengeList) challenges.Add(new SlayerChallenge() { Info = info });

            ChallengeList = challenges;
            AddMarlon();
        }

        /// <summary>
        ///     Loads custom slayer challenge list with custom map path and name
        /// </summary>
        /// <param name="map"></param>
        /// <param name="name"></param>
        /// <param name="customChallengeList"></param>
        public CustomAdventureGuild(string map, string name, IList<SlayerChallenge> customChallengeList) : base(map, name)
        {
            ChallengeList = customChallengeList;
            AddMarlon();
        }

        protected void AddMarlon()
        {
            addCharacter(new NPC(new AnimatedSprite("Characters\\Marlon", 0, 16, 32), new Vector2(320f, 704f), "AdventureGuild", 2, "Marlon", false, null, Game1.content.Load<Texture2D>("Portraits\\Marlon")));
        }

        #endregion

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
       ///  Checks to see if there are any 
       /// </summary>
        protected virtual void TalkToGil()
        {
            List<Item> rewards = new List<Item>();

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
                    var rewardItem = CreateReward(challenge.Info.RewardType, challenge.Info.RewardItemNumber);

                    if (rewardItem == null)
                    {
                        throw new Exception("Invalid reward parameters for challenge " + challenge.Info.ChallengeName + ":\n" +
                            "Reward Type: " + challenge.Info.RewardType + "\n" +
                            "Reward Item Number: " + challenge.Info.RewardItemNumber + "\n");
                    }
                    else if (rewardItem is StardewValley.Object)
                    {
                        rewardItem.specialItem = true;
                    }
                    else if (!Game1.player.hasOrWillReceiveMail("Gil_" + challenge.Info.ChallengeName + "_" + rewardItem.Name))
                    {
                        Game1.player.mailReceived.Add("Gil_" + challenge.Info.ChallengeName + "_" + rewardItem.Name);
                    }

                    rewards.Add(rewardItem);
                    challenge.CollectedReward = true;
                }
            }

            if(rewards.Count > 0)
            {
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
        ///     Creates the reward item using StardewValley.Objects.ObjectFactory
        /// </summary>
        /// <param name="rewardType"></param>
        /// <param name="rewardItemNumber"></param>
        /// <returns></returns>
        protected virtual Item CreateReward(int rewardType, int rewardItemNumber)
        {
            switch(rewardType)
            {
                case (int)ItemType.Hat:
                    return new Hat(rewardItemNumber);
                case (int)ItemType.Ring:
                    return new Ring(rewardItemNumber);
                default:
                    return ObjectFactory.getItemFromDescription((byte)rewardType, rewardItemNumber, 1);
            }
        }
    }
}
