// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Property.cs">
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
//   Concrete property. Used to convert any given data type to a number.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using numl.Utils;

    /// <summary>Concrete property. Used to convert any given data type to a number.</summary>
    [XmlRoot("Property")]
    [Serializable]
    public class Property : IXmlSerializable
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Property" /> class. Default constructor.
        /// </summary>
        public Property()
        {
            this.Start = -1;
        }

        #endregion

        #region Public Properties

        /// <summary>Discrete or continuous value.</summary>
        /// <value>true if discrete, false if not.</value>
        public bool Discrete { get; set; }

        /// <summary>Length of property.</summary>
        /// <value>The length.</value>
        public virtual int Length
        {
            get
            {
                return 1;
            }
        }

        /// <summary>Property Name - Maps to object property or dictionary lookup.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Start position in array.</summary>
        /// <value>The start.</value>
        public int Start { get; set; }

        /// <summary>Type of property.</summary>
        /// <value>The type.</value>
        public virtual Type Type { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Convert the numeric representation back to the original type.</summary>
        /// <param name="val">.</param>
        /// <returns>An object.</returns>
        public virtual object Convert(double val)
        {
            return Ject.Convert(val, this.Type);
        }

        /// <summary>Convert an object to a list of numbers.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="o">Object.</param>
        /// <returns>Lazy list of doubles.</returns>
        public virtual IEnumerable<double> Convert(object o)
        {
            if (Ject.CanUseSimpleType(o.GetType()))
            {
                yield return Ject.Convert(o);
            }
            else
            {
                throw new InvalidOperationException(string.Format("Cannot convert {0} to a double", o.GetType()));
            }
        }

        /// <summary>
        ///     Retrieve the list of expanded columns. If there is a one-to-one correspondence between the
        ///     type and its expansion it will return a single value/.
        /// </summary>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the columns in this collection.
        /// </returns>
        public virtual IEnumerable<string> GetColumns()
        {
            yield return this.Name;
        }

        /// <summary>serialization.</summary>
        /// <returns>The schema.</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        ///     Used as a postprocessing step when overridden. Can be used to look at the entire data set as
        ///     a whole after converting single elements.
        /// </summary>
        /// <param name="examples">Examples.</param>
        public virtual void PostProcess(IEnumerable<object> examples)
        {
            return;
        }

        /// <summary>
        ///     Used as a postprocessing step when overriden. Can be used to look at the current object in
        ///     question fater converting single elements.
        /// </summary>
        /// <param name="example">.</param>
        public virtual void PostProcess(object example)
        {
        }

        /// <summary>
        ///     Used as a preprocessing step when overridden. Can be used to look at the entire data set as a
        ///     whole before converting single elements.
        /// </summary>
        /// <param name="examples">Examples.</param>
        public virtual void PreProcess(IEnumerable<object> examples)
        {
            return;
        }

        /// <summary>
        ///     Used as a preprocessing step when overriden. Can be used to look at the current object in
        ///     question before converting single elements.
        /// </summary>
        /// <param name="example">Example.</param>
        public virtual void PreProcess(object example)
        {
            return;
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public virtual void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            this.Name = reader.GetAttribute("Name");
            this.Type = Ject.FindType(reader.GetAttribute("Type"));
            this.Discrete = bool.Parse(reader.GetAttribute("Discrete"));
            this.Start = int.Parse(reader.GetAttribute("Start"));
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", this.Name, this.Start, this.Length);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", this.Name);
            writer.WriteAttributeString("Type", this.Type.Name);
            writer.WriteAttributeString("Discrete", this.Discrete.ToString());
            writer.WriteAttributeString("Start", this.Start.ToString());
        }

        #endregion
    }
}