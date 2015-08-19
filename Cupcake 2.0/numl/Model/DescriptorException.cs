// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="DescriptorException.cs">
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
//   Descriptor Exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Model
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>Descriptor Exception.</summary>
    public class DescriptorException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DescriptorException" /> class. Default constructor.
        /// </summary>
        public DescriptorException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DescriptorException" /> class. Specialised constructor for use only by
        ///     derived classes.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        public DescriptorException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DescriptorException" /> class. Specialised constructor for use only by
        ///     derived classes.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="innerException">
        ///     The inner exception.
        /// </param>
        public DescriptorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DescriptorException" /> class. Specialised constructor for use only by
        ///     derived classes.
        /// </summary>
        /// <param name="info">
        ///     The information.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected DescriptorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}