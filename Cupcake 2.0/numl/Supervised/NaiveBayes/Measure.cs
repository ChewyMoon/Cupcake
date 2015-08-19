// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Measure.cs">
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
//   A measure.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.NaiveBayes
{
    using System;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>A measure.</summary>
    [XmlRoot("Measure")]
    public class Measure
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether the discrete.</summary>
        /// <value>true if discrete, false if not.</value>
        [XmlAttribute("Discrete")]
        public bool Discrete { get; set; }

        /// <summary>Gets or sets the label.</summary>
        /// <value>The label.</value>
        [XmlAttribute("Label")]
        public string Label { get; set; }

        /// <summary>Gets or sets the probabilities.</summary>
        /// <value>The probabilities.</value>
        [XmlArray("Probabilities")]
        public Statistic[] Probabilities { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        public Measure Clone()
        {
            var m = new Measure { Label = this.Label, Discrete = this.Discrete };

            if (this.Probabilities != null && this.Probabilities.Length > 0)
            {
                m.Probabilities = new Statistic[this.Probabilities.Length];
                for (var i = 0; i < m.Probabilities.Length; i++)
                {
                    m.Probabilities[i] = this.Probabilities[i].Clone();
                }
            }

            return m;
        }

        /// <summary>Tests if this object is considered equal to another.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the objects are considered equal, false if they are not.</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Measure))
            {
                return false;
            }

            var measure = obj as Measure;
            if (this.Label != measure.Label)
            {
                return false;
            }

            if (this.Discrete != measure.Discrete)
            {
                return false;
            }

            if (this.Probabilities == null && measure.Probabilities != null)
            {
                return false;
            }

            if (measure.Probabilities == null && this.Probabilities != null)
            {
                return false;
            }

            if (this.Probabilities != null)
            {
                if (this.Probabilities.Length != measure.Probabilities.Length)
                {
                    return false;
                }

                for (var i = 0; i < this.Probabilities.Length; i++)
                {
                    if (!this.Probabilities[i].Equals(measure.Probabilities[i]))
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
            return string.Format("{0} [{1}]", this.Label, this.Discrete ? "Discrete" : "Continuous");
        }

        #endregion

        #region Methods

        /// <summary>Gets a probability.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The x coordinate.</param>
        /// <returns>The probability.</returns>
        internal double GetProbability(double x)
        {
            var p = this.GetStatisticFor(x);
            if (p == null)
            {
                throw new InvalidOperationException("Range not found!");
            }

            return p.Probability;
        }

        /// <summary>Gets statistic for.</summary>
        /// <exception cref="IndexOutOfRangeException">
        ///     Thrown when the index is outside the required
        ///     range.
        /// </exception>
        /// <param name="x">The x coordinate.</param>
        /// <returns>The statistic for.</returns>
        internal Statistic GetStatisticFor(double x)
        {
            if (this.Probabilities == null || this.Probabilities.Length == 0)
            {
                throw new IndexOutOfRangeException("Invalid statistics");
            }

            var p = this.Probabilities.Where(s => s.X.Test(x)).FirstOrDefault();

            return p;
        }

        /// <summary>Increments.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The x coordinate.</param>
        internal void Increment(double x)
        {
            var p = this.GetStatisticFor(x);
            if (p == null)
            {
                throw new InvalidOperationException("Range not found!");
            }

            p.Count++;
        }

        /// <summary>Normalizes this object.</summary>
        internal void Normalize()
        {
            double total = this.Probabilities.Select(p => p.Count).Sum();
            for (var i = 0; i < this.Probabilities.Length; i++)
            {
                this.Probabilities[i].Probability = (double)this.Probabilities[i].Count / total;
            }
        }

        #endregion
    }
}