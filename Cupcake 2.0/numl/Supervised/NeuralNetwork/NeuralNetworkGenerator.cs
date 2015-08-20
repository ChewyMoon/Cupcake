// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NeuralNetworkGenerator.cs" company="ChewyMoon">
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
//   A neural network generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.NeuralNetwork
{
    using numl.Math.Functions;
    using numl.Math.LinearAlgebra;

    /// <summary>A neural network generator.</summary>
    public class NeuralNetworkGenerator : Generator
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NeuralNetworkGenerator" /> class. Default constructor.
        /// </summary>
        public NeuralNetworkGenerator()
        {
            this.LearningRate = 0.9;
            this.MaxIterations = -1;
            this.Activation = new Tanh();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the activation.</summary>
        /// <value>The activation.</value>
        public IFunction Activation { get; set; }

        /// <summary>Gets or sets the learning rate.</summary>
        /// <value>The learning rate.</value>
        public double LearningRate { get; set; }

        /// <summary>Gets or sets the maximum iterations.</summary>
        /// <value>The maximum iterations.</value>
        public int MaxIterations { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generate model based on a set of examples.</summary>
        /// <param name="x">The Matrix to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>Model.</returns>
        public override IModel Generate(Matrix x, Vector y)
        {
            // because I said so...
            if (this.MaxIterations == -1)
            {
                this.MaxIterations = x.Rows * 1000;
            }

            var network = Network.Default(this.Descriptor, x, y, this.Activation);
            var model = new NeuralNetworkModel { Descriptor = this.Descriptor, Network = network };
            this.OnModelChanged(this, ModelEventArgs.Make(model, "Initialized"));

            for (var i = 0; i < this.MaxIterations; i++)
            {
                var idx = i % x.Rows;
                network.Forward(x[idx, VectorType.Row]);

                // OnModelChanged(this, ModelEventArgs.Make(model, "Forward"));
                network.Back(y[idx], this.LearningRate);
                var output = string.Format("Run ({0}/{1})", i, this.MaxIterations);
                this.OnModelChanged(this, ModelEventArgs.Make(model, output));
            }

            return model;
        }

        #endregion
    }
}