// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Node.cs" company="ChewyMoon">
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
//   A node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.DecisionTree
{
    using System.Xml.Serialization;

    /// <summary>A node.</summary>
    [XmlRoot("Node")]
    public class Node
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Node" /> class. Default constructor.
        /// </summary>
        public Node()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the column.</summary>
        /// <value>The column.</value>
        [XmlAttribute("Column")]
        public int Column { get; set; }

        /// <summary>Gets or sets the edges.</summary>
        /// <value>The edges.</value>
        [XmlArray("Edges")]
        public Edge[] Edges { get; set; }

        /// <summary>Gets or sets the gain.</summary>
        /// <value>The gain.</value>
        [XmlAttribute("Gain")]
        public double Gain { get; set; }

        /// <summary>if is a leaf.</summary>
        /// <value>true if this object is leaf, false if not.</value>
        [XmlAttribute("Leaf")]
        public bool IsLeaf { get; set; }

        /// <summary>Gets or sets the label.</summary>
        /// <value>The label.</value>
        [XmlIgnore]
        public object Label { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
        [XmlAttribute("Value")]
        public double Value { get; set; }

        #endregion
    }

    /// <summary>An edge.</summary>
    [XmlRoot("Edge")]
    public class Edge
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Edge" /> class. Default constructor.
        /// </summary>
        public Edge()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the child.</summary>
        /// <value>The child.</value>
        [XmlElement("Child")]
        public Node Child { get; set; }

        /// <summary>Gets or sets a value indicating whether the discrete.</summary>
        /// <value>true if discrete, false if not.</value>
        [XmlAttribute("Discrete")]
        public bool Discrete { get; set; }

        /// <summary>Gets or sets the label.</summary>
        /// <value>The label.</value>
        [XmlAttribute("Label")]
        public string Label { get; set; }

        /// <summary>Gets or sets the maximum.</summary>
        /// <value>The maximum value.</value>
        [XmlAttribute("Max")]
        public double Max { get; set; }

        /// <summary>Gets or sets the minimum.</summary>
        /// <value>The minimum value.</value>
        [XmlAttribute("Min")]
        public double Min { get; set; }

        /// <summary>Gets or sets the parent.</summary>
        /// <value>The parent.</value>
        [XmlIgnore]
        public Node Parent { get; set; }

        #endregion
    }
}