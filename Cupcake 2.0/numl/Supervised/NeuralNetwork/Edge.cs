// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Edge.cs">
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
//   An edge.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.NeuralNetwork
{
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using numl.Math.Probability;

    /// <summary>An edge.</summary>
    [XmlRoot("Edge")]
    public class Edge : IXmlSerializable
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Edge" /> class. Default constructor.
        /// </summary>
        public Edge()
        {
            // random initialization
            // R. D. Reed and R. J. Marks II, "Neural Smithing: 
            // Supervised Learning in Feedforward Artificial 
            // Neural Networks", Mit Press, 1999. pg 57
            // selecting values from range [-a,+a] where 0.1 < a < 2
            this.Weight = (double)Sampling.GetUniform(1, 20) / 10d;
            if (Sampling.GetUniform() < .5)
            {
                this.Weight *= -1;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the source for the.</summary>
        /// <value>The source.</value>
        public Node Source { get; set; }

        /// <summary>Gets or sets the Target for the.</summary>
        /// <value>The target.</value>
        public Node Target { get; set; }

        /// <summary>Gets or sets the weight.</summary>
        /// <value>The weight.</value>
        public double Weight { get; set; }

        #endregion

        #region Properties

        /// <summary>Gets or sets the identifier of the source.</summary>
        /// <value>The identifier of the source.</value>
        internal string SourceId { get; set; }

        /// <summary>Gets or sets the identifier of the target.</summary>
        /// <value>The identifier of the target.</value>
        internal string TargetId { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Creates a new Edge.</summary>
        /// <param name="source">Source for the.</param>
        /// <param name="target">Target for the.</param>
        /// <returns>An Edge.</returns>
        public static Edge Create(Node source, Node target)
        {
            var e = new Edge { Source = source, Target = target };
            source.Out.Add(e);
            target.In.Add(e);
            return e;
        }

        /// <summary>
        ///     This method is reserved and should not be used. When implementing the IXmlSerializable
        ///     interface, you should return null (Nothing in Visual Basic) from this method, and instead, if
        ///     specifying a custom schema is required, apply the
        ///     <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the
        ///     object that is produced by the
        ///     <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" />
        ///     method and consumed by the
        ///     <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" />
        ///     method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            this.SourceId = reader.GetAttribute("Source");
            this.TargetId = reader.GetAttribute("Target");
            this.Weight = double.Parse(reader.GetAttribute("Weight"));
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("{0} ---- {1} ----> {2}", this.Source, this.Weight, this.Target);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Source", this.Source.Id);
            writer.WriteAttributeString("Target", this.Target.Id);
            writer.WriteAttributeString("Weight", this.Weight.ToString("r"));
        }

        #endregion
    }
}