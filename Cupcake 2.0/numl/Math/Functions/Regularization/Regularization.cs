// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Regularization.cs" company="ChewyMoon">
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
//   Standard regularization
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Functions.Regularization
{
    using System.Linq;

    using numl.Math.LinearAlgebra;

    /// <summary>
    ///     Standard regularization
    /// </summary>
    public class Regularization : IRegularizer
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Applies regularization to the current cost
        /// </summary>
        /// <param name="j">Current cost</param>
        /// <param name="theta">Current theta</param>
        /// <param name="m">Training records</param>
        /// <param name="lambda">Regularization constant</param>
        /// <returns></returns>
        public double Regularize(double j, Vector theta, int m, double lambda)
        {
            if (lambda != 0)
            {
                j = ((lambda / (2 * m)) * (new Vector(theta.Skip(1).ToArray()) ^ 2).Sum()) + j;
            }

            return j;
        }

        /// <summary>
        ///     Applies regularization to the current gradient
        /// </summary>
        /// <param name="theta">Current theta</param>
        /// <param name="gradient">Current gradient</param>
        /// <param name="m">Training records</param>
        /// <param name="lambda">Regularization constant</param>
        public Vector Regularize(Vector theta, Vector gradient, int m, double lambda)
        {
            if (lambda != 0)
            {
                for (var i = 1; i < theta.Length; i++)
                {
                    gradient[i] = gradient[i] + ((lambda / m) * theta[i]);
                }
            }

            return gradient;
        }

        #endregion
    }
}