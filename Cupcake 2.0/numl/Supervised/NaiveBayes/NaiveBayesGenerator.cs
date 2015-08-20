// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaiveBayesGenerator.cs" company="ChewyMoon">
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
//   A naive bayes generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.NaiveBayes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using numl.Math.LinearAlgebra;

    /// <summary>A naive bayes generator.</summary>
    public class NaiveBayesGenerator : Generator
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NaiveBayesGenerator" /> class. Constructor.
        /// </summary>
        /// <param name="width">
        ///     The width.
        /// </param>
        public NaiveBayesGenerator(int width)
        {
            this.Width = width;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the width.</summary>
        /// <value>The width.</value>
        public int Width { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generate model based on a set of examples.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The Matrix to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>Model.</returns>
        public override IModel Generate(Matrix x, Vector y)
        {
            if (this.Descriptor == null)
            {
                throw new InvalidOperationException("Cannot build naive bayes model without type knowledge!");
            }

            // create answer probabilities
            if (!this.Descriptor.Label.Discrete)
            {
                throw new InvalidOperationException("Need to use regression for non-discrete labels!");
            }

            // compute Y probabilities
            var statistics = this.GetLabelStats(y);

            var root = new Measure { Discrete = true, Label = this.Descriptor.Label.Name, Probabilities = statistics };

            // collect feature ranges
            var features = this.GetBaseConditionals(x);

            // compute conditional counts
            for (var i = 0; i < y.Length; i++)
            {
                var stat = statistics.Where(s => s.X.Min == y[i]).First();
                if (stat.Conditionals == null)
                {
                    stat.Conditionals = this.CloneMeasure(features);
                }

                for (var j = 0; j < x.Cols; j++)
                {
                    var s = stat.Conditionals[j];
                    s.Increment(x[i, j]);
                }
            }

            // normalize into probabilities
            for (var i = 0; i < statistics.Length; i++)
            {
                var cond = statistics[i];
                for (var j = 0; j < cond.Conditionals.Length; j++)
                {
                    cond.Conditionals[j].Normalize();
                }
            }

            return new NaiveBayesModel { Descriptor = this.Descriptor, Root = root };
        }

        #endregion

        #region Methods

        /// <summary>Clone measure.</summary>
        /// <param name="measures">The measures.</param>
        /// <returns>A Measure[].</returns>
        private Measure[] CloneMeasure(Measure[] measures)
        {
            var m = new Measure[measures.Length];
            for (var i = 0; i < m.Length; i++)
            {
                m[i] = measures[i].Clone();
            }

            return m;
        }

        /// <summary>Gets base conditionals.</summary>
        /// <param name="x">The Matrix to process.</param>
        /// <returns>An array of measure.</returns>
        private Measure[] GetBaseConditionals(Matrix x)
        {
            var features = new Measure[x.Cols];
            for (var i = 0; i < features.Length; i++)
            {
                var p = this.Descriptor.At(i);
                var f = new Measure { Discrete = p.Discrete, Label = this.Descriptor.ColumnAt(i), };

                IEnumerable<Statistic> fstats;
                if (f.Discrete)
                {
                    fstats =
                        x[i, VectorType.Col].Distinct()
                            .OrderBy(d => d)
                            .Select(d => Statistic.Make(p.Convert(d).ToString(), d, 1));
                }
                else
                {
                    fstats = x[i, VectorType.Col].Segment(this.Width).Select(d => Statistic.Make(f.Label, d, 1));
                }

                f.Probabilities = fstats.ToArray();
                features[i] = f;
            }

            return features;
        }

        /// <summary>Gets label statistics.</summary>
        /// <param name="y">The Vector to process.</param>
        /// <returns>An array of statistic.</returns>
        private Statistic[] GetLabelStats(Vector y)
        {
            var stats = y.Stats();
            var statistics = new Statistic[stats.Rows];
            for (var i = 0; i < statistics.Length; i++)
            {
                var yVal = stats[i, 0];
                var s = Statistic.Make(this.Descriptor.Label.Convert(stats[i, 0]).ToString(), yVal);
                s.Count = (int)stats[i, 1];
                s.Probability = stats[i, 2];
                statistics[i] = s;
            }

            return statistics;
        }

        #endregion
    }
}