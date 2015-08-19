// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="Cluster.cs">
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
//   A cluster.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Unsupervised
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using numl.Math.LinearAlgebra;

    /// <summary>A cluster.</summary>
    public class Cluster
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Cluster" /> class. Default constructor.
        /// </summary>
        public Cluster()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Cluster" /> class. Constructor.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <param name="left">
        ///     The left.
        /// </param>
        /// <param name="right">
        ///     The right.
        /// </param>
        public Cluster(int id, Cluster left, Cluster right)
        {
            this.Id = id;
            this.Children = new[] { left, right };
            this.Points = left.Points.Concat(right.Points);

            // maybe only need item at leaves
            this.Members = left.Members.Concat(right.Members).ToArray();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Cluster" /> class. Constructor.
        /// </summary>
        /// <param name="id">
        ///     The identifier.
        /// </param>
        /// <param name="children">
        ///     The children.
        /// </param>
        public Cluster(int id, IEnumerable<Cluster> children)
        {
            this.Id = id;
            this.Children = children.ToArray();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the center.</summary>
        /// <value>The center.</value>
        public Vector Center { get; set; }

        /// <summary>Gets or sets the children.</summary>
        /// <value>The children.</value>
        public Cluster[] Children { get; set; }

        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the members.</summary>
        /// <value>The members.</value>
        public object[] Members { get; set; }

        #endregion

        #region Properties

        /// <summary>Gets or sets the points.</summary>
        /// <value>The points.</value>
        internal IEnumerable<Vector> Points { get; set; }

        #endregion

        #region Public Indexers

        /// <summary>Indexer to get items within this collection using array index syntax.</summary>
        /// <param name="i">Zero-based index of the entry to access.</param>
        /// <returns>The indexed item.</returns>
        public Cluster this[int i]
        {
            get
            {
                if (i >= this.Children.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    return this.Children[i];
                }
            }
        }

        #endregion
    }
}