// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VectorStatics.cs" company="ChewyMoon">
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
//   A vector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.LinearAlgebra
{
    using System;
    using System.Linq;

    using numl.Math.Probability;

    /// <summary>A vector.</summary>
    public partial class Vector
    {
        #region Public Methods and Operators

        /// <summary>Calcs.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <param name="f">The Func&lt;int,double,double&gt; to process.</param>
        /// <returns>A Vector.</returns>
        public static Vector Calc(Vector v, Func<double, double> f)
        {
            var result = v.Copy();
            for (var i = 0; i < v.Length; i++)
            {
                result[i] = f(result[i]);
            }

            return result;
        }

        /// <summary>Calcs.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <param name="f">The Func&lt;int,double,double&gt; to process.</param>
        /// <returns>A Vector.</returns>
        public static Vector Calc(Vector v, Func<int, double, double> f)
        {
            var result = v.Copy();
            for (var i = 0; i < v.Length; i++)
            {
                result[i] = f(i, result[i]);
            }

            return result;
        }

        /// <summary>Combines the given v.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A Vector.</returns>
        public static Vector Combine(params Vector[] v)
        {
            if (v.Length == 0)
            {
                throw new InvalidOperationException("Need to specify vectors to combine!");
            }

            if (v.Length == 1)
            {
                return v[0];
            }

            var size = 0;
            for (var i = 0; i < v.Length; i++)
            {
                size += v[i].Length;
            }

            var r = new Vector(size);
            var z = -1;
            for (var i = 0; i < v.Length; i++)
            {
                for (var j = 0; j < v[i].Length; j++)
                {
                    r[++z] = v[i][j];
                }
            }

            return r;
        }

        /// <summary>Query if 'vector' contains na n.</summary>
        /// <param name="vector">The vector.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool ContainsNaN(Vector vector)
        {
            for (var i = 0; i < vector.Length; i++)
            {
                if (double.IsNaN(vector[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Diags.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Diag(Vector v)
        {
            var m = Matrix.Zeros(v.Length);
            for (var i = 0; i < v.Length; i++)
            {
                m[i, i] = v[i];
            }

            return m;
        }

        /// <summary>Diags.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <param name="n">The int to process.</param>
        /// <param name="d">The int to process.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Diag(Vector v, int n, int d)
        {
            var m = Matrix.Zeros(n, d);
            var min = Math.Min(n, d);
            for (var i = 0; i < min; i++)
            {
                m[i, i] = v[i];
            }

            return m;
        }

        /// <summary>Dots.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>A double.</returns>
        public static double Dot(Vector one, Vector two)
        {
            if (one.Length != two.Length)
            {
                throw new InvalidOperationException("Dimensions do not match!");
            }

            double total = 0;
            for (var i = 0; i < one.Length; i++)
            {
                total += one[i] * two[i];
            }

            return total;
        }

        /// <summary>Exponents the given v.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A Vector.</returns>
        public static Vector Exp(Vector v)
        {
            return Calc(v, d => Math.Exp(d));
        }

        /// <summary>Query if 'vector' is na n.</summary>
        /// <param name="vector">The vector.</param>
        /// <returns>true if na n, false if not.</returns>
        public static bool IsNaN(Vector vector)
        {
            var nan = true;
            for (var i = 0; i < vector.Length; i++)
            {
                nan = nan && double.IsNaN(vector[i]);
            }

            return nan;
        }

        /// <summary>Logs the given v.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A Vector.</returns>
        public static Vector Log(Vector v)
        {
            return Calc(v, d => Math.Log(d));
        }

        /// <summary>Normals.</summary>
        /// <param name="x">The Vector to process.</param>
        /// <returns>A double.</returns>
        public static double Norm(Vector x)
        {
            return Norm(x, 2);
        }

        /// <summary>Normals.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The Vector to process.</param>
        /// <param name="p">The double to process.</param>
        /// <returns>A double.</returns>
        public static double Norm(Vector x, double p)
        {
            if (p < 1)
            {
                throw new InvalidOperationException("p must be greater than 0");
            }

            double value = 0;
            if (p == 1)
            {
                for (var i = 0; i < x.Length; i++)
                {
                    value += Math.Abs(x[i]);
                }

                return value;
            }
            else if (p == int.MaxValue)
            {
                for (var i = 0; i < x.Length; i++)
                {
                    if (Math.Abs(x[i]) > value)
                    {
                        value = Math.Abs(x[i]);
                    }
                }

                return value;
            }
            else if (p == int.MinValue)
            {
                for (var i = 0; i < x.Length; i++)
                {
                    if (Math.Abs(x[i]) < value)
                    {
                        value = Math.Abs(x[i]);
                    }
                }

                return value;
            }
            else
            {
                for (var i = 0; i < x.Length; i++)
                {
                    value += Math.Pow(Math.Abs(x[i]), p);
                }

                return Math.Pow(value, 1 / p);
            }
        }

        /// <summary>Normalise random.</summary>
        /// <param name="n">The int to process.</param>
        /// <param name="mean">(Optional) the mean.</param>
        /// <param name="stdDev">(Optional) the standard development.</param>
        /// <param name="precision">(Optional) the precision.</param>
        /// <returns>A Vector.</returns>
        public static Vector NormRand(int n, double mean = 0, double stdDev = 1, int precision = -1)
        {
            var x = new double[n];
            for (var i = 0; i < n; i++)
            {
                if (precision > -1)
                {
                    x[i] = Math.Round(Sampling.GetNormal(mean, stdDev), precision);
                }
                else
                {
                    x[i] = Sampling.GetNormal(mean, stdDev);
                }
            }

            return new Vector(x);
        }

        /// <summary>Ones.</summary>
        /// <param name="n">The int to process.</param>
        /// <returns>A Vector.</returns>
        public static Vector Ones(int n)
        {
            var x = new double[n];
            for (var i = 0; i < n; i++)
            {
                x[i] = 1;
            }

            return new Vector(x);
        }

        /// <summary>Outers.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="x">The Vector to process.</param>
        /// <param name="y">The Vector to process.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Outer(Vector x, Vector y)
        {
            if (x.Length != y.Length)
            {
                throw new InvalidOperationException("Dimensions do not match!");
            }

            var n = x.Length;
            var m = new Matrix(n);
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    m[i, j] = x[i] * y[j];
                }
            }

            return m;
        }

        /// <summary>Products the given v.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A double.</returns>
        public static double Prod(Vector v)
        {
            var prod = v[0];
            for (var i = 1; i < v.Length; i++)
            {
                prod *= v[i];
            }

            return prod;
        }

        /// <summary>Rands.</summary>
        /// <param name="n">The int to process.</param>
        /// <returns>A Vector.</returns>
        public static Vector Rand(int n)
        {
            var x = new double[n];
            for (var i = 0; i < n; i++)
            {
                x[i] = Sampling.GetUniform();
            }

            return new Vector(x);
        }

        /// <summary>Rounds.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <param name="decimals">(Optional) the decimals.</param>
        /// <returns>A Vector.</returns>
        public static Vector Round(Vector v, int decimals = 0)
        {
            for (var i = 0; i < v.Length; i++)
            {
                v[i] = Math.Round(v[i], decimals);
            }

            return v;
        }

        /// <summary>Sort order.</summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The sorted order.</returns>
        public static Vector SortOrder(Vector vector)
        {
            return
                vector.Select((d, i) => new Tuple<int, double>(i, d))
                    .OrderByDescending(t => t.Item2)
                    .Select(t => t.Item1)
                    .ToArray();
        }

        /// <summary>Sums the given v.</summary>
        /// <param name="v">A variable-length parameters list containing v.</param>
        /// <returns>A double.</returns>
        public static double Sum(Vector v)
        {
            double sum = 0;
            for (var i = 0; i < v.Length; i++)
            {
                sum += v[i];
            }

            return sum;
        }

        /// <summary>Zeros.</summary>
        /// <param name="n">The int to process.</param>
        /// <returns>A Vector.</returns>
        public static Vector Zeros(int n)
        {
            return new Vector(n);
        }

        #endregion
    }
}