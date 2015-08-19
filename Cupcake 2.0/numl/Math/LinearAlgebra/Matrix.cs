// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Matrix.cs">
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using numl.Math.Probability;

    /// <summary>A matrix.</summary>
    [XmlRoot("m")]
    [Serializable]
    public partial class Matrix : IXmlSerializable
    {
        #region Fields

        /// <summary>true to as transpose reference.</summary>
        private bool _asTransposeRef;

        /// <summary>The matrix.</summary>
        private double[][] _matrix;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix" /> class. Create matrix n x n matrix.
        /// </summary>
        /// <param name="n">
        ///     size.
        /// </param>
        public Matrix(int n)
            : this(n, n)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix" /> class. Create new n x d matrix.
        /// </summary>
        /// <param name="n">
        ///     rows.
        /// </param>
        /// <param name="d">
        ///     cols.
        /// </param>
        public Matrix(int n, int d)
        {
            this._asTransposeRef = false;
            this.Rows = n;
            this.Cols = d;
            this._matrix = new double[n][];
            for (var i = 0; i < n; i++)
            {
                this._matrix[i] = new double[d];
                for (var j = 0; j < d; j++)
                {
                    this._matrix[i][j] = 0;
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix" /> class. Create new matrix with prepopulated vals.
        /// </summary>
        /// <param name="m">
        ///     initial matrix.
        /// </param>
        public Matrix(double[,] m)
        {
            this._asTransposeRef = false;
            this.Rows = m.GetLength(0);
            this.Cols = m.GetLength(1);
            this._matrix = new double[this.Rows][];
            for (var i = 0; i < this.Rows; i++)
            {
                this._matrix[i] = new double[this.Cols];
                for (var j = 0; j < this.Cols; j++)
                {
                    this._matrix[i][j] = m[i, j];
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix" /> class. Create matrix n x n matrix.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the requested operation is invalid.
        /// </exception>
        /// <param name="m">
        ///     initial matrix.
        /// </param>
        public Matrix(double[][] m)
        {
            this._asTransposeRef = false;
            this.Rows = m.GetLength(0);
            if (this.Rows > 0)
            {
                this.Cols = m[0].Length;
            }
            else
            {
                throw new InvalidOperationException("Insufficient information to construct Matrix");
            }

            this._matrix = m;
        }

        // --------------- ctor

        /// <summary>
        ///     Prevents a default instance of the <see cref="Matrix" /> class from being created. Used only internally.
        /// </summary>
        private Matrix()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the cols.</summary>
        /// <value>The cols.</value>
        public int Cols { get; private set; }

        /// <summary>Gets or sets the rows.</summary>
        /// <value>The rows.</value>
        public int Rows { get; private set; }

        /// <summary>
        ///     Returns read-only transpose (uses matrix reference to save space)
        ///     It will throw an exception if there is an attempt to write to the matrix.
        /// </summary>
        /// <value>The t.</value>
        public Matrix T
        {
            get
            {
                return new Matrix { _asTransposeRef = true, _matrix = this._matrix, Cols = this.Rows, Rows = this.Cols };
            }
        }

        #endregion

        #region Public Indexers

        // --------------- access
        /// <summary>Accessor.</summary>
        /// <param name="i">Row.</param>
        /// <param name="j">Column.</param>
        /// <returns>The indexed item.</returns>
        public virtual double this[int i, int j]
        {
            get
            {
                if (!this._asTransposeRef)
                {
                    return this._matrix[i][j];
                }
                else
                {
                    return this._matrix[j][i];
                }
            }

            set
            {
                if (this._asTransposeRef)
                {
                    throw new InvalidOperationException("Cannot modify matrix in read-only transpose mode!");
                }

                this._matrix[i][j] = value;
            }
        }

        /// <summary>Returns row vector specified at index i.</summary>
        /// <param name="i">row index.</param>
        /// <returns>The indexed item.</returns>
        public virtual Vector this[int i]
        {
            get
            {
                return this[i, VectorType.Row];
            }

            set
            {
                this[i, VectorType.Row] = value;
            }
        }

        /// <summary>returns col/row vector at index j.</summary>
        /// <param name="i">Col/Row.</param>
        /// <param name="t">Row or Column.</param>
        /// <returns>Vector.</returns>
        public virtual Vector this[int i, VectorType t]
        {
            get
            {
                // switch it up if using a transposed version
                if (this._asTransposeRef)
                {
                    t = t == VectorType.Row ? VectorType.Col : VectorType.Row;
                }

                if (t == VectorType.Row)
                {
                    if (i >= this.Rows)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    if (!this._asTransposeRef)
                    {
                        return new Vector(this._matrix[i].ToArray());
                    }
                    else
                    {
                        return new Vector(this._matrix, i, true);
                    }
                }
                else
                {
                    if (i >= this.Cols)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    if (!this._asTransposeRef)
                    {
                        var cols = new double[this.Rows];
                        for (var j = 0; j < this.Rows; j++)
                        {
                            cols[j] = this._matrix[j][i];
                        }

                        return new Vector(cols);
                    }
                    else
                    {
                        return new Vector(this._matrix, i);
                    }
                }
            }

            set
            {
                if (this._asTransposeRef)
                {
                    throw new InvalidOperationException("Cannot modify matrix in read-only transpose mode!");
                }

                if (t == VectorType.Row)
                {
                    if (i >= this.Rows)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    if (value.Length > this.Cols)
                    {
                        throw new InvalidOperationException(
                            string.Format("Vector has lenght larger then {0}", this.Cols));
                    }

                    for (var k = 0; k < this.Cols; k++)
                    {
                        this._matrix[i][k] = value[k];
                    }
                }
                else
                {
                    if (i >= this.Cols)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    if (value.Length > this.Rows)
                    {
                        throw new InvalidOperationException(
                            string.Format("Vector has lenght larger then {0}", this.Cols));
                    }

                    for (var k = 0; k < this.Rows; k++)
                    {
                        this._matrix[k][i] = value[k];
                    }
                }
            }
        }

        /// <summary>Indexer to set items within this collection using array index syntax.</summary>
        /// <param name="f">The Func&lt;double,bool&gt; to process.</param>
        /// <returns>The indexed item.</returns>
        public double this[Func<double, bool> f]
        {
            set
            {
                for (var i = 0; i < this.Rows; i++)
                {
                    for (var j = 0; j < this.Cols; j++)
                    {
                        if (f(this._matrix[i][j]))
                        {
                            this[i, j] = value;
                        }
                    }
                }
            }
        }

        /// <summary>Indexer to get items within this collection using array index syntax.</summary>
        /// <param name="f">The Func&lt;Vector,bool&gt; to process.</param>
        /// <param name="t">The VectorType to process.</param>
        /// <returns>The indexed item.</returns>
        public Matrix this[Func<Vector, bool> f, VectorType t]
        {
            get
            {
                var count = 0;
                if (t == VectorType.Row)
                {
                    for (var i = 0; i < this.Rows; i++)
                    {
                        if (f(this[i, t]))
                        {
                            count++;
                        }
                    }

                    var m = new Matrix(count, this.Cols);
                    var j = -1;
                    for (var i = 0; i < this.Rows; i++)
                    {
                        if (f(this[i, t]))
                        {
                            m[++j, t] = this[i, t];
                        }
                    }

                    return m;
                }
                else
                {
                    for (var i = 0; i < this.Cols; i++)
                    {
                        if (f(this[i, t]))
                        {
                            count++;
                        }
                    }

                    var m = new Matrix(this.Rows, count);
                    var j = -1;
                    for (var i = 0; i < this.Cols; i++)
                    {
                        if (f(this[i, t]))
                        {
                            m[++j, t] = this[i, t];
                        }
                    }

                    return m;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Creates a new Matrix.</summary>
        /// <param name="n">Size.</param>
        /// <param name="f">The Func&lt;int,int,double&gt; to process.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Create(int n, Func<double> f)
        {
            return Create(n, n, f);
        }

        /// <summary>Creates a new Matrix.</summary>
        /// <param name="n">Size.</param>
        /// <param name="d">cols.</param>
        /// <param name="f">The Func&lt;int,int,double&gt; to process.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Create(int n, int d, Func<double> f)
        {
            var matrix = new Matrix(n, d);
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Cols; j++)
                {
                    matrix[i, j] = f();
                }
            }

            return matrix;
        }

        /// <summary>Creates a new Matrix.</summary>
        /// <param name="n">Size.</param>
        /// <param name="f">The Func&lt;int,int,double&gt; to process.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Create(int n, Func<int, int, double> f)
        {
            return Create(n, n, f);
        }

        /// <summary>Creates a new Matrix.</summary>
        /// <param name="n">Size.</param>
        /// <param name="d">cols.</param>
        /// <param name="f">The Func&lt;int,int,double&gt; to process.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Create(int n, int d, Func<int, int, double> f)
        {
            var matrix = new Matrix(n, d);
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Cols; j++)
                {
                    matrix[i, j] = f(i, j);
                }
            }

            return matrix;
        }

        /// <summary>n x d identity matrix.</summary>
        /// <param name="n">rows.</param>
        /// <param name="d">cols.</param>
        /// <returns>Matrix.</returns>
        public static Matrix Identity(int n, int d)
        {
            var m = new double[n][];
            for (var i = 0; i < n; i++)
            {
                m[i] = new double[d];
                for (var j = 0; j < d; j++)
                {
                    if (i == j)
                    {
                        m[i][j] = 1;
                    }
                    else
                    {
                        m[i][j] = 0;
                    }
                }
            }

            return new Matrix { _matrix = m, Rows = n, Cols = d, _asTransposeRef = false };
        }

        /// <summary>n x n identity matrix.</summary>
        /// <param name="n">Size.</param>
        /// <returns>Matrix.</returns>
        public static Matrix Identity(int n)
        {
            return Identity(n, n);
        }

        /// <summary>Loads the given stream.</summary>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <param name="file">The file to load.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Load(string file)
        {
            if (File.Exists(file))
            {
                using (var stream = File.OpenRead(file)) return Load(stream);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        /// <summary>Loads the given stream.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="stream">The stream to load.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Load(Stream stream)
        {
            Matrix matrix = null;
            using (var reader = new StreamReader(stream))
            {
                var i = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                    if (!line.StartsWith("--"))
                    {
                        var numbers = line.Split(',');

                        // need to create new matrix
                        if (matrix == null)
                        {
                            if (numbers.Length >= 2)
                            {
                                matrix = new Matrix(int.Parse(numbers[0].Trim()), int.Parse(numbers[1].Trim()));
                            }
                            else
                            {
                                throw new InvalidOperationException("Invalid matrix format");
                            }
                        }
                        else
                        {
                            for (var j = 0; j < numbers.Length; j++)
                            {
                                matrix[i, j] = double.Parse(numbers[j]);
                            }

                            i++;
                        }
                    }
                }
            }

            return matrix;
        }

        /// <summary>Normalise random.</summary>
        /// <param name="n">Size.</param>
        /// <param name="d">cols.</param>
        /// <param name="min">(Optional) the minimum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix NormRand(int n, int d, double min = 0)
        {
            var m = new double[n][];
            for (var i = 0; i < n; i++)
            {
                m[i] = new double[d];
                for (var j = 0; j < d; j++)
                {
                    m[i][j] = Sampling.GetNormal() + min;
                }
            }

            return new Matrix { _matrix = m, _asTransposeRef = false, Cols = d, Rows = n };
        }

        /// <summary>Normalise random.</summary>
        /// <param name="n">Size.</param>
        /// <param name="min">(Optional) the minimum.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix NormRand(int n, double min = 0)
        {
            return NormRand(n, n, min);
        }

        /// <summary>Normalise random.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="means">The means.</param>
        /// <param name="stdDev">The standard development.</param>
        /// <param name="n">Size.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix NormRand(Vector means, Vector stdDev, int n)
        {
            if (means.Length != stdDev.Length)
            {
                throw new InvalidOperationException("Invalid Dimensionality");
            }

            var d = means.Length;
            var m = new double[n][];

            for (var i = 0; i < n; i++)
            {
                m[i] = new double[d];
                for (var j = 0; j < d; j++)
                {
                    m[i][j] = Sampling.GetNormal(means[j], stdDev[j]);
                }
            }

            return new Matrix { _matrix = m, _asTransposeRef = false, Cols = d, Rows = n };
        }

        /// <summary>
        ///     Generate a matrix n x d with numbers 0 less than x less than 1 drawn uniformly at random.
        /// </summary>
        /// <param name="n">rows.</param>
        /// <param name="d">cols.</param>
        /// <param name="min">(Optional) the minimum.</param>
        /// <returns>n x d Matrix.</returns>
        public static Matrix Rand(int n, int d, double min = 0)
        {
            var m = new double[n][];
            for (var i = 0; i < n; i++)
            {
                m[i] = new double[d];
                for (var j = 0; j < d; j++)
                {
                    m[i][j] = Sampling.GetUniform() + min;
                }
            }

            return new Matrix { _matrix = m, _asTransposeRef = false, Cols = d, Rows = n };
        }

        /// <summary>
        ///     Generate a matrix n x d with numbers 0 less than x less than 1 drawn uniformly at random.
        /// </summary>
        /// <param name="n">rows.</param>
        /// <param name="min">(Optional) the minimum.</param>
        /// <returns>n x d Matrix.</returns>
        public static Matrix Rand(int n, double min = 0)
        {
            return Rand(n, n, min);
        }

        // --------------- creation
        /// <summary>Initial Zero Matrix (n by n)</summary>
        /// <param name="n">Size.</param>
        /// <returns>Matrix.</returns>
        public static Matrix Zeros(int n)
        {
            return new Matrix(n, n);
        }

        /// <summary>Initial zero matrix.</summary>
        /// <param name="n">.</param>
        /// <param name="d">.</param>
        /// <returns>A Matrix.</returns>
        public static Matrix Zeros(int n, int d)
        {
            return new Matrix(n, d);
        }

        /// <summary>In place centering. WARNING: WILL UPDATE MATRIX!</summary>
        /// <param name="t">.</param>
        /// <returns>A Matrix.</returns>
        public Matrix Center(VectorType t)
        {
            var max = t == VectorType.Row ? this.Rows : this.Cols;
            for (var i = 0; i < max; i++)
            {
                this[i, t] -= this[i, t].Mean();
            }

            return this;
        }

        /// <summary>Cols.</summary>
        /// <param name="i">Zero-based index of the.</param>
        /// <returns>A Vector.</returns>
        public Vector Col(int i)
        {
            return this[i, VectorType.Col];
        }

        /// <summary>create deep copy of matrix.</summary>
        /// <returns>Matrix.</returns>
        public Matrix Copy()
        {
            var m = Zeros(this.Rows, this.Cols);
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Cols; j++)
                {
                    m[i, j] = this[i, j];
                }
            }

            return m;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="m">initial matrix.</param>
        /// <param name="tol">Double to be compared.</param>
        /// <returns>
        ///     true if the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        public bool Equals(Matrix m, double tol)
        {
            if (this.Rows != m.Rows || this.Cols != m.Cols)
            {
                return false;
            }

            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Cols; j++)
                {
                    if (Math.Abs(this[i, j] - m[i, j]) > tol)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     true if the specified <see cref="T:System.Object" /> is equal to the current
        ///     <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Matrix)
            {
                var m = obj as Matrix;
                if (this.Rows != m.Rows || this.Cols != m.Cols)
                {
                    return false;
                }

                for (var i = 0; i < this.Rows; i++)
                {
                    for (var j = 0; j < this.Cols; j++)
                    {
                        if (this[i, j] != m[i, j])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Gets the cols in this collection.</summary>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the cols in this collection.
        /// </returns>
        public IEnumerable<Vector> GetCols()
        {
            for (var i = 0; i < this.Cols; i++)
            {
                yield return this[i, VectorType.Col];
            }
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            return this._matrix.GetHashCode();
        }

        /// <summary>Gets a matrix.</summary>
        /// <param name="d1">The first int.</param>
        /// <param name="d2">The second int.</param>
        /// <param name="n1">The first int.</param>
        /// <param name="n2">The second int.</param>
        /// <returns>The matrix.</returns>
        public Matrix GetMatrix(int d1, int d2, int n1, int n2)
        {
            var m = Zeros(n2 - n1 + 1, d2 - d1 + 1);
            for (var i = 0; i < m.Rows; i++)
            {
                for (var j = 0; j < m.Cols; j++)
                {
                    m[i, j] = this[i + n1, j + d1];
                }
            }

            return m;
        }

        /// <summary>Gets the rows in this collection.</summary>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the rows in this collection.
        /// </returns>
        public IEnumerable<Vector> GetRows()
        {
            for (var i = 0; i < this.Rows; i++)
            {
                yield return this[i, VectorType.Row];
            }
        }

        // ---------------- Xml Serialization
        /// <summary>
        ///     This method is reserved and should not be used. When implementing the IXmlSerializable
        ///     interface, you should return null (Nothing in Visual Basic) from this method, and instead, if
        ///     specifying a custom schema is required, apply the
        ///     <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the
        ///     object that is produced by the
        ///     <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" />
        ///     method and consumed by the
        ///     <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" />
        ///     method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>Gets a vector.</summary>
        /// <param name="index">Zero-based index of the.</param>
        /// <param name="from">Source for the.</param>
        /// <param name="to">to.</param>
        /// <param name="type">The type.</param>
        /// <returns>The vector.</returns>
        public Vector GetVector(int index, int from, int to, VectorType type)
        {
            var v = (double[])Array.CreateInstance(typeof(double), to - from + 1);
            for (int i = from, j = 0; i < to + 1; i++, j++)
            {
                v[j] = this[index, type][i];
            }

            return new Vector(v);
        }

        /// <summary>
        ///     Returns a new Matrix with the Vector inserted at the specified position
        /// </summary>
        /// <param name="v">Vector to insert</param>
        /// <param name="index">The zero based row / column.</param>
        /// <param name="t">Vector orientation</param>
        /// <param name="insertAfter">Insert after or before the last row / column</param>
        /// <returns></returns>
        public Matrix Insert(Vector v, int index, VectorType t, bool insertAfter = true)
        {
            if (t == VectorType.Col && v.Length != this.Rows)
            {
                throw new ArgumentException("Column vector does not match matrix height");
            }

            if (t == VectorType.Row && v.Length != this.Cols)
            {
                throw new ArgumentException("Row vector does not match matrix width");
            }

            var temp = this.ToArray().ToList();
            if (t == VectorType.Row)
            {
                if (index == temp.Count - 1 && insertAfter)
                {
                    temp.Add(v);
                }
                else
                {
                    temp.Insert(index, v);
                }
            }
            else
            {
                if (index == temp[0].Length - 1 && insertAfter)
                {
                    for (var i = 0; i < temp.Count; i++)
                    {
                        var copy = temp[i].ToList();
                        copy.Add(v[i]);
                        temp[i] = copy.ToArray();
                    }
                }
                else
                {
                    for (var i = 0; i < temp.Count; i++)
                    {
                        var copy = temp[i].ToList();
                        copy.Insert(index, v[i]);
                        temp[i] = copy.ToArray();
                    }
                }
            }

            return new Matrix(temp.ToArray());
        }

        // -------------- destructive ops
        /// <summary>In place normalization. WARNING: WILL UPDATE MATRIX!</summary>
        /// <param name="t">.</param>
        public void Normalize(VectorType t)
        {
            var max = t == VectorType.Row ? this.Rows : this.Cols;
            for (var i = 0; i < max; i++)
            {
                this[i, t] /= this[i, t].Norm();
            }
        }

        /// <summary>Generates an object from its XML representation.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="reader">
        ///     The <see cref="T:System.Xml.XmlReader" /> stream from which the object is
        ///     deserialized.
        /// </param>
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            this.Rows = int.Parse(reader.GetAttribute("rows"));
            this.Cols = int.Parse(reader.GetAttribute("cols"));

            reader.ReadStartElement();

            if (this.Rows > 0 && this.Cols > 0)
            {
                this._asTransposeRef = false;
                this._matrix = new double[this.Rows][];
                for (var i = 0; i < this.Rows; i++)
                {
                    reader.ReadStartElement("r");
                    this._matrix[i] = new double[this.Cols];
                    for (var j = 0; j < this.Cols; j++)
                    {
                        this._matrix[i][j] = double.Parse(reader.ReadElementString("e"));
                    }

                    reader.ReadEndElement();
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid matrix size in XML!");
            }

            reader.ReadEndElement();
        }

        /// <summary>Removes this object.</summary>
        /// <param name="index">Zero-based index of the.</param>
        /// <param name="t">.</param>
        /// <returns>A Matrix.</returns>
        public Matrix Remove(int index, VectorType t)
        {
            var max = t == VectorType.Row ? this.Rows : this.Cols;
            var row = t == VectorType.Row ? this.Rows - 1 : this.Rows;
            var col = t == VectorType.Col ? this.Cols - 1 : this.Cols;

            var m = new Matrix(row, col);
            var j = -1;
            for (var i = 0; i < max; i++)
            {
                if (i == index)
                {
                    continue;
                }

                m[++j, t] = this[i, t];
            }

            return m;
        }

        /// <summary>Rows.</summary>
        /// <param name="i">Zero-based index of the.</param>
        /// <returns>A Vector.</returns>
        public Vector Row(int i)
        {
            return this[i, VectorType.Row];
        }

        /// <summary>Saves the given stream.</summary>
        /// <param name="file">The file to load.</param>
        public void Save(string file)
        {
            using (var stream = File.OpenWrite(file)) this.Save(stream);
        }

        /// <summary>Saves the given stream.</summary>
        /// <param name="stream">The stream to load.</param>
        public void Save(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(Environment.NewLine);

                // header
                writer.Write(this.Rows);
                writer.Write(",");
                writer.Write(this.Cols);
                writer.Write(Environment.NewLine);

                // contents
                for (var i = 0; i < this.Rows; i++)
                {
                    for (var j = 0; j < this.Cols; j++)
                    {
                        if (j > 0)
                        {
                            writer.Write(",");
                        }

                        writer.Write(this[i, j].ToString("r"));
                    }

                    writer.Write(Environment.NewLine);
                }
            }
        }

        /// <summary>Swaps.</summary>
        /// <param name="from">Source for the.</param>
        /// <param name="to">to.</param>
        /// <param name="t">.</param>
        public void Swap(int from, int to, VectorType t)
        {
            var temp = this[from, t].Copy();
            this[from, t] = this[to, t];
            this[to, t] = temp;
        }

        /// <summary>Swap col.</summary>
        /// <param name="from">Source for the.</param>
        /// <param name="to">to.</param>
        public void SwapCol(int from, int to)
        {
            this.Swap(from, to, VectorType.Col);
        }

        // --------------- aggregation/structural
        /// <summary>Swap row.</summary>
        /// <param name="from">Source for the.</param>
        /// <param name="to">to.</param>
        public void SwapRow(int from, int to)
        {
            this.Swap(from, to, VectorType.Row);
        }

        /// <summary>
        ///     Performs a deep copy of the underlying matrix and returns a 2D array.
        /// </summary>
        /// <returns></returns>
        public double[][] ToArray()
        {
            return this._matrix.Select(s => s.ToArray()).ToArray();
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var maxlpad = int.MinValue;
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Cols; j++)
                {
                    var lpart = this[i, j].ToString("F6");
                    if (lpart.Length > maxlpad)
                    {
                        maxlpad = lpart.Length;
                    }
                }
            }

            var matrix = new StringBuilder();
            matrix.Append("\n[");
            for (var i = 0; i < this.Rows; i++)
            {
                if (i == 0)
                {
                    matrix.Append("[ ");
                }
                else
                {
                    matrix.Append(" [ ");
                }

                for (var j = 0; j < this.Cols; j++)
                {
                    matrix.Append(" ");
                    matrix.Append(this[i, j].ToString("F6", CultureInfo.InvariantCulture).PadLeft(maxlpad));
                    if (j < this.Cols - 1)
                    {
                        matrix.Append(",");
                    }
                }

                if (i < this.Rows - 1)
                {
                    matrix.Append("],\n");
                }
                else
                {
                    matrix.Append("]]");
                }
            }

            return matrix.ToString();
        }

        /// <summary>Converts this object to a vector.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <returns>This object as a Vector.</returns>
        public Vector ToVector()
        {
            if (this.Rows == 1)
            {
                return this[0, VectorType.Row].Copy();
            }

            if (this.Cols == 1)
            {
                return this[0, VectorType.Col].Copy();
            }

            throw new InvalidOperationException("Matrix conversion failed: More then one row or one column!");
        }

        /// <summary>Deep copy transpose.</summary>
        /// <returns>Matrix.</returns>
        public Matrix Transpose()
        {
            var m = new Matrix(this.Cols, this.Rows);
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Cols; j++)
                {
                    m[j, i] = this[i, j];
                }
            }

            return m;
        }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">
        ///     The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is
        ///     serialized.
        /// </param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("cols", this.Cols.ToString());
            writer.WriteAttributeString("rows", this.Rows.ToString());

            for (var i = 0; i < this.Rows; i++)
            {
                writer.WriteStartElement("r");
                for (var j = 0; j < this.Cols; j++)
                {
                    writer.WriteStartElement("e");
                    writer.WriteValue(this._matrix[i][j]);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}