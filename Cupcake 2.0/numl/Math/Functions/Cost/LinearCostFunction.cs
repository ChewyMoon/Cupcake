﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearCostFunction.cs" company="ChewyMoon">
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
//   A Linear Cost Function.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Functions.Cost
{
    using numl.Math.Functions.Regularization;
    using numl.Math.LinearAlgebra;

    /// <summary>
    ///     A Linear Cost Function.
    /// </summary>
    public class LinearCostFunction : ICostFunction
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Compute the error cost of the given Theta parameter for the training and label sets
        /// </summary>
        /// <param name="theta">Learning Theta parameters</param>
        /// <param name="X">Training set</param>
        /// <param name="y">Training labels</param>
        /// <param name="lambda">Regularization constant</param>
        /// <param name="regularizer">Regularization term function.</param>
        /// <returns></returns>
        public double ComputeCost(Vector theta, Matrix X, Vector y, double lambda, IRegularizer regularizer)
        {
            var m = X.Rows;

            var j = 0.0;

            var s = (X * theta).ToVector();

            j = 1.0 / (2.0 * m) * ((s - y) ^ 2.0).Sum();

            if (lambda != 0)
            {
                j = regularizer.Regularize(j, theta, m, lambda);
            }

            return j;
        }

        /// <summary>
        ///     Compute the error cost of the given Theta parameter for the training and label sets
        /// </summary>
        /// <param name="theta">Learning Theta parameters</param>
        /// <param name="X">Training set</param>
        /// <param name="y">Training labels</param>
        /// <param name="lambda">Regularisation constant</param>
        /// <param name="regularizer">Regularization term function.</param>
        /// <returns></returns>
        public Vector ComputeGradient(Vector theta, Matrix X, Vector y, double lambda, IRegularizer regularizer)
        {
            var m = X.Rows;
            var gradient = Vector.Zeros(theta.Length);

            var s = (X * theta).ToVector();

            for (var i = 0; i < theta.Length; i++)
            {
                gradient[i] = 1.0 / m * ((s - y) * X[i, VectorType.Col]).Sum();
            }

            if (lambda != 0)
            {
                gradient = regularizer.Regularize(theta, gradient, m, lambda);
            }

            return gradient;
        }

        #endregion
    }
}