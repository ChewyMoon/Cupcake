// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Statistic.cs">
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
//   A statistic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.NaiveBayes
{
    using System.Xml.Serialization;

    using numl.Math;

    /// <summary>A statistic.</summary>
    [XmlRoot("Statistic")]
    public class Statistic
    {
        #region Public Properties

        /// <summary>Gets or sets the conditionals.</summary>
        /// <value>The conditionals.</value>
        [XmlArray("Conditionals")]
        public Measure[] Conditionals { get; set; }

        /// <summary>Gets or sets the number of. </summary>
        /// <value>The count.</value>
        [XmlAttribute("Count")]
        public int Count { get; set; }

        /// <summary>Gets or sets a value indicating whether the discrete.</summary>
        /// <value>true if discrete, false if not.</value>
        [XmlAttribute("Discrete")]
        public bool Discrete { get; set; }

        /// <summary>Gets or sets the label.</summary>
        /// <value>The label.</value>
        [XmlAttribute("Label")]
        public string Label { get; set; }

        /// <summary>Gets or sets the probability.</summary>
        /// <value>The probability.</value>
        [XmlAttribute("Probability")]
        public double Probability { get; set; }

        /// <summary>Gets or sets the x coordinate.</summary>
        /// <value>The x coordinate.</value>
        [XmlElement("Range")]
        public Range X { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Makes.</summary>
        /// <param name="label">The label.</param>
        /// <param name="x">The Range to process.</param>
        /// <param name="count">(Optional) number of.</param>
        /// <returns>A Statistic.</returns>
        public static Statistic Make(string label, Range x, int count = 0)
        {
            return new Statistic { Label = label, Discrete = false, Count = count, X = x };
        }

        /// <summary>Makes.</summary>
        /// <param name="label">The label.</param>
        /// <param name="val">The value.</param>
        /// <param name="count">(Optional) number of.</param>
        /// <returns>A Statistic.</returns>
        public static Statistic Make(string label, double val, int count = 0)
        {
            return new Statistic { Label = label, Discrete = true, Count = count, X = Range.Make(val) };
        }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        public Statistic Clone()
        {
            var s = new Statistic
                        {
                            Label = this.Label, Discrete = this.Discrete, Count = this.Count, X = this.X, 
                            Probability = this.Probability
                        };

            if (this.Conditionals != null && this.Conditionals.Length > 0)
            {
                s.Conditionals = new Measure[this.Conditionals.Length];
                for (var i = 0; i < s.Conditionals.Length; i++)
                {
                    s.Conditionals[i] = this.Conditionals[i].Clone();
                }
            }

            return s;
        }

        /// <summary>Tests if this object is considered equal to another.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the objects are considered equal, false if they are not.</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Statistic))
            {
                return false;
            }

            var statistic = obj as Statistic;
            if (this.Label != statistic.Label)
            {
                return false;
            }

            if (this.Discrete != statistic.Discrete)
            {
                return false;
            }

            if (this.Count != statistic.Count)
            {
                return false;
            }

            if (this.X.Max != statistic.X.Max)
            {
                return false;
            }

            if (this.X.Max != statistic.X.Max)
            {
                return false;
            }

            if (this.Probability != statistic.Probability)
            {
                return false;
            }

            if (this.Conditionals == null && statistic.Conditionals != null)
            {
                return false;
            }

            if (statistic.Conditionals == null && this.Conditionals != null)
            {
                return false;
            }

            if (this.Conditionals != null)
            {
                if (this.Conditionals.Length != statistic.Conditionals.Length)
                {
                    return false;
                }

                for (var i = 0; i < this.Conditionals.Length; i++)
                {
                    if (!this.Conditionals[i].Equals(statistic.Conditionals[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>Calculates a hash code for this object.</summary>
        /// <returns>A hash code for this object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format(
                "P({0}) = {1} [{2}, {3}]", 
                this.Label, 
                this.Probability, 
                this.Count, 
                this.Discrete ? this.X.Min.ToString() : this.X.ToString());
        }

        #endregion
    }
}