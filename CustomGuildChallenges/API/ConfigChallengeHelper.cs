using System.Collections.Generic;
using System.Linq;

namespace CustomGuildChallenges.API
{
    public class ConfigChallengeHelper : ICustomChallenges
    {
        private readonly CustomAdventureGuild customAdventureGuild;

        public ConfigChallengeHelper(CustomAdventureGuild guild)
        {
            customAdventureGuild = guild;
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
    }
}
