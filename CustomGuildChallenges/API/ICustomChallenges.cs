using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace CustomGuildChallenges.API
{
    public interface ICustomChallenges
    {
        /// <summary>
        ///     Is invoked each time a monster dies
        /// </summary>
        event EventHandler<Monster> MonsterKilled;
        /// <summary>
        ///     Set the dialogue for Gil
        /// </summary>
        /// <param name="initialNoRewardDialogue"></param>
        /// <param name="secondNoRewardDialogue"></param>
        /// <param name="specialRewardDialogue"></param>
        void SetGilDialogue(string initialNoRewardDialogue, string secondNoRewardDialogue, string specialRewardDialogue);
        /// <summary>
        ///     Add a challenge to the challenge list
        /// </summary>
        /// <param name="challengeName"></param>
        /// <param name="requiredKillCount"></param>
        /// <param name="rewardItemType"></param>
        /// <param name="rewardItemNumber"></param>
        /// <param name="monsterNames"></param>
        void AddChallenge(string challengeName, int requiredKillCount, int rewardItemType, int rewardItemNumber, IList<string> monsterNames);
        /// <summary>
        ///     Remove a challenge from the list
        /// </summary>
        /// <param name="challengeName"></param>
        void RemoveChallenge(string challengeName);
    }
}
