using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomGuildChallenges.API
{
    public class ConfigChallengeHelper : ICustomChallenges
    {
        public readonly string FarmLocationName = "Farm";
        public readonly string BugLocationName = "BugLand";
        
        internal readonly CustomAdventureGuild customAdventureGuild;

        public event EventHandler<Monster> MonsterKilled;

        public ConfigChallengeHelper(CustomAdventureGuild guild)
        {
            customAdventureGuild = guild;
            SetupMonsterKilledEvent();
        }

        public void AddChallenge(string challengeName, int killCount, int rewardItemType, int rewardItemNumber, IList<string> monsterNames)
        {
            var challenge = new SlayerChallenge()
            {
                CollectedReward = false,
                Info = new ChallengeInfo()
                {
                    ChallengeName = challengeName,
                    RequiredKillCount = killCount,
                    RewardType = rewardItemType,
                    RewardItemNumber = rewardItemNumber,
                    MonsterNames = monsterNames.ToList()
                }
            };

            customAdventureGuild.ChallengeList.Add(challenge);
        }

        public void RemoveChallenge(string challengeName)
        {
            for(int i = 0; i < customAdventureGuild.ChallengeList.Count; i++)
            {
                if(customAdventureGuild.ChallengeList[i].Info.ChallengeName == challengeName)
                {
                    customAdventureGuild.ChallengeList.RemoveAt(i);
                    break;
                }
            }
        }

        public void SetGilDialogue(string initialNoRewardDialogue, string secondNoRewardDialogue)
        {
            customAdventureGuild.GilNoRewardsText = initialNoRewardDialogue;
            customAdventureGuild.GilNappingText = secondNoRewardDialogue;
        }

        internal void SetupMonsterKilledEvent()
        {
            // Inject into all mines
            MineEvents.MineLevelChanged += MineEvents_MineLevelChanged;
            // Inject into all locations that spawn monsters that are not in the mines
            foreach (var location in Game1.locations)
            {
                if (location.Name == FarmLocationName || location.Name == BugLocationName)
                {
                    location.characters.OnValueRemoved += Characters_OnValueRemoved;
                }
            }
        }

        private void MineEvents_MineLevelChanged(object sender, EventArgsMineLevelChanged e)
        {
            if (Game1.mine != null) Game1.mine.characters.OnValueRemoved += Characters_OnValueRemoved;
        }

        private void Characters_OnValueRemoved(NPC value)
        {
            if (value is Monster monster && monster.Health <= 0)
            {
                MonsterKilled.Invoke(Game1.currentLocation, monster);
            }
        }
    }
}
