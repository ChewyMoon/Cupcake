// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureDimensions.cs" company="ChewyMoon">
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
//   Feature Dimensions class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.PreProcessing
{
    using System;

    using numl.Math.LinearAlgebra;

    /// <summary>
    ///     Feature Dimensions class
    /// </summary>
    public static class FeatureDimensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Adds a specified number of polynomial features to the training / test Vector.
        /// </summary>
        /// <param name="x">Training / Testing record</param>
        /// <param name="polynomialFeatures">Number of polynomial features to add</param>
        /// <returns></returns>
        public static Vector IncreaseDimensions(Vector x, int polynomialFeatures)
        {
            var xtemp = x.Copy();
            var maxCols = xtemp.Length;
            for (var j = 0; j < maxCols - 1; j++)
            {
                for (var k = 0; k <= polynomialFeatures; k++)
                {
                    for (var m = 0; m <= k; m++)
                    {
                        var v = Math.Pow(xtemp[j], (double)(k - m)) * Math.Pow(xtemp[j + 1], (double)m);
                        xtemp = xtemp.Insert(xtemp.Length - 1, v);
                    }
                }
            }

            return xtemp;
        }

        /// <summary>
        ///     Adds a specified number of polynomial features to the training set Matrix.
        /// </summary>
        /// <param name="x">Training set</param>
        /// <param name="polynomialFeatures">Number of polynomial features to add</param>
        /// <returns></returns>
        public static Matrix IncreaseDimensions(Matrix x, int polynomialFeatures)
        {
            var Xtemp = x.Copy();
            var maxCols = Xtemp.Cols;
            for (var j = 0; j < maxCols - 1; j++)
            {
                for (var k = 0; k <= polynomialFeatures; k++)
                {
                    for (var m = 0; m <= k; m++)
                    {
                        var v = (Xtemp[j, VectorType.Col].ToVector() ^ (double)(k - m))
                                * (Xtemp[j + 1, VectorType.Col] ^ (double)m).ToVector();
                        Xtemp = Xtemp.Insert(v, Xtemp.Cols - 1, VectorType.Col);
                    }
                }
            }

            return Xtemp;
        }

        #endregion
    }
}