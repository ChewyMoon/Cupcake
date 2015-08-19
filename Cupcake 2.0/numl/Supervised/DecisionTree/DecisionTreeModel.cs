// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="DecisionTreeModel.cs">
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
//   A data Model for the decision tree.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.DecisionTree
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    using numl.Math.LinearAlgebra;
    using numl.Model;
    using numl.Utils;

    /// <summary>A data Model for the decision tree.</summary>
    public class DecisionTreeModel : Model
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DecisionTreeModel" /> class. Default constructor.
        /// </summary>
        public DecisionTreeModel()
        {
            // no hint
            this.Hint = double.Epsilon;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the hint.</summary>
        /// <value>The hint.</value>
        public double Hint { get; set; }

        /// <summary>Gets or sets the tree.</summary>
        /// <value>The tree.</value>
        public Node Tree { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Loads the given stream.</summary>
        /// <param name="stream">The stream to load.</param>
        /// <returns>An IModel.</returns>
        public override IModel Load(Stream stream)
        {
            var model = base.Load(stream) as DecisionTreeModel;

            return model;
        }

        /// <summary>Predicts the given y coordinate.</summary>
        /// <param name="y">The Vector to process.</param>
        /// <returns>A double.</returns>
        public override double Predict(Vector y)
        {
            return this.WalkNode(y, this.Tree);
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            this.Hint = double.Parse(reader.GetAttribute("Hint"));
            reader.ReadStartElement();

            this.Descriptor = Xml.Read<Descriptor>(reader);
            this.Tree = Xml.Read<Node>(reader);

            // re-establish tree cycles and values
            this.ReLinkNodes(this.Tree);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.PrintNode(this.Tree, "\t");
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Hint", this.Hint.ToString("r"));
            Xml.Write<Descriptor>(writer, this.Descriptor);
            Xml.Write<Node>(writer, this.Tree);
        }

        #endregion

        #region Methods

        /// <summary>Print node.</summary>
        /// <param name="n">The Node to process.</param>
        /// <param name="pre">The pre.</param>
        /// <returns>A string.</returns>
        private string PrintNode(Node n, string pre)
        {
            if (n.IsLeaf)
            {
                return string.Format("{0} +({1}, {2})\n", pre, n.Label, n.Value);
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine(string.Format("{0}[{1}, {2:0.0000}]", pre, n.Name, n.Gain));
                foreach (var edge in n.Edges)
                {
                    sb.AppendLine(string.Format("{0} |- {1}", pre, edge.Label));
                    sb.Append(this.PrintNode(edge.Child, string.Format("{0} |\t", pre)));
                }

                return sb.ToString();
            }
        }

        /// <summary>Re link nodes.</summary>
        /// <param name="n">The Node to process.</param>
        private void ReLinkNodes(Node n)
        {
            if (n.Edges != null)
            {
                foreach (var e in n.Edges)
                {
                    e.Parent = n;
                    if (e.Child.IsLeaf)
                    {
                        e.Child.Label = this.Descriptor.Label.Convert(e.Child.Value);
                    }
                    else
                    {
                        this.ReLinkNodes(e.Child);
                    }
                }
            }
        }

        /// <summary>Walk node.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="v">The Vector to process.</param>
        /// <param name="node">The node.</param>
        /// <returns>A double.</returns>
        private double WalkNode(Vector v, Node node)
        {
            if (node.IsLeaf)
            {
                return node.Value;
            }

            // Get the index of the feature for this node.
            var col = node.Column;
            if (col == -1)
            {
                throw new InvalidOperationException("Invalid Feature encountered during node walk!");
            }

            for (var i = 0; i < node.Edges.Length; i++)
            {
                var edge = node.Edges[i];
                if (edge.Discrete && v[col] == edge.Min)
                {
                    return this.WalkNode(v, edge.Child);
                }

                if (!edge.Discrete && v[col] >= edge.Min && v[col] < edge.Max)
                {
                    return this.WalkNode(v, edge.Child);
                }
            }

            if (this.Hint != double.Epsilon)
            {
                return this.Hint;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to match split value {0} for feature {1}[2]\nConsider setting a Hint in order to avoid this error.", 
                        v[col], 
                        this.Descriptor.At(col), 
                        col));
            }
        }

        #endregion
    }
}