// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Range.cs" company="ChewyMoon">
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
//   A range.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math
{
    using System.Xml.Serialization;

    /// <summary>A range.</summary>
    [XmlRoot("Range")]
    public class Range
    {
        #region Public Properties

        /// <summary>Gets or sets the maximum.</summary>
        /// <value>The maximum value.</value>
        [XmlAttribute("Max")]
        public double Max { get; set; }

        /// <summary>Gets or sets the minimum.</summary>
        /// <value>The minimum value.</value>
        [XmlAttribute("Min")]
        public double Min { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Makes.</summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>A Range.</returns>
        public static Range Make(double min, double max)
        {
            return new Range { Min = min, Max = max };
        }

        /// <summary>Makes.</summary>
        /// <param name="min">The minimum.</param>
        /// <returns>A Range.</returns>
        public static Range Make(double min)
        {
            return new Range { Min = min, Max = min + 0.00001 };
        }

        /// <summary>Tests.</summary>
        /// <param name="d">The double to process.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public bool Test(double d)
        {
            return d >= this.Min && d < this.Max;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1})", this.Min, this.Max);
        }

        #endregion
    }
}