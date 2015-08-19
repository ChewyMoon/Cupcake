﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Impurity.cs">
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
//   An impurity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.Information
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using numl.Math.LinearAlgebra;

    /// <summary>An impurity.</summary>
    public abstract class Impurity
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether the discrete.</summary>
        /// <value>true if discrete, false if not.</value>
        public bool Discrete { get; set; }

        /// <summary>
        ///     Calculated ranges used for segmented splits. This is generated when a segmented conditional
        ///     impurity value is calculated.
        /// </summary>
        /// <value>The segments.</value>
        public Range[] Segments { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Calculates impurity measure of x.</summary>
        /// <param name="x">The list in question.</param>
        /// <returns>Impurity measure.</returns>
        public abstract double Calculate(Vector x);

        /// <summary>
        ///     Calculates conditional impurity of y | x R(Y|X) is the average of H(Y|X = x) over all
        ///     possible values X may take.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="y">Target impurity.</param>
        /// <param name="x">Conditioned impurity.</param>
        /// <returns>Conditional impurity measure.</returns>
        public double Conditional(Vector y, Vector x)
        {
            if (x == null && y == null)
            {
                throw new InvalidOperationException("x and y do not exist!");
            }

            double p = 0, 

                   // probability of slice
                   h = 0, 

                   // impurity of y | x_i : ith slice
                   result = 0, 

                   // aggregated sum
                   count = x.Count(); // total items in list

            var values = x.Distinct().OrderBy(z => z); // distinct values to split on

            this.Segments = values.Select(z => Range.Make(z, z)).ToArray();
            this.Discrete = true;

            // for each distinct value 
            // calculate conditional impurity
            // and aggregate results
            foreach (var i in values)
            {
                // get slice
                var s = x.Indices(d => d == i);

                // slice probability
                p = (double)s.Count() / (double)count;

                // impurity of (y | x_i)
                h = this.Calculate(y.Slice(s));

                // sum up
                result += p * h;
            }

            return result;
        }

        /// <summary>Calculates information gain of y | x.</summary>
        /// <param name="y">Target impurity.</param>
        /// <param name="x">Conditioned impurity.</param>
        /// <returns>Information gain using appropriate measure.</returns>
        public double Gain(Vector y, Vector x)
        {
            return this.Calculate(y) - this.Conditional(y, x);
        }

        /// <summary>Calculates relative information gain of y | x.</summary>
        /// <param name="y">Target impurity.</param>
        /// <param name="x">Conditioned impurity.</param>
        /// <returns>Relative information gain using appropriate measure.</returns>
        public double RelativeGain(Vector y, Vector x)
        {
            var h_yx = this.Conditional(y, x);
            var h_y = this.Calculate(y);
            return (h_y - h_yx) / h_y;
        }

        /// <summary>
        ///     Calculates segmented conditional impurity of y | x When stipulating segments (s), X is broken
        ///     up into s many segments therefore P(X=x_s) becomes a range probability rather than a fixed
        ///     probability. In essence the average over H(Y|X = x) becomes SUM_s [ p_s * H(Y|X = x_s) ]. The
        ///     values that were used to do the split are stored in the Splits member.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="y">Target impurity.</param>
        /// <param name="x">Conditioned impurity.</param>
        /// <param name="segments">Number of segments over x to condition upon.</param>
        /// <returns>Segmented conditional impurity measure.</returns>
        public double SegmentedConditional(Vector y, Vector x, int segments)
        {
            if (x == null && y == null)
            {
                throw new InvalidOperationException("x and y do not exist!");
            }

            return this.SegmentedConditional(y, x, x.Segment(segments));
        }

        /// <summary>
        ///     Calculates segmented conditional impurity of y | x When stipulating ranges (r), X is broken
        ///     up into
        ///     |r| many segments therefore P(X=x_r) becomes a range probability
        ///     rather than a fixed probability. In essence the average over H(Y|X = x) becomes SUM_s [ p_r *
        ///     H(Y|X = x_r) ]. The values that were used to do the split are stored in the Splits member.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="y">Target impurity.</param>
        /// <param name="x">Conditioned impurity.</param>
        /// <param name="ranges">Number of segments over x to condition upon.</param>
        /// <returns>Segmented conditional impurity measure.</returns>
        public double SegmentedConditional(Vector y, Vector x, IEnumerable<Range> ranges)
        {
            if (x == null && y == null)
            {
                throw new InvalidOperationException("x and y do not exist!");
            }

            double p = 0, 

                   // probability of slice
                   h = 0, 

                   // impurity of y | x_i : ith slice
                   result = 0, 

                   // aggregated sum
                   count = x.Count(); // total items in list

            this.Segments = ranges.OrderBy(r => r.Min).ToArray();
            this.Discrete = false;

            // for each range calculate
            // conditional impurity and
            // aggregate results
            foreach (var range in this.Segments)
            {
                // get slice
                var s = x.Indices(d => d >= range.Min && d < range.Max);

                // slice probability
                p = (double)s.Count() / (double)count;

                // impurity of (y | x_i)
                h = this.Calculate(y.Slice(s));

                // sum up
                result += p * h;
            }

            return result;
        }

        /// <summary>Segmented gain.</summary>
        /// <param name="y">Target impurity.</param>
        /// <param name="x">The list in question.</param>
        /// <param name="segments">Number of segments over x to condition upon.</param>
        /// <returns>A double.</returns>
        public double SegmentedGain(Vector y, Vector x, int segments)
        {
            return this.Calculate(y) - this.SegmentedConditional(y, x, segments);
        }

        /// <summary>Segmented gain.</summary>
        /// <param name="y">Target impurity.</param>
        /// <param name="x">The list in question.</param>
        /// <param name="ranges">Number of segments over x to condition upon.</param>
        /// <returns>A double.</returns>
        public double SegmentedGain(Vector y, Vector x, IEnumerable<Range> ranges)
        {
            return this.Calculate(y) - this.SegmentedConditional(y, x, ranges);
        }

        /// <summary>
        ///     Calculates relative information gain of y | x with a specified number of segments.
        /// </summary>
        /// <param name="y">Target impurity.</param>
        /// <param name="x">Conditioned impurity.</param>
        /// <param name="segments">Number of segments.</param>
        /// <returns>Relative segmented information gain using appropriate measure.</returns>
        public double SegmentedRelativeGain(Vector y, Vector x, int segments)
        {
            var h_yx = this.SegmentedConditional(y, x, segments);
            var h_y = this.Calculate(y);
            return (h_y - h_yx) / h_y;
        }

        /// <summary>Calculates relative information gain of y | x with under specified ranges.</summary>
        /// <param name="y">Target impurity.</param>
        /// <param name="x">Conditioned impurity.</param>
        /// <param name="ranges">Range breakdown.</param>
        /// <returns>Relative segmented information gain using appropriate measure.</returns>
        public double SegmentedRelativeGain(Vector y, Vector x, IEnumerable<Range> ranges)
        {
            var h_yx = this.SegmentedConditional(y, x, ranges);
            var h_y = this.Calculate(y);
            return (h_y - h_yx) / h_y;
        }

        #endregion
    }
}