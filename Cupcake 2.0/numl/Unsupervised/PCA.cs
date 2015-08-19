// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="PCA.cs">
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
//   A pca.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Unsupervised
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using numl.Math.LinearAlgebra;
    using numl.Model;

    /// <summary>A pca.</summary>
    public class PCA
    {
        #region Public Properties

        /// <summary>Gets or sets the eigenvalues.</summary>
        /// <value>The eigenvalues.</value>
        public Vector Eigenvalues { get; private set; }

        /// <summary>Gets or sets the eigenvectors.</summary>
        /// <value>The eigenvectors.</value>
        public Matrix Eigenvectors { get; private set; }

        /// <summary>Gets or sets the reduced.</summary>
        /// <value>The reduced.</value>
        public Matrix Reduced { get; private set; }

        /// <summary>Gets or sets the x coordinate.</summary>
        /// <value>The x coordinate.</value>
        public Matrix X { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generates.</summary>
        /// <param name="matrix">The matrix.</param>
        public void Generate(Matrix matrix)
        {
            // generate centered matrix
            // (using a copy since centering is in place)
            this.X = matrix.Copy().Center(VectorType.Col);

            // compute eigen-decomposition
            // of covariance matrix
            var eigs = this.X.Covariance().Eigs();
            this.Eigenvalues = eigs.Item1;
            this.Eigenvectors = eigs.Item2;
        }

        /// <summary>Generates.</summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="examples">The examples.</param>
        public void Generate(Descriptor descriptor, IEnumerable<object> examples)
        {
            // generate data matrix
            var x = descriptor.Convert(examples).ToMatrix();
            this.Generate(x);
        }

        /// <summary>Reduces.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="maxdim">The maxdim.</param>
        /// <returns>A Matrix.</returns>
        public Matrix Reduce(int maxdim)
        {
            if (maxdim < 1)
            {
                throw new InvalidOperationException("Cannot reduce to less than 1 dimension!");
            }

            if (this.X == null || this.Eigenvalues == null || this.Eigenvectors == null)
            {
                throw new InvalidOperationException("Cannot reduce until pca data has been generated");
            }

            // get columns in reverse order
            // and stuff into matrix
            var reduc = this.Eigenvectors.GetCols().Take(maxdim).ToMatrix();

            this.Reduced = reduc * this.X.T;

            return this.Reduced;
        }

        #endregion
    }
}