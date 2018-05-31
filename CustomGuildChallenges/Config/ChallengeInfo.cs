﻿using System.Collections.Generic;

namespace CustomGuildChallenges
{
    public class ChallengeInfo
    {
        public string ChallengeName { get; set; }
        public int RequiredKillCount { get; set; }
        public ItemType RewardType { get; set; }
        public int RewardItemNumber { get; set; }
        public List<string> MonsterNames { get; set; }

        public ChallengeInfo()
        {
            MonsterNames = new List<string>();
        }
    }
}
