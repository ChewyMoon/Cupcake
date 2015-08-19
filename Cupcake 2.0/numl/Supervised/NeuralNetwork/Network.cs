// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Network.cs">
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
//   A network.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.NeuralNetwork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using numl.Math.Functions;
    using numl.Math.LinearAlgebra;
    using numl.Model;

    /// <summary>A network.</summary>
    [XmlRoot("Network")]
    public class Network : IXmlSerializable
    {
        #region Fields

        /// <summary>The edges.</summary>
        private HashSet<Tuple<string, string>> _edges;

        /// <summary>The nodes.</summary>
        private HashSet<string> _nodes;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the in.</summary>
        /// <value>The in.</value>
        public Node[] In { get; set; }

        /// <summary>Gets or sets the out.</summary>
        /// <value>The out.</value>
        public Node[] Out { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Defaults.</summary>
        /// <param name="d">The Descriptor to process.</param>
        /// <param name="x">The Vector to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <param name="activation">The activation.</param>
        /// <returns>A Network.</returns>
        public static Network Default(Descriptor d, Matrix x, Vector y, IFunction activation)
        {
            var nn = new Network();

            // set output to number of choices of available
            // 1 if only two choices
            var distinct = y.Distinct().Count();
            var output = distinct > 2 ? distinct : 1;

            // identity funciton for bias nodes
            IFunction ident = new Ident();

            // set number of hidden units to (Input + Hidden) * 2/3 as basic best guess. 
            var hidden = (int)Math.Ceiling((decimal)(x.Cols + output) * 2m / 3m);

            // creating input nodes
            nn.In = new Node[x.Cols + 1];
            nn.In[0] = new Node { Label = "B0", Activation = ident };
            for (var i = 1; i < x.Cols + 1; i++)
            {
                nn.In[i] = new Node { Label = d.ColumnAt(i - 1), Activation = ident };
            }

            // creating hidden nodes
            var h = new Node[hidden + 1];
            h[0] = new Node { Label = "B1", Activation = ident };
            for (var i = 1; i < hidden + 1; i++)
            {
                h[i] = new Node { Label = string.Format("H{0}", i), Activation = activation };
            }

            // creating output nodes
            nn.Out = new Node[output];
            for (var i = 0; i < output; i++)
            {
                nn.Out[i] = new Node { Label = GetLabel(i, d), Activation = activation };
            }

            // link input to hidden. Note: there are
            // no inputs to the hidden bias node
            for (var i = 1; i < h.Length; i++)
            {
                for (var j = 0; j < nn.In.Length; j++)
                {
                    Edge.Create(nn.In[j], h[i]);
                }
            }

            // link from hidden to output (full)
            for (var i = 0; i < nn.Out.Length; i++)
            {
                for (var j = 0; j < h.Length; j++)
                {
                    Edge.Create(h[j], nn.Out[i]);
                }
            }

            return nn;
        }

        /// <summary>Backs.</summary>
        /// <param name="t">The double to process.</param>
        /// <param name="learningRate">The learning rate.</param>
        public void Back(double t, double learningRate)
        {
            // propagate error gradients
            for (var i = 0; i < this.In.Length; i++)
            {
                this.In[i].Error(t);
            }

            // reset weights
            for (var i = 0; i < this.Out.Length; i++)
            {
                this.Out[i].Update(learningRate);
            }
        }

        /// <summary>Forwards the given x coordinate.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The Vector to process.</param>
        public void Forward(Vector x)
        {
            if (this.In.Length != x.Length + 1)
            {
                throw new InvalidOperationException("Input nodes not aligned to input vector");
            }

            // set input
            for (var i = 0; i < this.In.Length; i++)
            {
                this.In[i].Input = this.In[i].Output = i == 0 ? 1 : x[i - 1];
            }

            // evaluate
            for (var i = 0; i < this.Out.Length; i++)
            {
                this.Out[i].Evaluate();
            }
        }

        /// <summary>Gets the edges in this collection.</summary>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the edges in this collection.
        /// </returns>
        public IEnumerable<Edge> GetEdges()
        {
            if (this._edges == null)
            {
                this._edges = new HashSet<Tuple<string, string>>();
            }
            else
            {
                this._edges.Clear();
            }

            foreach (var node in this.Out)
            {
                foreach (var edge in this.GetEdges(node))
                {
                    var key = new Tuple<string, string>(edge.Source.Id, edge.Target.Id);
                    if (!this._edges.Contains(key))
                    {
                        this._edges.Add(key);
                        yield return edge;
                    }
                }
            }
        }

        /// <summary>Gets the nodes in this collection.</summary>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the nodes in this collection.
        /// </returns>
        public IEnumerable<Node> GetNodes()
        {
            if (this._nodes == null)
            {
                this._nodes = new HashSet<string>();
            }
            else
            {
                this._nodes.Clear();
            }

            foreach (var node in this.Out)
            {
                this._nodes.Add(node.Id);
                yield return node;
                foreach (var n in this.GetNodes(node))
                {
                    if (!this._nodes.Contains(n.Id))
                    {
                        this._nodes.Add(n.Id);
                        yield return n;
                    }
                }
            }
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
            var nSerializer = new XmlSerializer(typeof(Node));
            var eSerializer = new XmlSerializer(typeof(Edge));

            reader.MoveToContent();

            var nodes = new Dictionary<string, Node>();
            var length = 0;
            reader.ReadStartElement();
            length = int.Parse(reader.GetAttribute("Length"));
            reader.ReadStartElement("Nodes");
            for (var i = 0; i < length; i++)
            {
                var node = (Node)nSerializer.Deserialize(reader);
                nodes.Add(node.Id, node);
                reader.Read();
            }

            reader.ReadEndElement();

            length = int.Parse(reader.GetAttribute("Length"));
            reader.ReadStartElement("Edges");
            for (var i = 0; i < length; i++)
            {
                var edge = (Edge)eSerializer.Deserialize(reader);
                reader.Read();

                edge.Source = nodes[edge.SourceId];
                edge.Target = nodes[edge.TargetId];

                edge.Source.Out.Add(edge);
                edge.Target.In.Add(edge);
            }

            reader.ReadEndElement();

            length = int.Parse(reader.GetAttribute("Length"));
            reader.ReadStartElement("In");
            this.In = new Node[length];
            for (var i = 0; i < length; i++)
            {
                reader.MoveToContent();
                this.In[i] = nodes[reader.GetAttribute("Id")];
                reader.Read();
            }

            reader.ReadEndElement();

            length = int.Parse(reader.GetAttribute("Length"));
            reader.ReadStartElement("Out");
            this.Out = new Node[length];
            for (var i = 0; i < length; i++)
            {
                reader.MoveToContent();
                this.Out[i] = nodes[reader.GetAttribute("Id")];
                reader.Read();
            }

            reader.ReadEndElement();
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public void WriteXml(XmlWriter writer)
        {
            var nSerializer = new XmlSerializer(typeof(Node));
            var eSerializer = new XmlSerializer(typeof(Edge));

            var nodes = this.GetNodes().ToArray();
            writer.WriteStartElement("Nodes");
            writer.WriteAttributeString("Length", nodes.Length.ToString());
            foreach (var node in nodes)
            {
                nSerializer.Serialize(writer, node);
            }

            writer.WriteEndElement();

            var edges = this.GetEdges().ToArray();
            writer.WriteStartElement("Edges");
            writer.WriteAttributeString("Length", edges.Length.ToString());
            foreach (var edge in edges)
            {
                eSerializer.Serialize(writer, edge);
            }

            writer.WriteEndElement();

            writer.WriteStartElement("In");
            writer.WriteAttributeString("Length", this.In.Length.ToString());
            for (var i = 0; i < this.In.Length; i++)
            {
                writer.WriteStartElement("Node");
                writer.WriteAttributeString("Id", this.In[i].Id);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteStartElement("Out");
            writer.WriteAttributeString("Length", this.Out.Length.ToString());
            for (var i = 0; i < this.Out.Length; i++)
            {
                writer.WriteStartElement("Node");
                writer.WriteAttributeString("Id", this.Out[i].Id);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion

        #region Methods

        /// <summary>Gets a label.</summary>
        /// <param name="n">The Node to process.</param>
        /// <param name="d">The Descriptor to process.</param>
        /// <returns>The label.</returns>
        private static string GetLabel(int n, Descriptor d)
        {
            if (d.Label.Type.IsEnum)
            {
                return Enum.GetName(d.Label.Type, n);
            }
            else if (d.Label is StringProperty && ((StringProperty)d.Label).AsEnum)
            {
                return ((StringProperty)d.Label).Dictionary[n];
            }
            else
            {
                return d.Label.Name;
            }
        }

        /// <summary>Gets the edges in this collection.</summary>
        /// <param name="n">The Node to process.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the edges in this collection.
        /// </returns>
        private IEnumerable<Edge> GetEdges(Node n)
        {
            foreach (var edge in n.In)
            {
                yield return edge;
                foreach (var e in this.GetEdges(edge.Source))
                {
                    yield return e;
                }
            }
        }

        /// <summary>Gets the nodes in this collection.</summary>
        /// <param name="n">The Node to process.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the nodes in this collection.
        /// </returns>
        private IEnumerable<Node> GetNodes(Node n)
        {
            foreach (var edge in n.In)
            {
                yield return edge.Source;
                foreach (var node in this.GetNodes(edge.Source))
                {
                    yield return node;
                }
            }
        }

        #endregion
    }
}