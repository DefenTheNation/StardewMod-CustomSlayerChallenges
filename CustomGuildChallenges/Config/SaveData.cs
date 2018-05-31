using System.Collections.Generic;

namespace TestMod
{
    public class SaveData
    {
        public List<ChallengeSave> Challenges { get; set; }

        public SaveData()
        {
            Challenges = new List<ChallengeSave>();
        }
    }
}
