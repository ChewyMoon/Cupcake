// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Function.cs">
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
//   A function.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Functions
{
    using System;

    using numl.Math.LinearAlgebra;

    /// <summary>A function.</summary>
    public abstract class Function : IFunction
    {
        #region Public Methods and Operators

        /// <summary>Computes the given x coordinate.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <returns>A Vector.</returns>
        public abstract double Compute(double x);

        /// <summary>Computes the given x coordinate.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <returns>A Vector.</returns>
        public Vector Compute(Vector x)
        {
            return x.Calc(d => this.Compute(d));
        }

        /// <summary>Derivatives the given x coordinate.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <returns>A Vector.</returns>
        public abstract double Derivative(double x);

        /// <summary>Derivatives the given x coordinate.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <returns>A Vector.</returns>
        public Vector Derivative(Vector x)
        {
            return x.Calc(d => this.Derivative(d));
        }

        #endregion

        #region Methods

        /// <summary>Exps.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <returns>A double.</returns>
        internal double exp(double x)
        {
            return Math.Exp(x);
        }

        /// <summary>Pows.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <param name="a">The double to process.</param>
        /// <returns>A double.</returns>
        internal double pow(double x, double a)
        {
            return Math.Pow(x, a);
        }

        #endregion
    }
}