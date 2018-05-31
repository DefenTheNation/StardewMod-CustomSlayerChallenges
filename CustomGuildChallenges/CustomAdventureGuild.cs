using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace TestMod
{
    public class CustomAdventureGuild : AdventureGuild
    {
        protected IList<SlayerChallenge> ChallengeList;

        public CustomAdventureGuild()
        {
            ChallengeList = GetVanillaSlayerChallenges();            
        }

        // TODO: Add localization
        public IList<SlayerChallenge> GetVanillaSlayerChallenges()
        {
            var slimeChallenge = new SlayerChallenge()
            {
                ChallengeName = "Slimes",
                RequiredKillCount = 1000,
                MonsterNames = { Monsters.GreenSlime, Monsters.FrostJelly, Monsters.Sludge }
            };

            var shadowChallenge = new SlayerChallenge()
            {
                ChallengeName = "Void Spirits",
                RequiredKillCount = 150,
                MonsterNames = { Monsters.ShadowGuy, Monsters.ShadowShaman, Monsters.ShadowBrute }
            };

            var skeletonChallenge = new SlayerChallenge()
            {
                ChallengeName = "Skeletons",
                RequiredKillCount = 50,
                MonsterNames = { Monsters.Skeleton, Monsters.SkeletonMage, Monsters.SkeletonWarrior }
            };

            var caveInsectsChallenge = new SlayerChallenge()
            {
                ChallengeName = "Cave Insects",
                RequiredKillCount = 125,
                MonsterNames = { Monsters.Bug, Monsters.Grub, Monsters.Fly }
            };

            var duggyChallenge = new SlayerChallenge()
            {
                ChallengeName = "Duggies",
                RequiredKillCount = 30,
                MonsterNames = { Monsters.Duggy }
            };

            var batChallenge = new SlayerChallenge()
            {
                ChallengeName = "Bats",
                RequiredKillCount = 100,
                MonsterNames = { Monsters.Bat, Monsters.FrostBat, Monsters.LavaBat }
            };

            var dustSpiritChallenge = new SlayerChallenge()
            {
                ChallengeName = "Dust Spirits",
                RequiredKillCount = 500,
                MonsterNames = { Monsters.DustSpirit }
            };

            return new List<SlayerChallenge>()
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

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            switch ((base.map.GetLayer("Buildings").Tiles[tileLocation] != null) ? base.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex : (-1))
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
                    Gil();
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
                foreach(var monsterName in challenge.MonsterNames)
                {
                    kills += Game1.stats.getMonstersKilled(monsterName);
                }

                stringBuilder.Append(KillListLine(challenge.ChallengeName, kills, challenge.RequiredKillCount));
            }

            stringBuilder.Append(Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_Footer").Replace('\n', '^'));
            Game1.drawLetterMessage(stringBuilder.ToString());
        }

        protected virtual void Gil()
        {
            List<Item> rewards = new List<Item>();

            foreach(var challenge in ChallengeList)
            {
                if (challenge.Collected) continue;

                int kills = 0;
                foreach (var monsterName in challenge.MonsterNames)
                {
                    kills += Game1.stats.getMonstersKilled(monsterName);
                }

                if(kills >= challenge.RequiredKillCount)
                {
                    var rewardItem = CreateReward(challenge.RewardType, challenge.RewardItemNumber);

                    if(rewardItem == null)
                    {
                        throw new Exception("BAD ITEM DATA!!!!");
                    }

                    rewards.Add(rewardItem);
                }
            }

            if(rewards.Count > 0)
            {

            }
        }

        // TODO: Localization
        protected virtual string KillListLine(string challengeName, int killCount, int target)
        {
            if (killCount == 0)
            {
                return "??? / ???" + "^";
            }
            else if (killCount >= target)
            {
                return killCount + " * ^";
            }
            else
            {
                return killCount + " / " + target;
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
                case ItemType.Regular: default:
                    return ObjectFactory.getItemFromDescription((byte)rewardType, rewardItemNumber, 1);
            }
        }
    }
}
