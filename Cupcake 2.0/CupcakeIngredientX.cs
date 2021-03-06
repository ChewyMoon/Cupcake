﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CupcakeIngredientX.cs" company="ChewyMoon">
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
//   The X coordinate of the prediction.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CupcakePrediction
{
    using System.Collections.Generic;

    using numl.Math.LinearAlgebra;
    using numl.Model;

    /// <summary>
    ///     The X coordinate of the prediction.
    /// </summary>
    public class CupcakeIngredientX
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the baked x.
        /// </summary>
        /// <value>
        ///     The baked x.
        /// </value>
        [Label]
        public float BakedX { get; set; }

        /// <summary>
        ///     Gets or sets the delay.
        /// </summary>
        /// <value>
        ///     The delay.
        /// </value>
        [Feature]
        public float Delay { get; set; }

        /// <summary>
        ///     Gets or sets the missile speed.
        /// </summary>
        /// <value>
        ///     The missile speed.
        /// </value>
        [Feature]
        public float MissileSpeed { get; set; }

        /// <summary>
        ///     Gets or sets the source position.
        /// </summary>
        /// <value>
        ///     The source position.
        /// </value>
        [Feature]
        public Vector SourcePosition { get; set; }

        /// <summary>
        ///     Gets or sets the target move speed.
        /// </summary>
        /// <value>
        ///     The target move speed.
        /// </value>
        [Feature]
        public float TargetMoveSpeed { get; set; }

        /// <summary>
        ///     Gets or sets the target position.
        /// </summary>
        /// <value>
        ///     The target position.
        /// </value>
        [Feature]
        public Vector TargetPosition { get; set; }

        /// <summary>
        ///     Gets or sets the waypoints.
        /// </summary>
        /// <value>
        ///     The waypoints.
        /// </value>
        [EnumerableFeature(2)]
        public IEnumerable<Vector> Waypoints { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        [Feature]
        public float Width { get; set; }

        #endregion
    }
}