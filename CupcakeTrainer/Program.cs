// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="ChewyMoon">
//   Copyright (C) 2015 ChewyMoon
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CupcakeTrainer
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Security.Permissions;

    using CupcakePrediction;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Newtonsoft.Json;

    using SharpDX;

    /// <summary>
    ///     The program.
    /// </summary>
    public class Program
    {
        #region Static Fields

        /// <summary>
        ///     The last spell casted
        /// </summary>
        private static SpellData lastSpellCast;

        /// <summary>
        ///     The last spell location
        /// </summary>
        private static Vector3 lastSpellLocation;

        /// <summary>
        ///     The pan
        /// </summary>
        private static CupcakePan pan;

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game loads.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void GameOnOnGameLoad(EventArgs args)
        {
            pan = new CupcakePan();
            LoadJson();

            Game.PrintChat("<font color=\"#E536F5\"><b>CupcakeTrainer:</b></font> loaded!");

            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;
            AttackableUnit.OnDamage += Obj_AI_Base_OnDamage;
        }

        /// <summary>
        ///     Loads the json.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void LoadJson()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CupcakeData.json");

            if (File.Exists(path))
            {
                pan = JsonConvert.DeserializeObject<CupcakePan>(File.ReadAllText(path));
            }
        }

        /// <summary>
        ///     The entry point of this application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += GameOnOnGameLoad;
        }

        /// <summary>
        ///     Fired when a unit takes damage.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AttackableUnitDamageEventArgs" /> instance containing the event data.</param>
        private static void Obj_AI_Base_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            var spell = new Spell(SpellSlot.Unknown, lastSpellCast.CastRangeDisplayOverride);
            spell.SetSkillshot(
                lastSpellCast.CastFrame / 30f, 
                Math.Abs(lastSpellCast.LineWidth) < float.Epsilon ? lastSpellCast.CastRadius : lastSpellCast.LineWidth, 
                lastSpellCast.MissileSpeed, 
                lastSpellCast.HaveHitBone, 
                SkillshotType.SkillshotLine);

            var ingredients = new CupcakeIngredients(
                ObjectManager.GetUnitByNetworkId<Obj_AI_Hero>(args.TargetNetworkId), 
                spell);

            var x = ingredients.ToXIngredient();
            var y = ingredients.ToYIngredient();

            x.BakedX = lastSpellLocation.X;
            y.BakedY = lastSpellLocation.Y;

            pan.X.Add(x);
            pan.Y.Add(y);

            SaveJson();
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private static void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            lastSpellCast = args.SData;
            lastSpellLocation = args.End;
        }

        /// <summary>
        ///     Saves the json.
        /// </summary>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]
        private static void SaveJson()
        {
            var json = JsonConvert.SerializeObject(pan, Formatting.None);
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CupcakeData.json");

            File.WriteAllText(path, json);
        }

        #endregion
    }
}