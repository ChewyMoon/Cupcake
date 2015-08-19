// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableHelpers.cs" company="ChewyMoon">
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
//   Extension methods for IEnumerable collections
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Extension methods for IEnumerable collections
    /// </summary>
    public static class EnumerableHelpers
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Calculates the standard deviation on the source collection
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="fnPropSelector"></param>
        /// <param name="isSamplePopulation"></param>
        /// <returns></returns>
        public static double StandardDeviation<TSource>(
            this IEnumerable<TSource> source, 
            Func<TSource, double> fnPropSelector, 
            bool isSamplePopulation = false)
        {
            return Math.Sqrt(Variance(source, fnPropSelector, isSamplePopulation));
        }

        /// <summary>
        ///     Calculates the variance on the source collection
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="fnPropSelector"></param>
        /// <param name="isSamplePopulation"></param>
        /// <returns></returns>
        public static double Variance<TSource>(
            this IEnumerable<TSource> source, 
            Func<TSource, double> fnPropSelector, 
            bool isSamplePopulation = false)
        {
            return source.Select(s => Math.Pow(fnPropSelector(s) - source.Average(fnPropSelector), 2)).Sum()
                   / (isSamplePopulation ? source.Count() - 1 : source.Count());
        }

        #endregion
    }
}