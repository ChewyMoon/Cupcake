// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="VectorOps.cs">
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

    /// <summary>A vector.</summary>
    public partial class Vector
    {
        #region Public Methods and Operators

        /// <summary>Addition operator.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator +(Vector one, Vector two)
        {
            if (one.Length != two.Length)
            {
                throw new InvalidOperationException("Dimensions do not match!");
            }

            var result = one.Copy();
            for (var i = 0; i < result.Length; i++)
            {
                result[i] += two[i];
            }

            return result;
        }

        /// <summary>Addition operator.</summary>
        /// <param name="v">The Vector to process.</param>
        /// <param name="s">The double to process.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator +(Vector v, double s)
        {
            for (var i = 0; i < v.Length; i++)
            {
                v[i] += s;
            }

            return v;
        }

        /// <summary>Addition operator.</summary>
        /// <param name="s">The double to process.</param>
        /// <param name="v">The Vector to process.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator +(double s, Vector v)
        {
            return v + s;
        }

        /// <summary>Division operator.</summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator /(Vector one, double two)
        {
            var result = one.Copy();
            for (var i = 0; i < one.Length; i++)
            {
                result[i] /= two;
            }

            return result;
        }

        /// <summary>Division operator.</summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator /(Vector one, int two)
        {
            var result = one.Copy();
            for (var i = 0; i < one.Length; i++)
            {
                result[i] /= two;
            }

            return result;
        }

        /// <summary>Equality operator.</summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>The result of the operation.</returns>
        public static bool operator ==(Vector one, Vector two)
        {
            return ReferenceEquals(one, null) && ReferenceEquals(two, null) || one.Equals(two);
        }

        /// <summary>Bitwise 'exclusive or' operator.</summary>
        /// <param name="one">The one.</param>
        /// <param name="power">The power.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator ^(Vector one, double power)
        {
            return one.Each(d => Math.Pow(d, power));
        }

        /// <summary>double[] casting operator.</summary>
        /// <param name="v">The Vector to process.</param>
        public static implicit operator double[](Vector v)
        {
            return v.ToArray();
        }

        /// <summary>Vector casting operator.</summary>
        /// <param name="array">The array.</param>
        public static implicit operator Vector(double[] array)
        {
            return new Vector(array);
        }

        /// <summary>Vector casting operator.</summary>
        /// <param name="array">The array.</param>
        public static implicit operator Vector(int[] array)
        {
            var vector = new Vector { _asMatrixRef = false, _vector = new double[array.Length] };

            for (var i = 0; i < array.Length; i++)
            {
                vector._vector[i] = array[i];
            }

            return vector;
        }

        /// <summary>Vector casting operator.</summary>
        /// <param name="array">The array.</param>
        public static implicit operator Vector(float[] array)
        {
            var vector = new Vector { _asMatrixRef = false, _vector = new double[array.Length] };

            for (var i = 0; i < array.Length; i++)
            {
                vector._vector[i] = array[i];
            }

            return vector;
        }

        /// <summary>Inequality operator.</summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>The result of the operation.</returns>
        public static bool operator !=(Vector one, Vector two)
        {
            return !one.Equals(two);
        }

        /// <summary>Multiplication operator.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator *(Vector one, Vector two)
        {
            if (one.Length != two.Length)
            {
                throw new InvalidOperationException("Dimensions do not match!");
            }

            var result = one.Copy();
            for (var i = 0; i < one.Length; i++)
            {
                result[i] *= two[i];
            }

            return result;
        }

        /// <summary>Multiplication operator.</summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator *(Vector one, double two)
        {
            var result = one.Copy();
            for (var i = 0; i < one.Length; i++)
            {
                result[i] *= two;
            }

            return result;
        }

        /// <summary>Multiplication operator.</summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator *(Vector one, int two)
        {
            var result = one.Copy();
            for (var i = 0; i < one.Length; i++)
            {
                result[i] *= two;
            }

            return result;
        }

        /// <summary>Multiplication operator.</summary>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator *(double one, Vector two)
        {
            return two * one;
        }

        /// <summary>Subtraction operator.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="one">The one.</param>
        /// <param name="two">The two.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator -(Vector one, Vector two)
        {
            if (one.Length != two.Length)
            {
                throw new InvalidOperationException("Dimensions do not match!");
            }

            var result = one.Copy();
            for (var i = 0; i < result.Length; i++)
            {
                result[i] -= two[i];
            }

            return result;
        }

        /// <summary>Subtraction operator.</summary>
        /// <param name="v">The Vector to process.</param>
        /// <param name="s">The double to process.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator -(Vector v, double s)
        {
            var result = v.Copy();
            for (var i = 0; i < result.Length; i++)
            {
                result[i] -= s;
            }

            return result;
        }

        /// <summary>Subtraction operator.</summary>
        /// <param name="s">The double to process.</param>
        /// <param name="v">The Vector to process.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator -(double s, Vector v)
        {
            return v - s;
        }

        /// <summary>Negation operator.</summary>
        /// <param name="one">The one.</param>
        /// <returns>The result of the operation.</returns>
        public static Vector operator -(Vector one)
        {
            var result = one.Copy();
            for (var i = 0; i < result.Length; i++)
            {
                result[i] *= -1;
            }

            return result;
        }

        #endregion
    }
}