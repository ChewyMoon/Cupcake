// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DescriptorProperty.cs" company="ChewyMoon">
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
//   Fluent API addition for simplifying the process of adding features and labels to a descriptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Model
{
    using System;
    using System.Collections.Generic;

    using numl.Utils;

    /// <summary>
    ///     Fluent API addition for simplifying the process of adding features and labels to a descriptor.
    /// </summary>
    public class DescriptorProperty
    {
        #region Fields

        /// <summary>The descriptor.</summary>
        private readonly Descriptor _descriptor;

        /// <summary>true to label.</summary>
        private readonly bool _label;

        /// <summary>The name.</summary>
        private readonly string _name;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DescriptorProperty" /> class. internal constructor used for creating
        ///     chaining.
        /// </summary>
        /// <param name="descriptor">
        ///     descriptor.
        /// </param>
        /// <param name="name">
        ///     name of property.
        /// </param>
        /// <param name="label">
        ///     label property?
        /// </param>
        internal DescriptorProperty(Descriptor descriptor, string name, bool label)
        {
            this._label = label;
            this._name = name;
            this._descriptor = descriptor;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Adds property to descriptor with chained name and type.</summary>
        /// <param name="type">Property Type.</param>
        /// <returns>descriptor with added property.</returns>
        public Descriptor As(Type type)
        {
            Property p;
            if (this._label)
            {
                p = TypeHelpers.GenerateLabel(type, this._name);
            }
            else
            {
                p = TypeHelpers.GenerateFeature(type, this._name);
            }

            this.AddProperty(p);
            return this._descriptor;
        }

        /// <summary>Adds DateTime property to descriptor with previously chained name.</summary>
        /// <exception cref="DescriptorException">Thrown when a Descriptor error condition occurs.</exception>
        /// <param name="features">
        ///     Which date features to use (can pipe: DateTimeFeature.Year |
        ///     DateTimeFeature.DayOfWeek)
        /// </param>
        /// <returns>descriptor with added property.</returns>
        public Descriptor AsDateTime(DateTimeFeature features)
        {
            if (this._label)
            {
                throw new DescriptorException("Cannot use a DateTime property as a label");
            }

            var p = new DateTimeProperty(features) { Discrete = true, Name = this._name };

            this.AddProperty(p);
            return this._descriptor;
        }

        /// <summary>Adds DateTime property to descriptor with previously chained name.</summary>
        /// <exception cref="DescriptorException">Thrown when a Descriptor error condition occurs.</exception>
        /// <param name="portion">
        ///     Which date portions to use (can pipe: DateTimeFeature.Year |
        ///     DateTimeFeature.DayOfWeek)
        /// </param>
        /// <returns>descriptor with added property.</returns>
        public Descriptor AsDateTime(DatePortion portion)
        {
            if (this._label)
            {
                throw new DescriptorException("Cannot use an DateTime property as a label");
            }

            var p = new DateTimeProperty(portion) { Discrete = true, Name = this._name };

            this.AddProperty(p);
            return this._descriptor;
        }

        /// <summary>Adds Enumerable property to descriptor with previousy chained name.</summary>
        /// <exception cref="DescriptorException">Thrown when a Descriptor error condition occurs.</exception>
        /// <param name="length">length of enumerable to expand.</param>
        /// <returns>descriptor with added property.</returns>
        public Descriptor AsEnumerable(int length)
        {
            if (this._label)
            {
                throw new DescriptorException("Cannot use an Enumerable property as a label");
            }

            var p = new EnumerableProperty(length) { Name = this._name, Discrete = false };

            this.AddProperty(p);

            return this._descriptor;
        }

        /// <summary>
        ///     Adds the default string property to descriptor with previously chained name.
        /// </summary>
        /// <returns>descriptor with added property.</returns>
        public Descriptor AsString()
        {
            var p = new StringProperty();
            p.Name = this._name;
            p.AsEnum = this._label;
            this.AddProperty(p);
            return this._descriptor;
        }

        /// <summary>Adds string property to descriptor with previously chained name.</summary>
        /// <param name="splitType">How to split string.</param>
        /// <param name="separator">(Optional) Separator to use.</param>
        /// <param name="exclusions">(Optional) file describing strings to exclude.</param>
        /// <returns>descriptor with added property.</returns>
        public Descriptor AsString(StringSplitType splitType, string separator = " ", string exclusions = null)
        {
            var p = new StringProperty();
            p.Name = this._name;
            p.SplitType = splitType;
            p.Separator = separator;
            p.ImportExclusions(exclusions);
            p.AsEnum = this._label;
            this.AddProperty(p);
            return this._descriptor;
        }

        /// <summary>Adds string property to descriptor with previously chained name.</summary>
        /// <returns>descriptor with added property.</returns>
        public Descriptor AsStringEnum()
        {
            var p = new StringProperty();
            p.Name = this._name;
            p.AsEnum = true;
            this.AddProperty(p);
            return this._descriptor;
        }

        /// <summary>Not ready.</summary>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        /// <param name="conversion">Conversion method.</param>
        /// <returns>Descriptor.</returns>
        public Descriptor Use(Func<object, double> conversion)
        {
            throw new NotImplementedException("Not yet ;)");

            // return _descriptor;
        }

        #endregion

        #region Methods

        /// <summary>Adds a property.</summary>
        /// <param name="p">The Property to process.</param>
        private void AddProperty(Property p)
        {
            if (this._label)
            {
                this._descriptor.Label = p;
            }
            else
            {
                var features = new List<Property>(this._descriptor.Features ?? new Property[] { });
                features.Add(p);
                this._descriptor.Features = features.ToArray();
            }
        }

        #endregion
    }
}