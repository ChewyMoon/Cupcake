// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KernelPerceptronModel.cs" company="ChewyMoon">
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
//   A data Model for the kernel perceptron.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.Perceptron
{
    using System;
    using System.Xml;

    using numl.Math.Kernels;
    using numl.Math.LinearAlgebra;
    using numl.Model;
    using numl.Utils;

    /// <summary>A data Model for the kernel perceptron.</summary>
    [Serializable]
    public class KernelPerceptronModel : Model
    {
        #region Public Properties

        /// <summary>Gets or sets a.</summary>
        /// <value>a.</value>
        public Vector A { get; set; }

        /// <summary>Gets or sets the kernel.</summary>
        /// <value>The kernel.</value>
        public IKernel Kernel { get; set; }

        /// <summary>Gets or sets the x coordinate.</summary>
        /// <value>The x coordinate.</value>
        public Matrix X { get; set; }

        /// <summary>Gets or sets the y coordinate.</summary>
        /// <value>The y coordinate.</value>
        public Vector Y { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Predicts the given o.</summary>
        /// <param name="y">The Vector to process.</param>
        /// <returns>An object.</returns>
        public override double Predict(Vector y)
        {
            var K = this.Kernel.Project(this.X, y);
            double v = 0;
            for (var i = 0; i < this.A.Length; i++)
            {
                v += this.A[i] * this.Y[i] * K[i];
            }

            return v;
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var type = Ject.FindType(reader.GetAttribute("Kernel"));
            this.Kernel = (IKernel)Activator.CreateInstance(type);
            reader.ReadStartElement();

            this.Descriptor = Xml.Read<Descriptor>(reader);
            this.Y = Xml.Read<Vector>(reader);
            this.A = Xml.Read<Vector>(reader);
            this.X = Xml.Read<Matrix>(reader);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Kernel", this.Kernel.GetType().Name);
            Xml.Write<Descriptor>(writer, this.Descriptor);
            Xml.Write<Vector>(writer, this.Y);
            Xml.Write<Vector>(writer, this.A);
            Xml.Write<Matrix>(writer, this.X);
        }

        #endregion
    }
}