using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CupcakeCritic
{
    using System.Diagnostics;
    using System.Security.Permissions;

    using CupcakePrediction;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    ///     The program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Gets or sets the spell list.
        /// </summary>
        /// <value>
        /// The spell list.
        /// </value>
        static List<Spell> SpellList { get; set; }

        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;        
        }

        /// <summary>
        /// Fired when the game loads
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void Game_OnGameLoad(EventArgs args)
        {
            Debugger.Launch();
            Game.PrintChat("INIT");
            Cupcake.Initialize();
            Game.PrintChat("INIT COMPLETE");

            SpellList = new List<Spell>();

            var q = new Spell(SpellSlot.Q, 1200);
            var w = new Spell(SpellSlot.W, 1050);
            var r = new Spell(SpellSlot.R);

            q.SetSkillshot(0.251f, 60, 2000, true, SkillshotType.SkillshotLine);
            w.SetSkillshot(0.539f, 80, 1600, false, SkillshotType.SkillshotLine);
            r.SetSkillshot(1.75f, 160, 2000, false, SkillshotType.SkillshotLine);

            SpellList.Add(q);
            SpellList.Add(w);
            SpellList.Add(r);

            Game.PrintChat("HELLO WORLD!!?!?!?!?");
            Game.OnUpdate += Game_OnUpdate;
        }

        /// <summary>
        /// Fired when the game updates.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            try
            {
                Game.PrintChat(Cupcake.Initialized.ToString());
                if (!Cupcake.Initialized)
                {
                    return;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            

            foreach (var spell in SpellList)
            {
                var target = TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.True);

                if (target == null)
                {
                    continue;
                }

                var prediction = Cupcake.GetPrediction(new CupcakeIngredients(target, spell)).CastPosition;
                spell.Cast(prediction);
            }
        }
    }
}
