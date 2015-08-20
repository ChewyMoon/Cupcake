// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGenerator.cs" company="ChewyMoon">
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
//   Interface for generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised
{
    using System.Collections.Generic;

    using numl.Math.LinearAlgebra;
    using numl.Model;

    /// <summary>Interface for generator.</summary>
    public interface IGenerator
    {
        #region Public Properties

        /// <summary>Gets or sets the descriptor.</summary>
        /// <value>The descriptor.</value>
        Descriptor Descriptor { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generates.</summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="examples">The examples.</param>
        /// <returns>An IModel.</returns>
        IModel Generate(Descriptor descriptor, IEnumerable<object> examples);

        /// <summary>Generates the given examples.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="examples">The examples.</param>
        /// <returns>An IModel.</returns>
        IModel Generate<T>(IEnumerable<T> examples) where T : class;

        /// <summary>Generates.</summary>
        /// <param name="x">The Matrix to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>An IModel.</returns>
        IModel Generate(Matrix x, Vector y);

        #endregion
    }
}