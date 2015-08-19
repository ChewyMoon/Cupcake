// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="NeuralNetworkModel.cs">
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
//   A data Model for the neural network.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.NeuralNetwork
{
    using System;
    using System.Linq;
    using System.Xml;

    using numl.Math.LinearAlgebra;

    /// <summary>A data Model for the neural network.</summary>
    public class NeuralNetworkModel : Model
    {
        #region Public Properties

        /// <summary>Gets or sets the network.</summary>
        /// <value>The network.</value>
        public Network Network { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Predicts the given o.</summary>
        /// <param name="y">The Vector to process.</param>
        /// <returns>An object.</returns>
        public override double Predict(Vector y)
        {
            this.Network.Forward(y);
            return this.Network.Out.Select(n => n.Output).Max();
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}