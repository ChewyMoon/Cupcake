// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="RBFKernel.cs">
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
//   The Radial Basis Function (RBF) Kernel is a projection into infinite dimensional space and
//   acts as a pseudo similarity measure in the projected inner product space. It is governed by
//   exp(||x - x'||2 / 2sigm^2)
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Kernels
{
    using System;

    using numl.Math.LinearAlgebra;

    /// <summary>
    ///     The Radial Basis Function (RBF) Kernel is a projection into infinite dimensional space and
    ///     acts as a pseudo similarity measure in the projected inner product space. It is governed by
    ///     exp(||x - x'||2 / 2sigm^2)
    /// </summary>
    public class RBFKernel : IKernel
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RBFKernel" /> class. ctor.
        /// </summary>
        /// <param name="sigma">
        ///     Input Parameter.
        /// </param>
        public RBFKernel(double sigma)
        {
            this.Sigma = sigma;
        }

        #endregion

        #region Public Properties

        /// <summary>RBF free parameter.</summary>
        /// <value>The sigma.</value>
        public double Sigma { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Computes RBF Kernel with provided free sigma parameter.</summary>
        /// <param name="m">Input Matrix.</param>
        /// <returns>RBF Kernel Matrix.</returns>
        public Matrix Compute(Matrix m)
        {
            var K = Matrix.Zeros(m.Rows);

            // by definition a kernel matrix is symmetric;
            // therefore we can cut calculations in half
            for (var i = 0; i < m.Rows; i++)
            {
                for (var j = i; j < m.Rows; j++)
                {
                    var p = m[i] - m[j];
                    var xy = -1 * p.Dot(p);
                    K[i, j] = K[j, i] = Math.Exp(xy / (2 * Math.Pow(this.Sigma, 2)));
                }
            }

            return K;
        }

        /// <summary>Projects vector into rbf kernel space.</summary>
        /// <param name="m">RBF Kernel Matrix.</param>
        /// <param name="x">Vector in original space.</param>
        /// <returns>Vector in RBF kernel space.</returns>
        public Vector Project(Matrix m, Vector x)
        {
            var K = Vector.Zeros(m.Rows);

            for (var i = 0; i < K.Length; i++)
            {
                var p = m[i] - x;
                var xy = -1 * p.Dot(p);
                K[i] = Math.Exp(xy / (2 * Math.Pow(this.Sigma, 2)));
            }

            return K;
        }

        #endregion
    }
}