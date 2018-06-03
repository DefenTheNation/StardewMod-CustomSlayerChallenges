using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomGuildChallenges.API
{
    public interface ICustomChallenges
    {
        void SetGilDialogue(string initialNoRewardDialogue, string secondNoRewardDialogue);
        void AddChallenge(string challengeName, int requiredKillCount, int rewardItemType, int rewardItemNumber, IList<string> monsterNames);
        void RemoveChallenge(string challengeName);
    }
}
