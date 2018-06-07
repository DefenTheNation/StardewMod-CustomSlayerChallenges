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
        // Location Names to find dying monsters
        public readonly string FarmLocationName = "Farm";
        public readonly string BugLocationName = "BugLand";

        /// <summary>
        ///     Mod's version of the Adventure Guild
        /// </summary>
        internal readonly CustomAdventureGuild customAdventureGuild;

        /// <summary>
        ///     Is invoked each time a monster dies
        /// </summary>
        public event EventHandler<Monster> MonsterKilled;

        /// <summary>
        ///     Creates guild and sets up events
        /// </summary>
        /// <param name="guild"></param>
        public ConfigChallengeHelper(CustomAdventureGuild guild)
        {
            customAdventureGuild = guild;

            SaveEvents.AfterCreate += SetupMonsterKilledEvent;
            SaveEvents.AfterLoad += SetupMonsterKilledEvent;
        }

        /// <summary>
        ///     Add a challenge for the player to complete. The global config will not be updated.
        /// </summary>
        /// <param name="challengeName"></param>
        /// <param name="killCount"></param>
        /// <param name="rewardItemType"></param>
        /// <param name="rewardItemNumber"></param>
        /// <param name="monsterNames"></param>
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

        /// <summary>
        ///     Remove a challenge from the challenge list. The global config will not be updated.
        /// </summary>
        /// <param name="challengeName"></param>
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

        /// <summary>
        ///     Set the dialogue for Gil
        /// </summary>
        /// <param name="initialNoRewardDialogue"></param>
        /// <param name="secondNoRewardDialogue"></param>
        /// <param name="specialRewardDialogue"></param>
        public void SetGilDialogue(string initialNoRewardDialogue, string secondNoRewardDialogue, string specialRewardDialogue)
        {
            customAdventureGuild.GilNoRewardsText = initialNoRewardDialogue;
            customAdventureGuild.GilNappingText = secondNoRewardDialogue;
            customAdventureGuild.GilSpecialRewardText = specialRewardDialogue;
        }

        /// <summary>
        ///     Setup event that detects whether monsters are killed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SetupMonsterKilledEvent(object sender, EventArgs e)
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

        /// <summary>
        ///     Sets up detection for when a monster dies in the mines
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MineEvents_MineLevelChanged(object sender, EventArgsMineLevelChanged e)
        {
            if (Game1.mine != null) Game1.mine.characters.OnValueRemoved += Characters_OnValueRemoved;
        }

        /// <summary>
        ///     Fires the MonsterKilled event if the removed NPC is a monster and has 0 or less health
        /// </summary>
        /// <param name="value"></param>
        private void Characters_OnValueRemoved(NPC value)
        {
            if (value is Monster monster && monster.Health <= 0)
            {
                MonsterKilled.Invoke(Game1.currentLocation, monster);
            }
        }
    }
}
