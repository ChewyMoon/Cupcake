// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KNNGenerator.cs" company="ChewyMoon">
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
//   A knn generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.KNN
{
    using numl.Math.LinearAlgebra;

    /// <summary>A knn generator.</summary>
    public class KNNGenerator : Generator
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="KNNGenerator" /> class. Constructor.
        /// </summary>
        /// <param name="k">
        ///     (Optional) the int to process.
        /// </param>
        public KNNGenerator(int k = 5)
        {
            this.K = k;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the k.</summary>
        /// <value>The k.</value>
        public int K { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generate model based on a set of examples.</summary>
        /// <param name="x">The Matrix to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>Model.</returns>
        public override IModel Generate(Matrix x, Vector y)
        {
            return new KNNModel { Descriptor = this.Descriptor, X = x, Y = y, K = this.K };
        }

        #endregion
    }
}