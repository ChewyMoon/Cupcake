// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CupcakeIngredients.cs" company="ChewyMoon">
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
//   Provides the necessary information for prediction.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CupcakePrediction
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using numl.Math.LinearAlgebra;

    /// <summary>
    ///     Provides the necessary information for prediction.
    /// </summary>
    public class CupcakeIngredients
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CupcakeIngredients" /> class.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="spell">
        ///     The spell.
        /// </param>
        /// <param name="source">
        ///     The source. <c>null</c> if the source is the Player.
        /// </param>
        public CupcakeIngredients(Obj_AI_Base target, Spell spell, Obj_AI_Base source = null)
        {
            this.Delay = spell.Delay;
            this.MissileSpeed = spell.Speed;
            this.SourcePosition = source == null
                                      ? ObjectManager.Player.ServerPosition.To2D().ToNumlVector()
                                      : source.ServerPosition.To2D().ToNumlVector();
            this.TargetMoveSpeed = target.MoveSpeed;
            this.TargetPosition = target.ServerPosition.To2D().ToNumlVector();
            this.Waypoints = target.GetWaypoints().Select(x => x.ToNumlVector());
            this.Width = spell.Width;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the delay.
        /// </summary>
        /// <value>
        ///     The delay.
        /// </value>
        public float Delay { get; set; }

        /// <summary>
        ///     Gets or sets the missile speed.
        /// </summary>
        /// <value>
        ///     The missile speed.
        /// </value>
        public float MissileSpeed { get; set; }

        /// <summary>
        ///     Gets or sets the source position.
        /// </summary>
        /// <value>
        ///     The source position.
        /// </value>
        public Vector SourcePosition { get; set; }

        /// <summary>
        ///     Gets or sets the target move speed.
        /// </summary>
        /// <value>
        ///     The target move speed.
        /// </value>
        public float TargetMoveSpeed { get; set; }

        /// <summary>
        ///     Gets or sets the target position.
        /// </summary>
        /// <value>
        ///     The target position.
        /// </value>
        public Vector TargetPosition { get; set; }

        /// <summary>
        ///     Gets or sets the waypoints.
        /// </summary>
        /// <value>
        ///     The waypoints.
        /// </value>
        public IEnumerable<Vector> Waypoints { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public float Width { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Converts this instance of <see cref="CupcakeIngredients" /> to a <see cref="CupcakeIngredientX" />
        /// </summary>
        /// <returns>The converted <see cref="CupcakeIngredientX" /></returns>
        internal CupcakeIngredientX ToXIngredient()
        {
            return new CupcakeIngredientX()
                       {
                           Delay = this.Delay, MissileSpeed = this.MissileSpeed, SourcePosition = this.SourcePosition, 
                           TargetMoveSpeed = this.TargetMoveSpeed, TargetPosition = this.TargetPosition, 
                           Width = this.Width, Waypoints = this.Waypoints
                       };
        }

        /// <summary>
        ///     Converts this instance of <see cref="CupcakeIngredients" /> to a <see cref="CupcakeIngredientY" />
        /// </summary>
        /// <returns>The converted <see cref="CupcakeIngredientY" /></returns>
        internal CupcakeIngredientY ToYIngredient()
        {
            return new CupcakeIngredientY()
                       {
                           Delay = this.Delay, MissileSpeed = this.MissileSpeed, SourcePosition = this.SourcePosition, 
                           TargetMoveSpeed = this.TargetMoveSpeed, TargetPosition = this.TargetPosition, 
                           Width = this.Width, Waypoints = this.Waypoints
                       };
        }

        #endregion
    }
}