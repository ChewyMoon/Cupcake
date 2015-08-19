// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Conversions.cs">
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
//   A conversions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.LinearAlgebra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>A conversions.</summary>
    public static class Conversions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     An IEnumerable&lt;IEnumerable&lt;double&gt;&gt; extension method that converts a matrix to
        ///     the examples.
        /// </summary>
        /// <param name="matrix">The matrix to act on.</param>
        /// <returns>matrix as a Tuple&lt;Matrix,Vector&gt;</returns>
        public static Tuple<Matrix, Vector> ToExamples(this IEnumerable<IEnumerable<double>> matrix)
        {
            // materialize
            var x = (from v in matrix select v.ToArray()).ToArray();

            // determine matrix
            // size and type
            var m = Build(x, true); // clip last col

            // fill 'er up!
            for (var i = 0; i < m.Rows; i++)
            {
                for (var j = 0; j < m.Cols; j++)
                {
                    if (j >= x[i].Length)
                    {
                        // over bound limits
                        m[i, j] = 0; // pad overlow to 0
                    }
                    else
                    {
                        m[i, j] = x[i][j];
                    }
                }
            }

            // fill up vector
            var y = Vector.Zeros(m.Rows);
            for (var i = 0; i < m.Rows; i++)
            {
                if (m.Cols >= x[i].Length)
                {
                    y[i] = 0; // pad overflow to 0
                }
                else
                {
                    y[i] = x[i][m.Cols];
                }
            }

            return new Tuple<Matrix, Vector>(m, y);
        }

        /// <summary>
        ///     An IEnumerable&lt;IEnumerable&lt;double&gt;&gt; extension method that converts a matrix to a
        ///     matrix.
        /// </summary>
        /// <param name="matrix">The matrix to act on.</param>
        /// <returns>matrix as a Matrix.</returns>
        public static Matrix ToMatrix(this IEnumerable<IEnumerable<double>> matrix)
        {
            // materialize
            var x = (from v in matrix select v.ToArray()).ToArray();

            // determine matrix
            // size and type
            var m = Build(x);

            // fill 'er up!
            for (var i = 0; i < m.Rows; i++)
            {
                for (var j = 0; j < m.Cols; j++)
                {
                    if (j >= x[i].Length)
                    {
                        // over bound limits
                        m[i, j] = 0; // pad overlow to 0
                    }
                    else
                    {
                        m[i, j] = x[i][j];
                    }
                }
            }

            return m;
        }

        #endregion

        #region Methods

        /// <summary>Builds.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The double[][] to process.</param>
        /// <param name="clip">(Optional) true to clip.</param>
        /// <returns>A Matrix.</returns>
        private static Matrix Build(double[][] x, bool clip = false)
        {
            // rows
            var n = x.Length;
            if (n == 0)
            {
                throw new InvalidOperationException("Empty matrix (n)");
            }

            // cols (being nice here...)
            var cols = x.Select(v => v.Length);
            var d = cols.Max();

            if (d == 0)
            {
                throw new InvalidOperationException("Empty matrix (d)");
            }

            // total zeros in matrix
            var zeros = (from v in x select v.Count(i => i == 0)).Sum();

            // if irregularities in jagged matrix, need to 
            // pad rows with less columns with additional
            // zeros by subtractic max width with each
            // individual row and getting the sum
            var pad = cols.Select(c => d - c).Sum();

            // check sparsity
            // var percent = (decimal)(zeros + pad) / (decimal)(n * d);
            var m = Matrix.Zeros(n, clip ? d - 1 : d);

            return m;
        }

        #endregion
    }
}