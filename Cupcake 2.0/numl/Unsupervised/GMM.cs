// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="GMM.cs">
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
//   A gmm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Unsupervised
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using numl.Math.LinearAlgebra;
    using numl.Math.Metrics;
    using numl.Model;

    /// <summary>A gmm.</summary>
    public class GMM
    {
        #region Public Properties

        /// <summary>Gets or sets the descriptor.</summary>
        /// <value>The descriptor.</value>
        public Descriptor Descriptor { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generates.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="examples">The examples.</param>
        /// <param name="k">The int to process.</param>
        public void Generate(IEnumerable<object> examples, int k)
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

            // Extract data
            var X = this.Descriptor.ToMatrix(examples);

            // generate model
            this.Generate(X, k);
        }

        /// <summary>Generates.</summary>
        /// <param name="X">The Matrix to process.</param>
        /// <param name="k">The int to process.</param>
        public void Generate(Matrix X, int k)
        {
            var n = X.Rows;
            var d = X.Cols;

            /***********************
             * initialize parameters
             ***********************/
            // convergence params
            var log_probability = 0d;
            var probability_difference = double.MaxValue;
            var mu_difference = double.MaxValue;

            // initialize centers with KMeans
            var kmeans = new KMeans();
            var asgn = kmeans.Generate(X, k, new EuclidianDistance());

            // tentative centers
            var mu_k = kmeans.Centers;

            // initial covariances (stored as diag(cov) 1 of k)
            var sg_k = new Matrix(k, d);
            for (var i = 0; i < k; i++)
            {
                var indices =
                    asgn.Select((a, b) => new Tuple<int, int>(a, b)).Where(t => t.Item1 == i).Select(t => t.Item2);
                var matrix = X.Slice(indices, VectorType.Row);
                sg_k[i] = matrix.CovarianceDiag();
            }

            // mixing coefficient
            var pi_k =
                asgn.OrderBy(i => i).GroupBy(j => j).Select(g => (double)g.Count() / (double)asgn.Length).ToVector();

            var max_iter = 100;
            do
            {
                /***********************
                 * Expectation Step
                 ***********************/
                // responsibilty matrix: how much is gaussian k responsible for this point x
                var z_nk = new Matrix(n, k);
                for (var i = 0; i < n; i++)
                {
                    // pi_j * N(x_n | mu_j, sigma_j)
                    for (var j = 0; j < k; j++)
                    {
                        z_nk[i, j] = pi_k[j] * this.Normal(X[i], mu_k[j], sg_k[j]);
                    }

                    var dn = z_nk[i].Sum();

                    if (dn == 0)
                    {
                        Console.WriteLine("Uh oh....");
                    }

                    z_nk[i].Each(z => z / dn);
                }

                /***********************
                 * Maximization Step
                 ***********************/
                var N_k = z_nk.Sum(VectorType.Row);

                var mu_k_new = new Matrix(mu_k.Rows, mu_k.Cols);
                for (var i = 0; i < k; i++)
                {
                    var sum = Vector.Zeros(d);
                    for (var j = 0; j < n; j++)
                    {
                        sum += z_nk[j, i] * X[j];
                    }

                    mu_k_new[i] = sum / N_k[i];
                }

                var sg_k_new = new Matrix(k, d);
                for (var i = 0; i < k; i++)
                {
                    var sum = Vector.Zeros(d);
                    for (var j = 0; j < n; j++)
                    {
                        sum += z_nk[j, i] * (X[j] - mu_k_new[i]).Each(s => s * s);
                    }

                    sg_k_new[i] = sum / N_k[i];
                }

                var pi_k_new = N_k / n;

                /***********************
                 * Convergence Check
                 ***********************/
                var new_log_prob = 0d;
                for (var i = 0; i < n; i++)
                {
                    var acc = 0d;

                    // pi_j * N(x_n | mu_j, sigma_j)
                    for (var j = 0; j < k; j++)
                    {
                        acc += pi_k[j] * this.Normal(X[i], mu_k[j], sg_k[j]);
                    }

                    new_log_prob += Math.Log(acc, Math.E);
                }

                // log likelihood differences
                probability_difference = Math.Abs(log_probability - new_log_prob);
                Console.WriteLine(
                    "Log Likelihoods (Total Points: {0}, k={1}, d={2})\nO: {3}\nN: {4}\nDifference: {5}\n", 
                    n, 
                    k, 
                    d, 
                    log_probability, 
                    new_log_prob, 
                    probability_difference);
                log_probability = new_log_prob;

                // centers differences
                mu_difference =
                    mu_k.GetRows()
                        .Zip(mu_k_new.GetRows(), (v1, v2) => new { V1 = v1, V2 = v2 })
                        .Sum(a => (a.V1 - a.V2).Norm());

                Console.WriteLine("Centers:\nO: {0}\nN: {1}\nDifference: {2}\n", mu_k, mu_k_new, mu_difference);
                mu_k = mu_k_new;

                // covariance differences
                var diff =
                    sg_k.GetRows()
                        .Zip(sg_k_new.GetRows(), (v1, v2) => new { V1 = v1, V2 = v2 })
                        .Sum(a => (a.V1 - a.V2).Norm());

                Console.WriteLine("Covariance:\nO: {0}\nN: {1}\nDifference: {2}\n", sg_k, sg_k_new, diff);
                sg_k = sg_k_new;

                // mixing differences
                diff = (pi_k - pi_k_new).Each(s => Math.Abs(s)).Sum();
                Console.WriteLine("Mixing Coeffs:\nO: {0}\nN: {1}\nDifference: {2}\n", pi_k, pi_k_new, diff);
                pi_k = pi_k_new;

                Console.WriteLine("-------------------------------------------------------------");
            }
            while (probability_difference > .0000000001 && mu_difference > .0000000001 && --max_iter >= 0);
        }

        /// <summary>Compute probability according to multivariate Gaussian.</summary>
        /// <param name="x">Vector in question.</param>
        /// <param name="mu">Mean.</param>
        /// <param name="sigma">diag(covariance)</param>
        /// <returns>Probability.</returns>
        public double Normal(Vector x, Vector mu, Vector sigma)
        {
            var p = 1 / sqrt(pow(2 * Math.PI, mu.Length) * sigma.Prod());
            var exp = -0.5d * ((x - mu) * sigma.Each(d => 1 / d, true)).Dot(x - mu);
            var e_exp = pow(Math.E, exp);
            return p * e_exp;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     TODO The pow.
        /// </summary>
        /// <param name="a">
        ///     TODO The a.
        /// </param>
        /// <param name="d">
        ///     TODO The d.
        /// </param>
        /// <returns>
        /// </returns>
        private static double pow(double a, double d)
        {
            return Math.Pow(a, d);
        }

        /// <summary>
        ///     TODO The sqrt.
        /// </summary>
        /// <param name="d">
        ///     TODO The d.
        /// </param>
        /// <returns>
        /// </returns>
        private static double sqrt(double d)
        {
            return Math.Sqrt(d);
        }

        #endregion
    }
}