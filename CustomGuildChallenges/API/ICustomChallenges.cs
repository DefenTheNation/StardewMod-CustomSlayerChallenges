using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace CustomGuildChallenges.API
{
    public interface ICustomChallenges
    {
        event EventHandler<Monster> MonsterKilled;

        void SetGilDialogue(string initialNoRewardDialogue, string secondNoRewardDialogue);
        void AddChallenge(string challengeName, int requiredKillCount, int rewardItemType, int rewardItemNumber, IList<string> monsterNames);
        void RemoveChallenge(string challengeName);
    }
}
