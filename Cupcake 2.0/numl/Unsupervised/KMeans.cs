// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="KMeans.cs">
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
//   A means.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Unsupervised
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using numl.Math.LinearAlgebra;
    using numl.Math.Metrics;
    using numl.Math.Probability;
    using numl.Model;

    /// <summary>A means.</summary>
    public class KMeans
    {
        #region Public Properties

        /// <summary>Gets or sets the centers.</summary>
        /// <value>The centers.</value>
        public Matrix Centers { get; set; }

        /// <summary>Gets or sets the descriptor.</summary>
        /// <value>The descriptor.</value>
        public Descriptor Descriptor { get; set; }

        /// <summary>Gets or sets the x coordinate.</summary>
        /// <value>The x coordinate.</value>
        public Matrix X { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generates.</summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="examples">The examples.</param>
        /// <param name="k">The int to process.</param>
        /// <param name="metric">(Optional) the metric.</param>
        /// <returns>An int[].</returns>
        public Cluster Generate(Descriptor descriptor, IEnumerable<object> examples, int k, IDistance metric = null)
        {
            var data = examples.ToArray();
            this.Descriptor = descriptor;
            this.X = this.Descriptor.Convert(data).ToMatrix();

            // generate assignments
            var assignments = this.Generate(this.X, k, metric);

            // begin packing objects into clusters
            var objects = new List<object>[k];
            for (var i = 0; i < assignments.Length; i++)
            {
                var a = assignments[i];
                if (objects[a] == null)
                {
                    objects[a] = new List<object>();
                }

                objects[a].Add(data[i]);
            }

            // create clusters
            var clusters = new List<Cluster>();
            for (var i = 0; i < k; i++)
            {
                if (!this.Centers[i].IsNaN())
                {
                    // check for degenerate clusters
                    clusters.Add(
                        new Cluster
                            {
                                Id = i + 1, Center = this.Centers[i], Members = objects[i].ToArray(), 
                                Children = new Cluster[] { }
                            });
                }
            }

            // return single cluster with K children
            return new Cluster { Id = 0, Center = this.X.Mean(VectorType.Row), Children = clusters.ToArray() };
        }

        /// <summary>Generates.</summary>
        /// <param name="x">The Matrix to process.</param>
        /// <param name="k">The int to process.</param>
        /// <param name="metric">the metric.</param>
        /// <returns>An int[].</returns>
        public int[] Generate(Matrix x, int k, IDistance metric)
        {
            if (metric == null)
            {
                metric = new EuclidianDistance();
            }

            this.X = x;

            var means = this.InitializeRandom(this.X, k);
            var assignments = new int[this.X.Rows];

            for (var i = 0; i < 100; i++)
            {
                // Assignment step
                Parallel.For(
                    0, 
                    this.X.Rows, 
                    j =>
                        {
                            var min_index = -1;
                            var min = double.MaxValue;
                            for (var m = 0; m < means.Rows; m++)
                            {
                                var d = metric.Compute(this.X[j], means[m]);
                                if (d < min)
                                {
                                    min = d;
                                    min_index = m;
                                }
                            }

                            // bounds?
                            if (min_index == -1)
                            {
                                min_index = 0;
                            }

                            assignments[j] = min_index;
                        });

                // Update Step
                var new_means = Matrix.Zeros(k, this.X.Cols);
                var sum = Vector.Zeros(k);

                // Part 1: Sum up assignments
                for (var j = 0; j < this.X.Rows; j++)
                {
                    var a = assignments[j];
                    new_means[a] += this.X[j, VectorType.Row];
                    sum[a]++;
                }

                // Part 2: Divide by counts
                for (var j = 0; j < new_means.Rows; j++)
                {
                    new_means[j] /= sum[j];
                }

                // Part 3: Check for convergence
                // find norm of the difference
                if ((means - new_means).Norm() < .00001)
                {
                    break;
                }

                means = new_means;
            }

            this.Centers = means;

            return assignments;
        }

        /// <summary>Generates.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="examples">The examples.</param>
        /// <param name="k">The int to process.</param>
        /// <param name="metric">(Optional) the metric.</param>
        /// <returns>An int[].</returns>
        public int[] Generate(IEnumerable<object> examples, int k, IDistance metric = null)
        {
            if (examples == null)
            {
                throw new InvalidOperationException("Cannot generate a model will no data!");
            }

            if (k < 2)
            {
                throw new InvalidOperationException("Can only cluter with k > 1");
            }

            if (this.Descriptor == null)
            {
                throw new InvalidOperationException("Invalid Description!");
            }

            var count = examples.Count();
            if (k >= count)
            {
                throw new InvalidOperationException(
                    string.Format("Cannot cluster {0} items {1} different ways!", count, k));
            }

            var X = this.Descriptor.Convert(examples).ToMatrix();
            var data = this.Generate(X, k, metric);
            return data;
        }

        #endregion

        #region Methods

        /// <summary>Initializes the random.</summary>
        /// <param name="X">The Matrix to process.</param>
        /// <param name="k">The int to process.</param>
        /// <returns>A Matrix.</returns>
        private Matrix InitializeRandom(Matrix X, int k)
        {
            // initialize mean variables
            // to random existing points
            var m = Matrix.Zeros(k, X.Cols);

            var seeds = new List<int>(k);
            for (var i = 0; i < k; i++)
            {
                var index = -1;
                do
                {
                    // pick random row that has not yet 
                    // been used (need to fix this...)
                    index = Sampling.GetUniform(k);

                    if (!seeds.Contains(index))
                    {
                        seeds.Add(index);
                        break;
                    }
                }
                while (true);

                for (var j = 0; j < X.Cols; j++)
                {
                    m[i, j] = X[index, j];
                }
            }

            return m;
        }

        /// <summary>Initializes the uniform.</summary>
        /// <param name="X">The Matrix to process.</param>
        /// <param name="k">The int to process.</param>
        /// <returns>A Matrix.</returns>
        private Matrix InitializeUniform(Matrix X, int k)
        {
            var multiple = (int)Math.Floor((double)X.Rows / (double)k);

            var m = Matrix.Zeros(k, X.Cols);

            for (var i = 0; i < k; i++)
            {
                for (var j = 0; j < X.Cols; j++)
                {
                    m[i, j] = X[i * multiple, j];
                }
            }

            return m;
        }

        #endregion
    }
}