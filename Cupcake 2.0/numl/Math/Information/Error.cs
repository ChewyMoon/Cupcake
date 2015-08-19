// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Error.cs">
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
//   This class calculates the Classification Error of any given vector. It inherits from
//   <see cref="Impurity" /> class which provides additional functionality.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Information
{
    using System;
    using System.Linq;

    using numl.Math.LinearAlgebra;

    /// <summary>
    ///     This class calculates the Classification Error of any given vector. It inherits from
    ///     <see cref="Impurity" /> class which provides additional functionality.
    /// </summary>
    public class Error : Impurity
    {
        #region Public Methods and Operators

        /// <summary>Calculates Classification Error of x.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The list in question.</param>
        /// <returns>Impurity measure.</returns>
        public override double Calculate(Vector x)
        {
            if (x == null)
            {
                throw new InvalidOperationException("x does not exist!");
            }

            double length = x.Count();

            var e = from i in x.Distinct() let q = (from j in x where j == i select j).Count() select q / length;

            return 1 - e.Max();
        }

        #endregion
    }
}