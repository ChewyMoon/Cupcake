// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="IModel.cs">
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
//   Interface for model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised
{
    using System.IO;

    using numl.Math.LinearAlgebra;
    using numl.Model;

    /// <summary>Interface for model.</summary>
    public interface IModel
    {
        #region Public Properties

        /// <summary>Gets or sets the descriptor.</summary>
        /// <value>The descriptor.</value>
        Descriptor Descriptor { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Loads the given stream.</summary>
        /// <param name="file">The file to load.</param>
        /// <returns>An IModel.</returns>
        IModel Load(string file);

        /// <summary>Loads the given stream.</summary>
        /// <param name="stream">The stream to load.</param>
        /// <returns>An IModel.</returns>
        IModel Load(Stream stream);

        /// <summary>Predicts the given o.</summary>
        /// <param name="y">The Vector to process.</param>
        /// <returns>An object.</returns>
        double Predict(Vector y);

        /// <summary>Predicts the given o.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="o">The object to process.</param>
        /// <returns>A T.</returns>
        T Predict<T>(T o);

        /// <summary>Predicts the given o.</summary>
        /// <param name="o">The object to process.</param>
        /// <returns>An object.</returns>
        object Predict(object o);

        /// <summary>Model persistance.</summary>
        /// <param name="file">The file to load.</param>
        void Save(string file);

        /// <summary>Saves the given stream.</summary>
        /// <param name="stream">The stream to load.</param>
        void Save(Stream stream);

        #endregion
    }
}