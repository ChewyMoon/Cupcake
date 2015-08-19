// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="TanimotoCoefficient.cs">
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
//   A tanimoto coefficient.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Metrics
{
    using System;

    using numl.Math.LinearAlgebra;

    /// <summary>A tanimoto coefficient.</summary>
    public sealed class TanimotoCoefficient : ISimilarity
    {
        #region Public Methods and Operators

        /// <summary>Computes.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>A double.</returns>
        public double Compute(Vector x, Vector y)
        {
            var dot = x.Dot(y);
            return dot / (Math.Pow(x.Norm(), 2) + Math.Pow(y.Norm(), 2) - dot);
        }

        #endregion
    }
}