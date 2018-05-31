using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Utilities;
using System;

namespace TestMod
{
    public class Class1 : Mod
    {
        public override void Entry(IModHelper helper)
        {
            MineEvents.MineLevelChanged += (sender, e) =>
            {
                Monitor.Log(Game1.player.currentLocation.Name);
            };

            void injectGuild(object sender, EventArgs e)
            {

                
            }

            SaveEvents.AfterLoad += injectGuild;
            SaveEvents.AfterCreate += injectGuild;

            Monitor.Log("Initialized");
        }

        

        public override object GetApi()
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {

        }
    }
}
