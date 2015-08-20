// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IKernel.cs" company="ChewyMoon">
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
//   In machine learning there is something called the Kernel Trick. In essence it allows for the
//   mapping of observations in any general space into an inner product space (or Reproducing
//   Kernel Hilbert Space). This trick thereby creates (or one hopes) linear separability in the
//   augmented inner product space where simple linear classifiers perform extremely well.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Kernels
{
    using numl.Math.LinearAlgebra;

    /// <summary>
    ///     In machine learning there is something called the Kernel Trick. In essence it allows for the
    ///     mapping of observations in any general space into an inner product space (or Reproducing
    ///     Kernel Hilbert Space). This trick thereby creates (or one hopes) linear separability in the
    ///     augmented inner product space where simple linear classifiers perform extremely well.
    /// </summary>
    public interface IKernel
    {
        #region Public Methods and Operators

        /// <summary>Computes the Kernel Matrix using the given input.</summary>
        /// <param name="m">Input Matrix.</param>
        /// <returns>Kernel Matrix.</returns>
        Matrix Compute(Matrix m);

        /// <summary>Projects the vector <c>x</c> into the correspoding inner product space.</summary>
        /// <param name="m">Kernel Matrix.</param>
        /// <param name="x">Vector in original space.</param>
        /// <returns>Vector in inner product space.</returns>
        Vector Project(Matrix m, Vector x);

        #endregion
    }
}