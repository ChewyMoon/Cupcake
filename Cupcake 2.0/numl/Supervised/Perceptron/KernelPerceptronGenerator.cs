// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="KernelPerceptronGenerator.cs">
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
//   A kernel perceptron generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.Perceptron
{
    using numl.Math.Kernels;
    using numl.Math.LinearAlgebra;

    /// <summary>A kernel perceptron generator.</summary>
    public class KernelPerceptronGenerator : Generator
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="KernelPerceptronGenerator" /> class. Constructor.
        /// </summary>
        /// <param name="kernel">
        ///     The kernel.
        /// </param>
        public KernelPerceptronGenerator(IKernel kernel)
        {
            this.Kernel = kernel;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the kernel.</summary>
        /// <value>The kernel.</value>
        public IKernel Kernel { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generate model based on a set of examples.</summary>
        /// <param name="x">The Matrix to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>Model.</returns>
        public override IModel Generate(Matrix x, Vector y)
        {
            var N = y.Length;
            var a = Vector.Zeros(N);

            // compute kernel
            var K = this.Kernel.Compute(x);

            var n = 1;

            // hopefully enough to converge right? ;)
            // need to be smarter about storing SPD kernels...
            var found_error = true;
            while (n < 500 && found_error)
            {
                found_error = false;
                for (var i = 0; i < N; i++)
                {
                    found_error = y[i] * a.Dot(K[i]) <= 0;
                    if (found_error)
                    {
                        a[i] += y[i];
                    }
                }

                n++;
            }

            // anything that *matters*
            // i.e. support vectors
            var indices = a.Indices(d => d != 0);

            // slice up examples to contain
            // only support vectors
            return new KernelPerceptronModel
                       {
                          Kernel = this.Kernel, A = a.Slice(indices), Y = y.Slice(indices), X = x.Slice(indices) 
                       };
        }

        #endregion
    }
}