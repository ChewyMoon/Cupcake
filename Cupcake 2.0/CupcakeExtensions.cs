// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CupcakeExtensions.cs" company="ChewyMoon">
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
//   Provides extensions needed for SharpDX -&gt; Numl and vice versa.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CupcakePrediction
{
    using System.Diagnostics.CodeAnalysis;

    using numl.Math.LinearAlgebra;

    using SharpDX;

    /// <summary>
    ///     Provides extensions needed for SharpDX -> Numl and vice versa.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", 
        Justification = "Reviewed. Suppression is OK here.")]
    internal static class CupcakeExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Converts a <see cref="Vector2" /> to a <see cref="Vector" />.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The converted <see cref="Vector" /></returns>
        public static Vector ToNumlVector(this Vector2 position)
        {
            return new Vector(new double[] { position.X, position.Y });
        }

        #endregion
    }
}