using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMod
{
    public class SlayerChallenge
    {
        public string ChallengeName { get; set; }
        public int RequiredKillCount { get; set; }
        public List<string> MonsterNames { get; set; }

        public ItemType RewardType { get; set; }
        public int RewardItemNumber { get; set; }

        public bool Collected { get; set; }
    }

    public enum ItemType
    {
        Regular = 0,

        Hat = 7,
        Ring = 8,
        MeleeWeapon = 9,
    }
}
