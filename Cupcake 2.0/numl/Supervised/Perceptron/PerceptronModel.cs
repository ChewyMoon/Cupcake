// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerceptronModel.cs" company="ChewyMoon">
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
//   A data Model for the perceptron.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.Perceptron
{
    using System.Xml;

    using numl.Math.LinearAlgebra;
    using numl.Model;
    using numl.Utils;

    /// <summary>A data Model for the perceptron.</summary>
    public class PerceptronModel : Model
    {
        #region Public Properties

        /// <summary>Gets or sets the b.</summary>
        /// <value>The b.</value>
        public double B { get; set; }

        /// <summary>Gets or sets a value indicating whether the normalized.</summary>
        /// <value>true if normalized, false if not.</value>
        public bool Normalized { get; set; }

        /// <summary>Gets or sets the w.</summary>
        /// <value>The w.</value>
        public Vector W { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Predicts the given o.</summary>
        /// <param name="y">The Vector to process.</param>
        /// <returns>An object.</returns>
        public override double Predict(Vector y)
        {
            if (this.Normalized)
            {
                y = y / y.Norm();
            }

            return this.W.Dot(y) + this.B;
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            this.B = double.Parse(reader.GetAttribute("B"));
            this.Normalized = bool.Parse(reader.GetAttribute("Normalized"));
            reader.ReadStartElement();

            this.Descriptor = Xml.Read<Descriptor>(reader);
            this.W = Xml.Read<Vector>(reader);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("B", this.B.ToString("r"));
            writer.WriteAttributeString("Normalized", this.Normalized.ToString());

            Xml.Write<Descriptor>(writer, this.Descriptor);
            Xml.Write<Vector>(writer, this.W);
        }

        #endregion
    }
}