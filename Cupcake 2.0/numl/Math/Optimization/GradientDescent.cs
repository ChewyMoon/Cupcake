// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GradientDescent.cs" company="ChewyMoon">
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
//   Gradient Descent
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Optimization
{
    using System;

    using numl.Math.Functions.Cost;
    using numl.Math.Functions.Regularization;
    using numl.Math.LinearAlgebra;

    /// <summary>
    ///     Gradient Descent
    /// </summary>
    public static class GradientDescent
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Performs gradient descent to optomise theta parameters.
        /// </summary>
        /// <param name="theta">Initial Theta (Zeros)</param>
        /// <param name="x">Training set</param>
        /// <param name="y">Training labels</param>
        /// <param name="maxIterations">Maximum number of iterations to run gradient descent</param>
        /// <param name="learningRateAlpha">The learning rate (Alpha)</param>
        /// <param name="costFunction">Cost function to use for gradient descent</param>
        /// <param name="lambda">The regularization constant to apply</param>
        /// <param name="regularizer">The regularization function to apply</param>
        /// <returns></returns>
        public static Tuple<double, Vector> Run(
            Vector theta, 
            Matrix x, 
            Vector y, 
            int maxIterations, 
            double learningRateAlpha, 
            ICostFunction costFunction, 
            double lambda, 
            IRegularizer regularizer)
        {
            var bestTheta = theta.Copy();
            var bestCost = double.PositiveInfinity;

            double currentCost = 0;
            var currentGradient = theta.Copy();

            for (var i = 0; i <= maxIterations; i++)
            {
                currentCost = costFunction.ComputeCost(bestTheta, x, y, lambda, regularizer);
                currentGradient = costFunction.ComputeGradient(bestTheta, x, y, lambda, regularizer);

                if (currentCost < bestCost)
                {
                    bestTheta = bestTheta - learningRateAlpha * currentGradient;
                    bestCost = currentCost;
                }
                else
                {
                    learningRateAlpha = learningRateAlpha * 0.99;
                }
            }

            return new Tuple<double, Vector>(bestCost, bestTheta);
        }

        #endregion
    }
}