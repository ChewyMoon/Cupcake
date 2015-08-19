// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="MatrixOps.cs">
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
//   A matrix.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.LinearAlgebra
{
    using System;

    /// <summary>A matrix.</summary>
    public partial class Matrix
    {
        #region Public Methods and Operators

        /// <summary>Addition operator.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="m1">The first Matrix.</param>
        /// <param name="m2">The second Matrix.</param>
        /// <returns>The result of the operation.</returns>
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.Rows != m2.Rows || m1.Cols != m2.Cols)
            {
                throw new InvalidOperationException("Dimensions do not match");
            }

            var result = Zeros(m1.Rows, m1.Cols);
            for (var i = 0; i < m1.Rows; i++)
            {
                for (var j = 0; j < m1.Cols; j++)
                {
                    result[i, j] = m1[i, j] + m2[i, j];
                }
            }

            return result;
        }

        /// <summary>In memory addition of double to matrix.</summary>
        /// <param name="m">Matrix.</param>
        /// <param name="s">double.</param>
        /// <returns>The result of the operation.</returns>
        public static Matrix operator +(Matrix m, double s)
        {
            for (var i = 0; i < m.Rows; i++)
            {
                for (var j = 0; j < m.Cols; j++)
                {
                    m[i, j] += s;
                }
            }

            return m;
        }

        /// <summary>Addition operator.</summary>
        /// <param name="s">The double to process.</param>
        /// <param name="m">The Matrix to process.</param>
        /// <returns>The result of the operation.</returns>
        public static Matrix operator +(double s, Matrix m)
        {
            return m + s;
        }

        /// <summary>
        ///     Solves Ax = b for x If A is not square or the system is overdetermined, this operation solves
        ///     the linear least squares A.T * A x = A.T * b.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="A">Matrix A.</param>
        /// <param name="b">Vector b.</param>
        /// <returns>x.</returns>
        public static Vector operator /(Matrix A, Vector b)
        {
            if (A.Rows != b.Length)
            {
                throw new InvalidOperationException("Matrix row count does not match vector length!");
            }

            // LLS
            if (A.Rows != A.Cols)
            {
                var C = A.T * A;
                var L = C.Cholesky();
                var d = (A.T * b).ToVector();
                var z = Forward(L, d);
                var x = Backward(L.T, z);
                return x;
            }

            // regular solve
            else
            {
                // need to be smarter here....
                return ((A ^ -1) * b).ToVector();
            }
        }

        /// <summary>Division operator.</summary>
        /// <param name="A">The Matrix to process.</param>
        /// <param name="b">The double to process.</param>
        /// <returns>The result of the operation.</returns>
        public static Matrix operator /(Matrix A, double b)
        {
            for (var i = 0; i < A.Rows; i++)
            {
                for (var j = 0; j < A.Cols; j++)
                {
                    A[i, j] /= b;
                }
            }

            return A;
        }

        // --------------------- mathematical ops
        /// <summary>Equality operator.</summary>
        /// <param name="m1">The first Matrix.</param>
        /// <param name="m2">The second Matrix.</param>
        /// <returns>The result of the operation.</returns>
        public static bool operator ==(Matrix m1, Matrix m2)
        {
            return ReferenceEquals(m1, null) && ReferenceEquals(m2, null) || m1.Equals(m2);
        }

        /// <summary>
        ///     Matrix inverse using pivoted Gauss-Jordan elimination with partial pivoting
        ///     See:http://www.cse.illinois.edu/iem/linear_equations/gauss_jordan/for python implementaion.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="mat">Matrix.</param>
        /// <param name="n">-1.</param>
        /// <returns>Inverse (or exception if matrix is singular)</returns>
        public static Matrix operator ^(Matrix mat, int n)
        {
            if (mat.Rows != mat.Cols)
            {
                throw new InvalidOperationException("Can only find powers of square matrices!");
            }

            if (n == 0)
            {
                return Identity(mat.Rows, mat.Cols);
            }

            if (n == 1)
            {
                return mat.Copy();
            }

            if (n == -1)
            {
                // warning, this is not the most efficient way to solve a system of Linear Equations. Consider using the \ operator
                return Inverse(mat);
            }

            var negative = n < 0;
            var pow = Math.Abs(n);
            var scratch = mat.Copy();
            for (var i = 0; i < pow; i++)
            {
                scratch = scratch * mat;
            }

            if (!negative)
            {
                return scratch;
            }
            else
            {
                // warning, this is not the most efficient way to solve a system of Linear Equations. Consider using the \ operator
                return Inverse(scratch);
            }
        }

        // --------------------- implicity operators
        /// <summary>Matrix casting operator.</summary>
        /// <param name="m">Matrix.</param>
        public static implicit operator Matrix(double[,] m)
        {
            return new Matrix(m);
        }

        /// <summary>Matrix casting operator.</summary>
        /// <param name="m">Matrix.</param>
        public static implicit operator Matrix(int[,] m)
        {
            var matrix = new Matrix();
            matrix._asTransposeRef = false;
            matrix.Rows = m.GetLength(0);
            matrix.Cols = m.GetLength(1);
            matrix._matrix = new double[matrix.Rows][];
            for (var i = 0; i < matrix.Rows; i++)
            {
                matrix._matrix[i] = new double[matrix.Cols];
                for (var j = 0; j < matrix.Cols; j++)
                {
                    matrix._matrix[i][j] = m[i, j];
                }
            }

            return matrix;
        }

        /// <summary>Inequality operator.</summary>
        /// <param name="m1">The first Matrix.</param>
        /// <param name="m2">The second Matrix.</param>
        /// <returns>The result of the operation.</returns>
        public static bool operator !=(Matrix m1, Matrix m2)
        {
            return !m1.Equals(m2);
        }

        /// <summary>matrix multiplication.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="m1">left hand side.</param>
        /// <param name="m2">right hand side.</param>
        /// <returns>matrix.</returns>
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.Cols != m2.Rows)
            {
                throw new InvalidOperationException("Invalid multiplication dimenstion");
            }

            var result = Zeros(m1.Rows, m2.Cols);
            for (var i = 0; i < m1.Rows; i++)
            {
                for (var j = 0; j < m2.Cols; j++)
                {
                    for (var k = 0; k < m1.Cols; k++)
                    {
                        result[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }

            return result;
        }

        /// <summary>Scalar matrix multiplication.</summary>
        /// <param name="s">scalar.</param>
        /// <param name="m">matrix.</param>
        /// <returns>matrix.</returns>
        public static Matrix operator *(double s, Matrix m)
        {
            var result = Zeros(m.Rows, m.Cols);
            for (var i = 0; i < m.Rows; i++)
            {
                for (var j = 0; j < m.Cols; j++)
                {
                    result[i, j] = s * m[i, j];
                }
            }

            return result;
        }

        /// <summary>reverse.</summary>
        /// <param name="m">.</param>
        /// <param name="s">.</param>
        /// <returns>The result of the operation.</returns>
        public static Matrix operator *(Matrix m, double s)
        {
            return s * m;
        }

        /// <summary>Multiplication operator.</summary>
        /// <param name="m">The Matrix to process.</param>
        /// <param name="v">The Vector to process.</param>
        /// <returns>The result of the operation.</returns>
        public static Matrix operator *(Matrix m, Vector v)
        {
            var ans = Dot(m, v);
            return ans.ToMatrix(VectorType.Col);
        }

        /// <summary>Multiplication operator.</summary>
        /// <param name="v">The Vector to process.</param>
        /// <param name="m">The Matrix to process.</param>
        /// <returns>The result of the operation.</returns>
        public static Matrix operator *(Vector v, Matrix m)
        {
            var ans = Dot(v, m);
            return ans.ToMatrix(VectorType.Row);
        }

        /// <summary>Subtraction operator.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="m1">The first Matrix.</param>
        /// <param name="m2">The second Matrix.</param>
        /// <returns>The result of the operation.</returns>
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            if (m1.Rows != m2.Rows || m1.Cols != m2.Cols)
            {
                throw new InvalidOperationException("Dimensions do not match");
            }

            var result = Zeros(m1.Rows, m1.Cols);
            for (var i = 0; i < m1.Rows; i++)
            {
                for (var j = 0; j < m1.Cols; j++)
                {
                    result[i, j] = m1[i, j] - m2[i, j];
                }
            }

            return result;
        }

        /// <summary>Subtract double from every element in the Matrix.</summary>
        /// <param name="m">Matrix.</param>
        /// <param name="s">Double.</param>
        /// <returns>The result of the operation.</returns>
        public static Matrix operator -(Matrix m, double s)
        {
            for (var i = 0; i < m.Rows; i++)
            {
                for (var j = 0; j < m.Cols; j++)
                {
                    m[i, j] -= s;
                }
            }

            return m;
        }

        /// <summary>Subtraction operator.</summary>
        /// <param name="s">The double to process.</param>
        /// <param name="m">The Matrix to process.</param>
        /// <returns>The result of the operation.</returns>
        public static Matrix operator -(double s, Matrix m)
        {
            // subtracting matrix so every item
            // in matrix is negated and s is added
            return (-1 * m) + s;
        }

        /// <summary>
        ///     Creates an inverse of the current matrix
        /// </summary>
        /// <returns></returns>
        public Matrix Inverse()
        {
            return Inverse(this);
        }

        #endregion

        #region Methods

        /// <summary>Inverses the given matrix.</summary>
        /// <exception cref="SingularMatrixException">Thrown when a Singular Matrix error condition occurs.</exception>
        /// <param name="mat">Matrix.</param>
        /// <returns>A Matrix.</returns>
        private static Matrix Inverse(Matrix mat)
        {
            // working space
            var matrix = new Matrix(mat.Rows, 2 * mat.Cols);

            // copy over colums
            for (var i = 0; i < mat.Cols; i++)
            {
                matrix[i, VectorType.Col] = mat[i, VectorType.Col];
            }

            // fill in identity
            for (var i = mat.Cols; i < 2 * mat.Cols; i++)
            {
                matrix[i - mat.Cols, i] = 1;
            }

            int maxrow;
            double c;
            for (var y = 0; y < matrix.Rows; y++)
            {
                maxrow = y;
                for (var y2 = y + 1; y2 < matrix.Rows; y2++)
                {
                    if (Math.Abs(matrix[y2, y]) > Math.Abs(matrix[maxrow, y]))
                    {
                        maxrow = y2;
                    }
                }

                // swap rows
                matrix.SwapRow(maxrow, y);

                // uh oh
                if (Math.Abs(matrix[y][y]) <= 0.00000000001)
                {
                    throw new SingularMatrixException("Matrix is becoming unstable!");
                }

                for (var y2 = y + 1; y2 < matrix.Rows; y2++)
                {
                    c = matrix[y2, y] / matrix[y, y];
                    for (var x = y; x < matrix.Cols; x++)
                    {
                        matrix[y2, x] -= matrix[y, x] * c;
                    }
                }
            }

            // back substitute
            for (var y = matrix.Rows - 1; y >= 0; y--)
            {
                c = matrix[y][y];
                for (var y2 = 0; y2 < y; y2++)
                {
                    for (var x = matrix.Cols - 1; x > y - 1; x--)
                    {
                        matrix[y2, x] -= matrix[y, x] * matrix[y2, y] / c;
                    }
                }

                matrix[y, y] /= c;
                for (var x = matrix.Rows; x < matrix.Cols; x++)
                {
                    matrix[y, x] /= c;
                }
            }

            // generate result
            var result = new Matrix(mat.Rows);
            for (var i = mat.Cols; i < 2 * mat.Cols; i++)
            {
                result[i - mat.Cols, VectorType.Col] = matrix[i, VectorType.Col];
            }

            return result;
        }

        #endregion
    }
}