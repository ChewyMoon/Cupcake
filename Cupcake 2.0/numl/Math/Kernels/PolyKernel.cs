// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PolyKernel.cs" company="ChewyMoon">
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
//   Polynomial kernel of arbitrary dimension.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Kernels
{
    using System;

    using numl.Math.LinearAlgebra;

    /// <summary>Polynomial kernel of arbitrary dimension.</summary>
    public class PolyKernel : IKernel
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PolyKernel" /> class. ctor.
        /// </summary>
        /// <param name="dimension">
        ///     Polynomial Kernel Dimension.
        /// </param>
        public PolyKernel(double dimension)
        {
            this.Dimension = dimension;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Specifies dimensionality of projection based on (1 + x.T y)^d where d is the dimension.
        /// </summary>
        /// <value>The dimension.</value>
        public double Dimension { get; internal set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Computes polynomial kernel of the specified degree (in Dimension)</summary>
        /// <param name="m">Input Matrix.</param>
        /// <returns>Kernel Matrix.</returns>
        public Matrix Compute(Matrix m)
        {
            // by definition a kernel matrix is symmetric;
            // therefore we can cut calculations in half
            var K = Matrix.Zeros(m.Rows);
            for (var i = 0; i < m.Rows; i++)
            {
                for (var j = i; j < m.Rows; j++)
                {
                    K[i, j] = K[j, i] = Math.Pow(1 + m[i].Dot(m[j]), this.Dimension);
                }
            }

            return K;
        }

        /// <summary>Projects vector into polynomial kernel space.</summary>
        /// <param name="m">Polynomial Kernel Matrix.</param>
        /// <param name="x">Vector in original space.</param>
        /// <returns>Vector in polynomial kernel space.</returns>
        public Vector Project(Matrix m, Vector x)
        {
            var K = Vector.Zeros(m.Rows);
            for (var i = 0; i < K.Length; i++)
            {
                K[i] = Math.Pow(1 + m[i].Dot(x), this.Dimension);
            }

            return K;
        }

        #endregion
    }
}