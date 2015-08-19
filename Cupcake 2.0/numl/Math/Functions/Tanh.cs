// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Tanh.cs">
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
//   A hyperbolic tangent.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Functions
{
    /// <summary>A hyperbolic tangent.</summary>
    public class Tanh : Function
    {
        #region Public Methods and Operators

        /// <summary>Computes the given x coordinate.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <returns>A Vector.</returns>
        public override double Compute(double x)
        {
            return (this.exp(x) - this.exp(-x)) / (this.exp(x) + this.exp(-x));
        }

        /// <summary>Derivatives the given x coordinate.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <returns>A Vector.</returns>
        public override double Derivative(double x)
        {
            return 1 - this.pow(this.Compute(x), 2);
        }

        #endregion
    }
}