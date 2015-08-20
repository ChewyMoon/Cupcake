// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingularMatrixException.cs" company="ChewyMoon">
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
//   Exception for signalling singular matrix errors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Math.LinearAlgebra
{
    using System;

    /// <summary>Exception for signalling singular matrix errors.</summary>
    public class SingularMatrixException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingularMatrixException" /> class. Default constructor.
        /// </summary>
        public SingularMatrixException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingularMatrixException" /> class. Constructor.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        public SingularMatrixException(string message)
            : base(message)
        {
        }

        #endregion
    }
}