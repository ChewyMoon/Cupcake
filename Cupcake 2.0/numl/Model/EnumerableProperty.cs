// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableProperty.cs" company="ChewyMoon">
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
//   Enumerable property. Expanded feature.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;

    using numl.Utils;

    /// <summary>Enumerable property. Expanded feature.</summary>
    [XmlRoot("EnumerableProperty")]
    [Serializable]
    public class EnumerableProperty : Property
    {
        #region Fields

        /// <summary>The length.</summary>
        private int _length;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumerableProperty" /> class. Constructor.
        /// </summary>
        /// <param name="length">
        ///     The length.
        /// </param>
        public EnumerableProperty(int length)
        {
            this._length = length;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumerableProperty" /> class. Default constructor.
        /// </summary>
        internal EnumerableProperty()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Length of property.</summary>
        /// <value>The length.</value>
        public override int Length
        {
            get
            {
                return this._length;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Convert the numeric representation back to the original type.</summary>
        /// <param name="val">.</param>
        /// <returns>An object.</returns>
        public override object Convert(double val)
        {
            return val;
        }

        /// <summary>Convert an object to a list of numbers.</summary>
        /// <exception cref="InvalidCastException">
        ///     Thrown when an object cannot be cast to a required
        ///     type.
        /// </exception>
        /// <param name="o">Object.</param>
        /// <returns>Lazy list of doubles.</returns>
        public override IEnumerable<double> Convert(object o)
        {
            // is it some sort of enumeration?
            if (o.GetType().GetInterfaces().Contains(typeof(IEnumerable)))
            {
                var a = (IEnumerable)o;
                var i = 0;
                foreach (var item in a)
                {
                    // if on first try we can't do anything, just bail;
                    // needs to be an enumeration of a simple type
                    if (i == 0 && !Ject.CanUseSimpleType(item.GetType()))
                    {
                        throw new InvalidCastException(
                            string.Format("Cannot properly cast {0} to a number", item.GetType()));
                    }

                    // check if contained item is discrete
                    if (i == 0)
                    {
                        var type = item.GetType();
                        this.Discrete = type.BaseType == typeof(Enum) || type == typeof(bool) || type == typeof(string)
                                        || type == typeof(char);
                    }

                    yield return Ject.Convert(item);

                    // should pull no more than specified length
                    if (++i == this.Length)
                    {
                        break;
                    }
                }

                // pad excess with 0's
                for (var j = i + 1; i < this.Length; i++)
                {
                    yield return 0;
                }
            }
            else
            {
                throw new InvalidCastException(string.Format("Cannot cast {0} to an IEnumerable", o.GetType().Name));
            }
        }

        /// <summary>
        ///     Retrieve the list of expanded columns. If there is a one-to-one correspondence between the
        ///     type and its expansion it will return a single value/.
        /// </summary>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the columns in this collection.
        /// </returns>
        public override IEnumerable<string> GetColumns()
        {
            for (var i = 0; i < this._length; i++)
            {
                yield return i.ToString();
            }
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            this.Name = reader.GetAttribute("Name");
            var elementType = reader.GetAttribute("ElementType");
            if (elementType != "None")
            {
                this.Type = Ject.FindType(elementType);
            }

            this.Discrete = bool.Parse(reader.GetAttribute("Discrete"));
            this.Start = int.Parse(reader.GetAttribute("Start"));
            this._length = int.Parse(reader.GetAttribute("Length"));
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", this.Name);
            writer.WriteAttributeString("ElementType", this.Type == null ? "None" : this.Type.Name);
            writer.WriteAttributeString("Discrete", this.Discrete.ToString());
            writer.WriteAttributeString("Start", this.Start.ToString());

            writer.WriteAttributeString("Length", this._length.ToString());
        }

        #endregion
    }
}