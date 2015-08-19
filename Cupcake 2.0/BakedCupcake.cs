// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BakedCupcake.cs" company="ChewyMoon">
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
//   Represents the output of the prediction.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CupcakePrediction
{
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     Represents the output of the prediction.
    /// </summary>
    public class BakedCupcake
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BakedCupcake" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        internal BakedCupcake(float x, float y)
        {
            this.CastPosition = new Vector2(x, y).To3D2();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the cast position.
        /// </summary>
        /// <value>
        ///     The cast position.
        /// </value>
        public Vector3 CastPosition { get; }

        #endregion
    }
}