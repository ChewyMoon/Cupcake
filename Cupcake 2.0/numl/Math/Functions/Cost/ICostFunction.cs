// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICostFunction.cs" company="ChewyMoon">
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
//   Cost function interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Functions.Cost
{
    using numl.Math.Functions.Regularization;
    using numl.Math.LinearAlgebra;

    /// <summary>
    ///     Cost function interface
    /// </summary>
    public interface ICostFunction
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Computes the cost of the current theta parameters against the known Y labels
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="X"></param>
        /// <param name="y"></param>
        /// <param name="lambda">Regaularisation constant</param>
        /// <param name="regularizationTerm">Regularization method to apply</param>
        /// <returns></returns>
        double ComputeCost(Vector theta, Matrix X, Vector y, double lambda, IRegularizer regularizationTerm);

        /// <summary>
        ///     Computes the current gradient step direction towards the minima
        /// </summary>
        /// <param name="theta">Current theta parameters</param>
        /// <param name="X">Training set</param>
        /// <param name="y">Training labels</param>
        /// <param name="lambda">Regularisation constant</param>
        /// <param name="regularizationTerm">Regularization method to apply</param>
        /// <returns></returns>
        Vector ComputeGradient(Vector theta, Matrix X, Vector y, double lambda, IRegularizer regularizationTerm);

        #endregion
    }
}