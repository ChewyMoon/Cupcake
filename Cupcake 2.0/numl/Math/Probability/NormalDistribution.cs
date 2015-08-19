// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="NormalDistribution.cs">
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
//   A normal distribution.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Probability
{
    using numl.Math.LinearAlgebra;

    /// <summary>A normal distribution.</summary>
    public class NormalDistribution
    {
        #region Public Properties

        /// <summary>Gets or sets the mu.</summary>
        /// <value>The mu.</value>
        public Vector Mu { get; set; }

        /// <summary>Gets or sets the sigma.</summary>
        /// <value>The sigma.</value>
        public Matrix Sigma { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Computes the given x coordinate.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <returns>A double.</returns>
        public double Compute(Vector x)
        {
            return 0;
        }

        /// <summary>Estimates.</summary>
        /// <param name="X">The Matrix to process.</param>
        /// <param name="type">(Optional) the type.</param>
        public void Estimate(Matrix X, VectorType type = VectorType.Row)
        {
            var n = type == VectorType.Row ? X.Rows : X.Cols;
            var s = type == VectorType.Row ? X.Cols : X.Rows;
            this.Mu = X.Sum(type) / n;
            this.Sigma = Matrix.Zeros(s);

            for (var i = 0; i < n; i++)
            {
                var x = X[i, type] - this.Mu;
                this.Sigma += x.Outer(x);
            }

            this.Sigma *= 1d / (n - 1d);
        }

        #endregion
    }
}