// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Xml.cs">
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
//   Xml serialization helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Utils
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>Xml serialization helper.</summary>
    public static class Xml
    {
        #region Public Methods and Operators

        /// <summary>Loads the given stream.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="file">file.</param>
        /// <returns>A T.</returns>
        public static T Load<T>(string file)
        {
            using (var stream = File.OpenRead(file)) return Load<T>(stream);
        }

        /// <summary>Loads.</summary>
        /// <param name="file">file.</param>
        /// <param name="t">type.</param>
        /// <returns>An object.</returns>
        public static object Load(string file, Type t)
        {
            using (var stream = File.OpenRead(file)) return Load(stream, t);
        }

        /// <summary>Loads the given stream.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="stream">The stream.</param>
        /// <returns>A T.</returns>
        public static T Load<T>(Stream stream)
        {
            var o = Load(stream, typeof(T));
            return (T)o;
        }

        /// <summary>Loads.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="t">type.</param>
        /// <returns>An object.</returns>
        public static object Load(Stream stream, Type t)
        {
            var serializer = new XmlSerializer(t);
            var o = serializer.Deserialize(stream);
            return o;
        }

        /// <summary>Loads XML string.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="xml">The XML.</param>
        /// <returns>The XML string.</returns>
        public static T LoadXmlString<T>(string xml)
        {
            var o = LoadXmlString(xml, typeof(T));
            return (T)o;
        }

        /// <summary>Loads XML string.</summary>
        /// <param name="xml">The XML.</param>
        /// <param name="t">type.</param>
        /// <returns>The XML string.</returns>
        public static object LoadXmlString(string xml, Type t)
        {
            TextReader reader = new StringReader(xml);
            var serializer = new XmlSerializer(t);
            var o = serializer.Deserialize(reader);
            return o;
        }

        /// <summary>Reads the given reader.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="reader">The reader.</param>
        /// <returns>A T.</returns>
        public static T Read<T>(XmlReader reader)
        {
            var dserializer = new XmlSerializer(typeof(T));
            var item = (T)dserializer.Deserialize(reader);

            // move to next thing
            // reader.Read();
            return item;
        }

        /// <summary>Save object to file.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="file">file.</param>
        /// <param name="o">object.</param>
        /// ###
        /// <typeparam name="T">Type.</typeparam>
        public static void Save<T>(string file, T o)
        {
            using (var stream = File.OpenWrite(file)) Save(stream, o, typeof(T));
        }

        /// <summary>Save object to file.</summary>
        /// <param name="file">file.</param>
        /// <param name="o">object.</param>
        /// <param name="t">type.</param>
        public static void Save(string file, object o, Type t)
        {
            using (var stream = File.OpenWrite(file)) Save(stream, o, t);
        }

        /// <summary>Save object to file.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="stream">The stream.</param>
        /// <param name="o">object.</param>
        public static void Save<T>(Stream stream, T o)
        {
            Save(stream, o, typeof(T));
        }

        /// <summary>Save object to file.</summary>
        /// <param name="stream">The stream.</param>
        /// <param name="o">object.</param>
        /// <param name="t">type.</param>
        public static void Save(Stream stream, object o, Type t)
        {
            var serializer = new XmlSerializer(t);
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            serializer.Serialize(stream, o, ns);
        }

        /// <summary>Converts an o to an XML string.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="o">object.</param>
        /// <returns>o as a string.</returns>
        public static string ToXmlString<T>(T o)
        {
            return ToXmlString(o, typeof(T));
        }

        /// <summary>Converts this object to an XML string.</summary>
        /// <param name="o">object.</param>
        /// <param name="t">type.</param>
        /// <returns>The given data converted to a string.</returns>
        public static string ToXmlString(object o, Type t)
        {
            var serializer = new XmlSerializer(t);
            var ns = new XmlSerializerNamespaces();
            var textWriter = new StringWriter();
            ns.Add(string.Empty, string.Empty);

            serializer.Serialize(textWriter, o, ns);
            return textWriter.ToString();
        }

        /// <summary>Writes.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="writer">The writer.</param>
        /// <param name="thing">The thing.</param>
        public static void Write<T>(XmlWriter writer, T thing)
        {
            var serializer = new XmlSerializer(typeof(T));
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            // check for a null thing
            if (thing != null)
            {
                serializer.Serialize(writer, thing, ns);
            }
            else
            {
                var ctor = typeof(T).GetConstructor(new Type[] { });
                if (ctor != null)
                {
                    serializer.Serialize(writer, ctor.Invoke(new object[] { }), ns);
                }
                else
                {
                    serializer.Serialize(writer, default(T), ns);
                }
            }
        }

        #endregion
    }
}