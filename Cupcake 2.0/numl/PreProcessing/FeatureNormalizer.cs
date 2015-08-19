// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureNormalizer.cs" company="ChewyMoon">
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
//   Feature Normalisation extension methods
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.PreProcessing
{
    using System;
    using System.Linq;

    using numl.Math.LinearAlgebra;
    using numl.Utils;

    /// <summary>
    ///     Feature Normalisation extension methods
    /// </summary>
    public static class FeatureNormalizer
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Performs feature scaling on the supplied value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="avg">The feature average</param>
        /// <param name="std">The standard deviation of the feature</param>
        /// <returns></returns>
        public static double FeatureScale(double value, double avg, double std)
        {
            return (value - avg) / std;
        }

        /// <summary>
        ///     Performs feature scaling on the supplied array and returns a copy
        /// </summary>
        /// <param name="column">Column array to compute</param>
        /// <returns></returns>
        public static double[] FeatureScale(double[] column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("Column was null");
            }

            var result = new double[column.Length];

            var avg = column.Average();
            var sdv = column.StandardDeviation(c => c, false);
            for (var x = 0; x < column.Length; x++)
            {
                result[x] = FeatureScale(column[x], avg, sdv);
            }

            return result;
        }

        /// <summary>
        ///     Performs feature scaling on the supplied column vector and returns a copy
        /// </summary>
        /// <param name="column">Column vector to compute</param>
        /// <returns></returns>
        public static Vector FeatureScale(Vector column)
        {
            var temp = column.ToArray();
            return new Vector(FeatureScale(temp));
        }

        #endregion
    }
}