// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HammingDistance.cs" company="ChewyMoon">
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
//   A hamming distance.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Metrics
{
    using System;

    using numl.Math.LinearAlgebra;

    /// <summary>A hamming distance.</summary>
    public sealed class HammingDistance : IDistance
    {
        #region Public Methods and Operators

        /// <summary>Computes.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The Vector to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>A double.</returns>
        public double Compute(Vector x, Vector y)
        {
            if (x.Length != y.Length)
            {
                throw new InvalidOperationException("Cannot compute distance between two unequally sized Vectors!");
            }

            double sum = 0;
            for (var i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                {
                    sum++;
                }
            }

            return sum;
        }

        #endregion
    }
}