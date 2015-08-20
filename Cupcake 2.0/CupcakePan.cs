// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CupcakePan.cs" company="ChewyMoon">
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
//   Holds a representation of training data. This data is deserialized from JSON.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CupcakePrediction
{
    using System.Collections.Generic;

    /// <summary>
    ///     Holds a representation of training data. This data is deserialized from JSON.
    /// </summary>
    public class CupcakePan
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CupcakePan" /> class.
        /// </summary>
        public CupcakePan()
        {
            this.X = new List<CupcakeIngredientX>();
            this.Y = new List<CupcakeIngredientY>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the x.
        /// </summary>
        /// <value>
        ///     The x.
        /// </value>
        public List<CupcakeIngredientX> X { get; set; }

        /// <summary>
        ///     Gets or sets the y.
        /// </summary>
        /// <value>
        ///     The y.
        /// </value>
        public List<CupcakeIngredientY> Y { get; set; }

        #endregion
    }
}