// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinearRegressionGenerator.cs" company="ChewyMoon">
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
//   A linear regression generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.Regression
{
    using numl.Math.Functions.Cost;
    using numl.Math.Functions.Regularization;
    using numl.Math.LinearAlgebra;
    using numl.Math.Optimization;
    using numl.PreProcessing;

    /// <summary>A linear regression generator.</summary>
    public class LinearRegressionGenerator : Generator
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearRegressionGenerator" /> class.
        ///     Initialise a new LinearRegressionGenerator
        /// </summary>
        public LinearRegressionGenerator()
        {
            this.MaxIterations = 500;
            this.LearningRate = 0.01;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The regularisation term Lambda
        /// </summary>
        public double Lambda { get; set; }

        /// <summary>Gets or sets the learning rate used with gradient descent.</summary>
        /// <para>The default value is 0.01</para>
        /// <value>The learning rate.</value>
        public double LearningRate { get; set; }

        /// <summary>Gets or sets the maximum iterations used with gradient descent.</summary>
        /// <para>The default is 500</para>
        /// <value>The maximum iterations.</value>
        public int MaxIterations { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generate Linear Regression model based on a set of examples.</summary>
        /// <param name="x">The Matrix to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>Model.</returns>
        public override IModel Generate(Matrix x, Vector y)
        {
            // create initial theta
            var theta = Vector.Ones(x.Cols + 1);
            var copy = x.Copy();

            // normalise features
            for (var i = 0; i < copy.Cols; i++)
            {
                var j = FeatureNormalizer.FeatureScale(copy[i, VectorType.Col]);
                for (var k = 0; k < copy.Rows; k++)
                {
                    copy[k, i] = j[k];
                }
            }

            // add intercept term
            copy = copy.Insert(Vector.Ones(copy.Rows), 0, VectorType.Col);

            // run gradient descent
            var run = GradientDescent.Run(
                theta, 
                copy, 
                y, 
                this.MaxIterations, 
                this.LearningRate, 
                new LinearCostFunction(), 
                this.Lambda, 
                new Regularization());

            // once converged create model and apply theta
            var model = new LinearRegressionModel(x.Mean(VectorType.Row), x.StdDev(VectorType.Row))
                            {
                               Descriptor = this.Descriptor, Theta = run.Item2 
                            };

            return model;
        }

        #endregion
    }
}