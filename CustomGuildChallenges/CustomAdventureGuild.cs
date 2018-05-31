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

namespace TestMod
{
    [XmlInclude(typeof(CustomAdventureGuild))]
    public class CustomAdventureGuild : AdventureGuild
    {
        public const string MapPath = "Maps\\AdventureGuild";
        public const string MapName = "AdventureGuild";

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

        // TODO: Add localization
        public static IList<ChallengeInfo> GetVanillaSlayerChallenges()
        {
            var slimeChallenge = new ChallengeInfo()
            {
                ChallengeName = "Slimes",
                RequiredKillCount = 1000,
                MonsterNames = { Monsters.GreenSlime, Monsters.FrostJelly, Monsters.Sludge },
                RewardType = ItemType.Ring,
                RewardItemNumber = (int) Rings.SlimeCharmerRing
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

        #endregion

        protected override void resetLocalState()
        {
            base.resetLocalState();
            talkedToGil = false;

            if (!Game1.player.mailReceived.Contains("guildMember"))
            {
                Game1.player.mailReceived.Add("guildMember");
            }
        }

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

        protected virtual void TalkToGil()
        {
            List<Item> rewards = new List<Item>();

            foreach(var challenge in ChallengeList)
            {
                if (challenge.Collected) continue;

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
                        throw new Exception("BAD ITEM DATA!!!!");
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
                    challenge.Collected = true;
                }
            }

            if(rewards.Count > 0)
            {
                Game1.activeClickableMenu = new ItemGrabMenu(rewards);
            }
            else if(talkedToGil)
            {
                Game1.drawDialogue(Gil, "I'm tryin to take a nap ya damn hippie");
            }
            else
            {
                Game1.drawDialogue(Gil, "I ain't got no freebies for a hippie like you");
                talkedToGil = true;
            }
        }

        // TODO: Localization
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

        protected virtual Item CreateReward(ItemType rewardType, int rewardItemNumber)
        {
            switch(rewardType)
            {
                case ItemType.Hat:
                    return new Hat(rewardItemNumber);
                case ItemType.Ring:
                    return new Ring(rewardItemNumber);
                case ItemType.MeleeWeapon:
                    return new MeleeWeapon(rewardItemNumber);
                default:
                    return ObjectFactory.getItemFromDescription((byte)rewardType, rewardItemNumber, 1);
            }
        }
    }
}
