// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerceptronGenerator.cs" company="ChewyMoon">
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
//   A perceptron generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Supervised.Perceptron
{
    using numl.Math.LinearAlgebra;

    /// <summary>A perceptron generator.</summary>
    public class PerceptronGenerator : Generator
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PerceptronGenerator" /> class. Default constructor.
        /// </summary>
        public PerceptronGenerator()
        {
            this.Normalize = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PerceptronGenerator" /> class. Constructor.
        /// </summary>
        /// <param name="normalize">
        ///     true to normalize.
        /// </param>
        public PerceptronGenerator(bool normalize)
        {
            this.Normalize = normalize;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether the normalize.</summary>
        /// <value>true if normalize, false if not.</value>
        public bool Normalize { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generate model based on a set of examples.</summary>
        /// <param name="X">The Matrix to process.</param>
        /// <param name="Y">The Vector to process.</param>
        /// <returns>Model.</returns>
        public override IModel Generate(Matrix X, Vector Y)
        {
            var w = Vector.Zeros(X.Cols);
            var a = Vector.Zeros(X.Cols);

            double wb = 0;
            double ab = 0;

            var n = 1;

            if (this.Normalize)
            {
                X.Normalize(VectorType.Row);
            }

            // repeat 10 times for *convergence*
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < X.Rows; j++)
                {
                    var x = X[j];
                    var y = Y[j];

                    // perceptron update
                    if (y * (w.Dot(x) + wb) <= 0)
                    {
                        w = w + y * x;
                        wb += y;
                        a = (a + y * x) + n;
                        ab += y * n;
                    }

                    n += 1;
                }
            }

            return new PerceptronModel
                       {
                          W = w - (a / n), B = wb - (ab / n), Normalized = this.Normalize, Descriptor = this.Descriptor 
                       };
        }

        #endregion
    }
}