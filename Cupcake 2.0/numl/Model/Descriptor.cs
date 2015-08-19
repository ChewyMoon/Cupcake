﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Descriptor.cs">
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
//   This class is designed to describe the underlying types that will be used in the machine
//   learning process. Any machine learning process requires a set of <see cref="Features" /> that
//   will be used to discriminate the <see cref="Label" />. The <see cref="Label" /> itself is the
//   target element that the machine learning algorithms learn to predict.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using numl.Math.LinearAlgebra;
    using numl.Utils;

    /// <summary>
    ///     This class is designed to describe the underlying types that will be used in the machine
    ///     learning process. Any machine learning process requires a set of <see cref="Features" /> that
    ///     will be used to discriminate the <see cref="Label" />. The <see cref="Label" /> itself is the
    ///     target element that the machine learning algorithms learn to predict.
    /// </summary>
    [XmlRoot("Descriptor")]
    [Serializable]
    public class Descriptor : IXmlSerializable
    {
        #region Fields

        /// <summary>Length of the vector.</summary>
        private int _vectorLength = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Descriptor" /> class. Default constructor.
        /// </summary>
        public Descriptor()
        {
            this.Name = string.Empty;
            this.Features = new Property[] { };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Set of features used to discriminate or learn about the <see cref="Label" />.
        /// </summary>
        /// <value>The features.</value>
        public Property[] Features { get; set; }

        /// <summary>Target property that is the target of machine learning.</summary>
        /// <value>The label.</value>
        public Property Label { get; set; }

        /// <summary>Descriptor name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Base type of object being described. This could also be null.</summary>
        /// <value>The type.</value>
        public Type Type { get; set; }

        /// <summary>
        ///     Total feature count The number of features does not necessarily equal the number of
        ///     <see cref="Features" /> given that there might exist multi-valued features.
        /// </summary>
        /// <value>The length of the vector.</value>
        public int VectorLength
        {
            get
            {
                if (this._vectorLength < 0)
                {
                    this._vectorLength = this.Features.Select(f => f.Length).Sum();
                }

                return this._vectorLength;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>Index into features (for convenience)</summary>
        /// <param name="i">Feature index.</param>
        /// <returns>Feature Property.</returns>
        public Property this[int i]
        {
            get
            {
                if (i >= this.Features.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    return this.Features[i];
                }
            }
        }

        /// <summary>Index intor features (for convenience)</summary>
        /// <param name="name">Feature name.</param>
        /// <returns>Feature Property.</returns>
        public Property this[string name]
        {
            get
            {
                if (this.Features.Where(p => p.Name == name).Count() == 1)
                {
                    return this.Features.Where(p => p.Name == name).First();
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        // ---- Creational
        /// <summary>Creates a descriptor based upon a marked up concrete class.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <returns>Descriptor.</returns>
        /// ###
        /// <typeparam name="T">Class Type.</typeparam>
        public static Descriptor Create<T>() where T : class
        {
            return Create(typeof(T));
        }

        /// <summary>Creates a descriptor based upon a marked up concrete type.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="t">Class Type.</param>
        /// <returns>Descriptor.</returns>
        public static Descriptor Create(Type t)
        {
            if (!t.IsClass)
            {
                throw new InvalidOperationException("Can only work with class types");
            }

            var features = new List<Property>();
            Property label = null;

            foreach (var property in t.GetProperties())
            {
                var item = property.GetCustomAttributes(typeof(NumlAttribute), false);

                if (item.Length == 1)
                {
                    var attrib = (NumlAttribute)item[0];

                    // generate appropriate property from attribute
                    var p = attrib.GenerateProperty(property);

                    // feature
                    if (attrib.GetType().IsSubclassOf(typeof(FeatureAttribute)) || attrib is FeatureAttribute)
                    {
                        features.Add(p);
                    }

                    // label
                    else if (attrib.GetType().IsSubclassOf(typeof(LabelAttribute)) || attrib is LabelAttribute)
                    {
                        if (label != null)
                        {
                            throw new InvalidOperationException("Cannot have multiple labels in a class");
                        }

                        label = p;
                    }
                }
            }

            return new Descriptor { Features = features.ToArray(), Label = label, Type = t, };
        }

        /// <summary>
        ///     Creates a new descriptor using a strongly typed fluent approach. This initial descriptor is
        ///     worthless without adding features.
        /// </summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <returns>Empty Descriptor.</returns>
        /// ###
        /// <typeparam name="T">Source Object Type.</typeparam>
        public static Descriptor<T> For<T>()
        {
            return new Descriptor<T>() { Type = typeof(T), Features = new Property[] { } };
        }

        /// <summary>
        ///     Creates a new descriptor using a strongly typed fluent approach. This initial descriptor is
        ///     worthless without adding features.
        /// </summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="name">Desired Descriptor Name.</param>
        /// <returns>Empty Descriptor.</returns>
        /// ###
        /// <typeparam name="T">Source Object Type.</typeparam>
        public static Descriptor<T> For<T>(string name)
        {
            return new Descriptor<T>() { Name = name, Type = typeof(T), Features = new Property[] { } };
        }

        /// <summary>Load a descriptor from a file.</summary>
        /// <param name="file">File Location.</param>
        /// <returns>Descriptor.</returns>
        public static Descriptor Load(string file)
        {
            using (var stream = File.OpenRead(file)) return Load(stream);
        }

        /// <summary>Load a descriptor from a stream.</summary>
        /// <param name="stream">Stream.</param>
        /// <returns>Descriptor.</returns>
        public static Descriptor Load(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Descriptor));
            var o = serializer.Deserialize(stream);
            return (Descriptor)o;
        }

        /// <summary>
        ///     Creates a new descriptor using a fluent approach. This initial descriptor is worthless
        ///     without adding features.
        /// </summary>
        /// <returns>Empty Descriptor.</returns>
        public static Descriptor New()
        {
            return new Descriptor() { Features = new Property[] { } };
        }

        /// <summary>
        ///     Creates a new descriptor using a fluent approach. This initial descriptor is worthless
        ///     without adding features.
        /// </summary>
        /// <param name="name">Desired name.</param>
        /// <returns>Empty Named Descriptor.</returns>
        public static Descriptor New(string name)
        {
            return new Descriptor() { Name = name, Features = new Property[] { } };
        }

        /// <summary>
        ///     Creates a new descriptor using a fluent approach. This initial descriptor is worthless
        ///     without adding features.
        /// </summary>
        /// <param name="type">Type mapping.</param>
        /// <returns>A Descriptor.</returns>
        public static Descriptor New(Type type)
        {
            return new Descriptor() { Type = type, Features = new Property[] { } };
        }

        /// <summary>Gets related property given its offset within the vector representation.</summary>
        /// <exception cref="IndexOutOfRangeException">
        ///     Thrown when the index is outside the required
        ///     range.
        /// </exception>
        /// <param name="i">Vector Index.</param>
        /// <returns>Associated Feature.</returns>
        public Property At(int i)
        {
            if (i < 0 || i > this.VectorLength)
            {
                throw new IndexOutOfRangeException(string.Format("{0} falls outside of the appropriate range", i));
            }

            var q = from p in this.Features where i >= p.Start && i < p.Start + p.Length select p;

            return q.First();
        }

        /// <summary>
        ///     Gets related property column name given its offset within the vector representation.
        /// </summary>
        /// <param name="i">Vector Index.</param>
        /// <returns>Associated Property Name.</returns>
        public string ColumnAt(int i)
        {
            var prop = this.At(i);
            var offset = i - prop.Start;
            var col = prop.GetColumns().ElementAt(offset);
            return col;
        }

        /// <summary>
        ///     Converts a given example into a lazy list of doubles in preparation for vector conversion
        ///     (both features and corresponding label)
        /// </summary>
        /// <param name="item">Example.</param>
        /// <returns>Lazy List of doubles.</returns>
        public IEnumerable<double> Convert(object item)
        {
            return this.Convert(item, true);
        }

        /// <summary>
        ///     Converts a given example into a lazy list of doubles in preparation for vector conversion.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="item">Example.</param>
        /// <param name="withLabel">Should convert label as well.</param>
        /// <returns>Lazy List of doubles.</returns>
        public IEnumerable<double> Convert(object item, bool withLabel)
        {
            if (this.Features.Length == 0)
            {
                throw new InvalidOperationException("Cannot convert item with an empty Feature set.");
            }

            for (var i = 0; i < this.Features.Length; i++)
            {
                // current feature
                var feature = this.Features[i];

                // pre-process item
                feature.PreProcess(item);

                // start start position
                if (feature.Start < 0)
                {
                    feature.Start = i == 0 ? 0 : this.Features[i - 1].Start + this.Features[i - 1].Length;
                }

                // retrieve item
                var o = Ject.Get(item, feature.Name);

                // convert item
                foreach (var val in feature.Convert(o))
                {
                    yield return val;
                }

                // post-process item
                feature.PostProcess(item);
            }

            // convert label (if available)
            if (this.Label != null && withLabel)
            {
                foreach (var val in this.Label.Convert(Ject.Get(item, this.Label.Name)))
                {
                    yield return val;
                }
            }
        }

        /// <summary>Converts a list of examples into a lazy double list of doubles.</summary>
        /// <param name="items">Examples.</param>
        /// <returns>Lazy double enumerable of doubles.</returns>
        public IEnumerable<IEnumerable<double>> Convert(IEnumerable<object> items)
        {
            // Pre processing items
            foreach (var feature in this.Features)
            {
                feature.PreProcess(items);
            }

            if (this.Label != null)
            {
                this.Label.PreProcess(items);
            }

            // convert items
            foreach (var o in items)
            {
                yield return this.Convert(o);
            }

            // Post processing items
            foreach (var feature in this.Features)
            {
                feature.PostProcess(items);
            }

            if (this.Label != null)
            {
                this.Label.PostProcess(items);
            }
        }

        /// <summary>
        ///     Available column names used to discriminate or learn about <see cref="Label" />. The number of
        ///     columns does not necessarily equal the number of <see cref="Features" /> given that there
        ///     might exist multi-valued features.
        /// </summary>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the columns in this collection.
        /// </returns>
        public IEnumerable<string> GetColumns()
        {
            foreach (var p in this.Features)
            {
                foreach (var s in p.GetColumns())
                {
                    yield return s;
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

        /// <summary>Adds (or replaces) a label to the descriptor.</summary>
        /// <param name="name">Name of label (must match property name or dictionary key)</param>
        /// <returns>A DescriptorProperty.</returns>
        public DescriptorProperty Learn(string name)
        {
            return new DescriptorProperty(this, name, true);
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <exception cref="TypeLoadException">Thrown when a Type Load error condition occurs.</exception>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public void ReadXml(XmlReader reader)
        {
            var descendants = Ject.FindAllAssignableFrom(typeof(Property));

            // don't want to leak memory, should reuse serializers
            var serializers = new Dictionary<string, XmlSerializer>();
            Func<string, XmlSerializer> serializer = s =>
                {
                    if (!serializers.ContainsKey(s))
                    {
                        var t = (from y in descendants where y.Name == s select y).FirstOrDefault();
                        if (t == null)
                        {
                            throw new TypeLoadException(string.Format("Could not find type {0}", reader.LocalName));
                        }

                        serializers[s] = new XmlSerializer(t);
                    }

                    return serializers[reader.LocalName];
                };

            reader.MoveToContent();
            var type = reader.GetAttribute("Type");
            if (type != null && type.ToLowerInvariant() != "none")
            {
                this.Type = Ject.FindType(type);
            }

            this.Name = reader.GetAttribute("Name");

            reader.ReadStartElement();
            var length = reader.GetAttribute("Length") ?? "0";
            this.Features = new Property[int.Parse(length)];
            reader.ReadStartElement("Features");
            for (var i = 0; i < this.Features.Length; i++)
            {
                this.Features[i] = (Property)serializer(reader.LocalName).Deserialize(reader);
                reader.Read();
            }

            reader.ReadEndElement();

            // is there a label?
            if (reader.LocalName == "Label")
            {
                reader.ReadStartElement("Label");
                this.Label = (Property)serializer(reader.LocalName).Deserialize(reader);
                reader.Read();
                reader.ReadEndElement();

                // move to end Descriptor
                reader.Read();
            }
        }

        /// <summary>Converts a list of examples into a Matrix/Vector tuple.</summary>
        /// <param name="examples">Examples.</param>
        /// <returns>Tuple containing Matrix and Vector.</returns>
        public Tuple<Matrix, Vector> ToExamples(IEnumerable<object> examples)
        {
            return this.Convert(examples).ToExamples();
        }

        /// <summary>Converts a list of examples into a Matrix.</summary>
        /// <param name="examples">Examples.</param>
        /// <returns>Matrix representation.</returns>
        public Matrix ToMatrix(IEnumerable<object> examples)
        {
            return this.Convert(examples).ToMatrix();
        }

        /// <summary>Pretty printed descriptor.</summary>
        /// <returns>Pretty printed string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Descriptor ({0}) {{", this.Type == null ? this.Name : this.Type.Name));
            for (var i = 0; i < this.Features.Length; i++)
            {
                sb.AppendLine(string.Format("   {0}", this.Features[i]));
            }

            if (this.Label != null)
            {
                sb.AppendLine(string.Format("  *{0}", this.Label));
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        ///     Convert an object to its vector representation based on the descriptor properties.
        /// </summary>
        /// <param name="item">object to convert.</param>
        /// <returns>Vector representation.</returns>
        public Vector ToVector(object item)
        {
            return this.Convert(item).ToVector();
        }

        /// <summary>Adds a new feature to descriptor.</summary>
        /// <param name="name">Name of feature (must match property name or dictionary key)</param>
        /// <returns>method for describing feature.</returns>
        public DescriptorProperty With(string name)
        {
            return new DescriptorProperty(this, name, false);
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public void WriteXml(XmlWriter writer)
        {
            // don't want to leak memory, should reuse serializers
            var serializers = new Dictionary<Type, XmlSerializer>();

            writer.WriteAttributeString("Type", this.Type == null ? "None" : this.Type.Name);
            writer.WriteAttributeString("Name", this.Name == null ? string.Empty : this.Name);

            writer.WriteStartElement("Features");
            writer.WriteAttributeString("Length", this.Features.Length.ToString());
            for (var i = 0; i < this.Features.Length; i++)
            {
                var prop = this.Features[i];
                if (!serializers.ContainsKey(prop.GetType()))
                {
                    serializers[prop.GetType()] = new XmlSerializer(prop.GetType());
                }

                serializers[prop.GetType()].Serialize(writer, prop);
            }

            writer.WriteEndElement();

            if (this.Label != null)
            {
                writer.WriteStartElement("Label");
                if (!serializers.ContainsKey(this.Label.GetType()))
                {
                    serializers[this.Label.GetType()] = new XmlSerializer(this.Label.GetType());
                }

                serializers[this.Label.GetType()].Serialize(writer, this.Label);
                writer.WriteEndElement();
            }
        }

        #endregion
    }

    /// <summary>A descriptor.</summary>
    /// <tparam name="T">Generic type parameter.</tparam>
    public class Descriptor<T> : Descriptor
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Descriptor{T}" /> class. Initializes a new instance of the Descriptor
        ///     class.
        /// </summary>
        public Descriptor()
        {
            this.Type = typeof(T);
            this.Features = new Property[] { };
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Learns the given property.</summary>
        /// <param name="property">The property.</param>
        /// <returns>A Descriptor&lt;T&gt;</returns>
        public Descriptor<T> Learn(Expression<Func<T, object>> property)
        {
            var pi = GetPropertyInfo(property);
            this.AddProperty(TypeHelpers.GenerateLabel(pi.PropertyType, pi.Name), true);
            return this;
        }

        /// <summary>Withs the given property.</summary>
        /// <param name="property">The property.</param>
        /// <returns>A Descriptor&lt;T&gt;</returns>
        public Descriptor<T> With(Expression<Func<T, object>> property)
        {
            var pi = GetPropertyInfo<object>(property);
            this.AddProperty(TypeHelpers.GenerateFeature(pi.PropertyType, pi.Name), false);
            return this;
        }

        /// <summary>With date time.</summary>
        /// <param name="property">The property.</param>
        /// <param name="features">The features.</param>
        /// <returns>A Descriptor&lt;T&gt;</returns>
        public Descriptor<T> WithDateTime(Expression<Func<T, DateTime>> property, DateTimeFeature features)
        {
            var pi = GetPropertyInfo<DateTime>(property);
            var p = new DateTimeProperty(features) { Discrete = true, Name = pi.Name };

            this.AddProperty(p, false);
            return this;
        }

        /// <summary>With date time.</summary>
        /// <param name="property">The property.</param>
        /// <param name="portion">The portion.</param>
        /// <returns>A Descriptor&lt;T&gt;</returns>
        public Descriptor<T> WithDateTime(Expression<Func<T, DateTime>> property, DatePortion portion)
        {
            var pi = GetPropertyInfo<DateTime>(property);
            var p = new DateTimeProperty(portion) { Discrete = true, Name = pi.Name };

            this.AddProperty(p, false);
            return this;
        }

        /// <summary>With enumerable.</summary>
        /// <param name="property">The property.</param>
        /// <param name="length">The length.</param>
        /// <returns>A Descriptor&lt;T&gt;</returns>
        public Descriptor<T> WithEnumerable(Expression<Func<T, IEnumerable>> property, int length)
        {
            var pi = GetPropertyInfo<IEnumerable>(property);
            var p = new EnumerableProperty(length) { Name = pi.Name, Discrete = false };

            this.AddProperty(p, false);
            return this;
        }

        /// <summary>With string.</summary>
        /// <param name="property">The property.</param>
        /// <param name="splitType">Type of the split.</param>
        /// <param name="separator">(Optional) the separator.</param>
        /// <param name="asEnum">(Optional) true to as enum.</param>
        /// <param name="exclusions">(Optional) the exclusions.</param>
        /// <returns>A Descriptor&lt;T&gt;</returns>
        public Descriptor<T> WithString(
            Expression<Func<T, string>> property, 
            StringSplitType splitType, 
            string separator = " ", 
            bool asEnum = false, 
            string exclusions = null)
        {
            var pi = GetPropertyInfo<string>(property);
            var p = new StringProperty();
            p.Name = pi.Name;
            p.SplitType = splitType;
            p.Separator = separator;
            p.ImportExclusions(exclusions);
            p.AsEnum = asEnum;
            this.AddProperty(p, false);
            return this;
        }

        #endregion

        #region Methods

        /// <summary>Gets property information.</summary>
        /// <tparam name="K">Generic type parameter.</tparam>
        /// <param name="property">The property.</param>
        /// <returns>The property information.</returns>
        private static PropertyInfo GetPropertyInfo<K>(Expression<Func<T, K>> property)
        {
            PropertyInfo propertyInfo = null;
            if (property.Body is MemberExpression)
            {
                propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;
            }
            else
            {
                propertyInfo = (((UnaryExpression)property.Body).Operand as MemberExpression).Member as PropertyInfo;
            }

            return propertyInfo;
        }

        /// <summary>Adds a property to 'label'.</summary>
        /// <param name="p">The Property to process.</param>
        /// <param name="label">true to label.</param>
        private void AddProperty(Property p, bool label)
        {
            if (label)
            {
                this.Label = p;
            }
            else
            {
                var features = new List<Property>(this.Features ?? new Property[] { });
                features.Add(p);
                this.Features = features.ToArray();
            }
        }

        #endregion
    }
}