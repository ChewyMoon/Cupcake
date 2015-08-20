// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeHelpers.cs" company="ChewyMoon">
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
//   A type helpers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Utils
{
    using System;
    using System.Collections;
    using System.Linq;

    using numl.Model;

    /// <summary>A type helpers.</summary>
    internal class TypeHelpers
    {
        #region Public Methods and Operators

        /// <summary>Generates a feature.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <returns>The feature.</returns>
        public static Property GenerateFeature(Type type, string name)
        {
            Property p;
            if (type == typeof(string))
            {
                p = new StringProperty();
            }
            else if (type == typeof(DateTime))
            {
                p = new DateTimeProperty();
            }
            else if (type.GetInterfaces().Contains(typeof(IEnumerable)))
            {
                throw new InvalidOperationException(
                    string.Format("Property {0} needs to be labeled as an EnumerableFeature", name));
            }
            else
            {
                p = new Property();
            }

            p.Discrete = type.BaseType == typeof(Enum) || type == typeof(bool) || type == typeof(string)
                         || type == typeof(char) || type == typeof(DateTime);

            p.Type = type;
            p.Name = name;

            return p;
        }

        /// <summary>Generates a label.</summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <returns>The label.</returns>
        public static Property GenerateLabel(Type type, string name)
        {
            var p = GenerateFeature(type, name);
            if (p is StringProperty)
            {
                // if it is a label, must be enum
                ((StringProperty)p).AsEnum = true;
            }

            return p;
        }

        #endregion
    }
}