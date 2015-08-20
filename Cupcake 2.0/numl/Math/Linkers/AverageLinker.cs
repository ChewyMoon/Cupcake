// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AverageLinker.cs" company="ChewyMoon">
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
//   An average linker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Linkers
{
    using System.Collections.Generic;
    using System.Linq;

    using numl.Math.LinearAlgebra;
    using numl.Math.Metrics;

    /// <summary>An average linker.</summary>
    public class AverageLinker : ILinker
    {
        #region Fields

        /// <summary>The metric.</summary>
        private IDistance _metric;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AverageLinker" /> class. Constructor.
        /// </summary>
        /// <param name="metric">
        ///     The metric.
        /// </param>
        public AverageLinker(IDistance metric)
        {
            this._metric = metric;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Distances.</summary>
        /// <param name="x">The IEnumerable&lt;Vector&gt; to process.</param>
        /// <param name="y">The IEnumerable&lt;Vector&gt; to process.</param>
        /// <returns>A double.</returns>
        public double Distance(IEnumerable<Vector> x, IEnumerable<Vector> y)
        {
            double distanceSum = 0;

            var xCount = x.Count();
            var yCount = y.Count();
            for (var i = 0; i < xCount; i++)
            {
                for (var j = i + 1; j < yCount; j++)
                {
                    distanceSum += this._metric.Compute(x.ElementAt(i), y.ElementAt(j));
                }
            }

            return distanceSum / (double)(xCount * yCount);
        }

        #endregion
    }
}