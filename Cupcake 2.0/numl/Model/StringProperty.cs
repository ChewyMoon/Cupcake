// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="StringProperty.cs">
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
//   Enumeration describing how to split a string property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Serialization;

    using numl.Utils;

    /// <summary>Enumeration describing how to split a string property.</summary>
    public enum StringSplitType
    {
        /// <summary>
        ///     Split string into corresponding characters
        /// </summary>
        Character, 

        /// <summary>
        ///     Split string into corresponding words
        /// </summary>
        Word
    }

    /// <summary>Represents a string property.</summary>
    [XmlRoot("StringProperty")]
    [Serializable]
    public class StringProperty : Property
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringProperty" /> class. Default constructor.
        /// </summary>
        public StringProperty()
            : base()
        {
            // set to default conventions
            this.SplitType = StringSplitType.Word;
            this.Separator = " ";
            this.Dictionary = new string[] { };
            this.Exclude = new string[] { };
            this.AsEnum = false;
            this.Type = typeof(string);
            this.Discrete = true;
        }

        #endregion

        #region Public Properties

        /// <summary>Treat as enumeration.</summary>
        /// <value>true if as enum, false if not.</value>
        public bool AsEnum { get; set; }

        /// <summary>generated dictionary (using bag of words model)</summary>
        /// <value>The dictionary.</value>
        public string[] Dictionary { get; set; }

        /// <summary>Exclusion set (stopword removal)</summary>
        /// <value>The exclude.</value>
        public string[] Exclude { get; set; }

        /// <summary>Expansion length (total distinct words)</summary>
        /// <value>The length.</value>
        public override int Length
        {
            get
            {
                return this.AsEnum ? 1 : this.Dictionary.Length;
            }
        }

        /// <summary>How to separate words (defaults to a space)</summary>
        /// <value>The separator.</value>
        public string Separator { get; set; }

        /// <summary>How to split text.</summary>
        /// <value>The type of the split.</value>
        public StringSplitType SplitType { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Convert from number to string.</summary>
        /// <param name="val">Number.</param>
        /// <returns>String.</returns>
        public override object Convert(double val)
        {
            if (this.AsEnum)
            {
                return this.Dictionary[(int)val];
            }
            else
            {
                return val.ToString();
            }
        }

        /// <summary>Convert string to list of numbers.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="o">in string.</param>
        /// <returns>lazy list of numbers.</returns>
        public override IEnumerable<double> Convert(object o)
        {
            // check for valid dictionary
            if (this.Dictionary == null || this.Dictionary.Length == 0)
            {
                throw new InvalidOperationException(string.Format("{0} dictionaries do not exist.", this.Name));
            }

            // sanitize string
            var s = string.Empty;
            if (o == null || string.IsNullOrEmpty(o.ToString()) || string.IsNullOrWhiteSpace(o.ToString()))
            {
                s = StringHelpers.EMPTY_STRING;
            }
            else
            {
                s = o.ToString();
            }

            // returns single number
            if (this.AsEnum)
            {
                yield return (double)StringHelpers.GetWordPosition(s, this.Dictionary, false);
            }

            // returns list
            else
            {
                foreach (var val in StringHelpers.GetWordCount(s, this))
                {
                    yield return val;
                }
            }
        }

        /// <summary>Expansion column names.</summary>
        /// <returns>List of distinct words and positions.</returns>
        public override IEnumerable<string> GetColumns()
        {
            if (this.AsEnum)
            {
                yield return this.Name;
            }
            else
            {
                foreach (var s in this.Dictionary)
                {
                    yield return s;
                }
            }
        }

        /// <summary>import exclusion list from file.</summary>
        /// <param name="file">.</param>
        public void ImportExclusions(string file)
        {
            // add exclusions
            if (!string.IsNullOrEmpty(file) && !string.IsNullOrWhiteSpace(file) && File.Exists(file))
            {
                Regex regex;
                if (this.SplitType == StringSplitType.Word)
                {
                    regex = new Regex(
                        @"\w+", 
                        RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                else
                {
                    regex = new Regex(
                        @"\w", 
                        RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }

                var exclusionList = new List<string>();
                using (var sr = new StreamReader(file))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var match = regex.Match(line);

                        // found something not already in list...
                        if (match.Success && !exclusionList.Contains(match.Value.Trim().ToUpperInvariant()))
                        {
                            exclusionList.Add(match.Value.Trim().ToUpperInvariant());
                        }
                    }
                }

                this.Exclude = exclusionList.OrderBy(s => s).ToArray();
            }
            else
            {
                this.Exclude = new string[] { };
            }
        }

        /// <summary>Preprocess data set to create dictionary.</summary>
        /// <param name="examples">.</param>
        public override void PreProcess(IEnumerable<object> examples)
        {
            var q = from s in examples select Ject.Get(s, this.Name).ToString();

            if (this.AsEnum)
            {
                this.Dictionary = StringHelpers.BuildEnumDictionary(q).Select(kv => kv.Key).ToArray();
            }
            else
            {
                switch (this.SplitType)
                {
                    case StringSplitType.Character:
                        this.Dictionary =
                            StringHelpers.BuildCharDictionary(q, this.Exclude).Select(kv => kv.Key).ToArray();
                        break;
                    case StringSplitType.Word:
                        this.Dictionary =
                            StringHelpers.BuildWordDictionary(q, this.Separator, this.Exclude)
                                .Select(kv => kv.Key)
                                .ToArray();
                        break;
                }
            }
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            this.Separator = reader.GetAttribute("Separator");
            this.SplitType = (StringSplitType)Enum.Parse(typeof(StringSplitType), reader.GetAttribute("SplitType"));
            this.AsEnum = bool.Parse(reader.GetAttribute("AsEnum"));

            reader.ReadStartElement();

            this.Dictionary = new string[int.Parse(reader.GetAttribute("Length"))];
            reader.ReadStartElement("Dictionary");
            for (var i = 0; i < this.Dictionary.Length; i++)
            {
                this.Dictionary[i] = reader.ReadElementString("item");
            }

            reader.ReadEndElement();

            this.Exclude = new string[int.Parse(reader.GetAttribute("Length"))];
            reader.ReadStartElement("Exclude");
            for (var i = 0; i < this.Exclude.Length; i++)
            {
                this.Exclude[i] = reader.ReadElementString("item");
            }
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteAttributeString("Separator", this.Separator);
            writer.WriteAttributeString("SplitType", this.SplitType.ToString());
            writer.WriteAttributeString("AsEnum", this.AsEnum.ToString());

            writer.WriteStartElement("Dictionary");
            writer.WriteAttributeString("Length", this.Dictionary.Length.ToString());
            for (var i = 0; i < this.Dictionary.Length; i++)
            {
                writer.WriteStartElement("item");
                writer.WriteValue(this.Dictionary[i]);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteStartElement("Exclude");
            writer.WriteAttributeString("Length", this.Exclude.Length.ToString());
            for (var i = 0; i < this.Exclude.Length; i++)
            {
                writer.WriteStartElement("item");
                writer.WriteValue(this.Exclude[i]);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}