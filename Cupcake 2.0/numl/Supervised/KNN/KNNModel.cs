// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="KNNModel.cs">
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
//   A data Model for the knn.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.KNN
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using numl.Math.LinearAlgebra;
    using numl.Model;
    using numl.Utils;

    /// <summary>A data Model for the knn.</summary>
    [Serializable]
    public class KNNModel : Model
    {
        #region Public Properties

        /// <summary>Gets or sets the k.</summary>
        /// <value>The k.</value>
        public int K { get; set; }

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
            var distances = new Tuple<int, double>[this.X.Rows];

            // happens per slot so we are good to parallelize
            Parallel.For(0, this.X.Rows, i => distances[i] = new Tuple<int, double>(i, (y - this.X.Row(i)).Norm(2)));

            var slice = distances.OrderBy(t => t.Item2).Take(this.K).Select(i => i.Item1);

            return this.Y.Slice(slice).Mode();
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            this.K = int.Parse(reader.GetAttribute("K"));
            reader.ReadStartElement();

            this.Descriptor = Xml.Read<Descriptor>(reader);
            this.X = Xml.Read<Matrix>(reader);
            this.Y = Xml.Read<Vector>(reader);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("K", this.K.ToString("r"));
            Xml.Write<Descriptor>(writer, this.Descriptor);
            Xml.Write<Matrix>(writer, this.X);
            Xml.Write<Vector>(writer, this.Y);
        }

        #endregion
    }
}