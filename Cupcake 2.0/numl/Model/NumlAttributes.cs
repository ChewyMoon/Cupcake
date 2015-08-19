// --------------------------------------------------------------------------------------------------------------------
// <copyright company="ChewyMoon" file="NumlAttributes.cs">
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
//   Attribute for numl.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Model
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    using numl.Utils;

    /// <summary>Attribute for numl.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class NumlAttribute : Attribute
    {
        #region Public Methods and Operators

        /// <summary>Generates a property.</summary>
        /// <param name="property">The property.</param>
        /// <returns>The property.</returns>
        public virtual Property GenerateProperty(PropertyInfo property)
        {
            return TypeHelpers.GenerateFeature(property.PropertyType, property.Name);
        }

        #endregion
    }

    /// <summary>Attribute for feature.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FeatureAttribute : NumlAttribute
    {
    }

    /// <summary>Attribute for label.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class LabelAttribute : NumlAttribute
    {
        #region Public Methods and Operators

        /// <summary>Generates a property.</summary>
        /// <param name="property">The property.</param>
        /// <returns>The property.</returns>
        public override Property GenerateProperty(PropertyInfo property)
        {
            return TypeHelpers.GenerateLabel(property.PropertyType, property.Name);
        }

        #endregion
    }

    /// <summary>Attribute for string feature.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StringFeatureAttribute : FeatureAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringFeatureAttribute" /> class. Default constructor.
        /// </summary>
        public StringFeatureAttribute()
        {
            this.AsEnum = false;
            this.SplitType = StringSplitType.Word;
            this.Separator = " ";
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringFeatureAttribute" /> class. Constructor.
        /// </summary>
        /// <param name="splitType">
        ///     Type of the split.
        /// </param>
        /// <param name="separator">
        ///     (Optional) the separator.
        /// </param>
        /// <param name="exclusions">
        ///     (Optional) the exclusions.
        /// </param>
        public StringFeatureAttribute(StringSplitType splitType, string separator = " ", string exclusions = null)
        {
            this.SplitType = splitType;
            this.Separator = separator;
            this.ExclusionFile = exclusions;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringFeatureAttribute" /> class. Constructor.
        /// </summary>
        /// <param name="asEnum">
        ///     true to as enum.
        /// </param>
        public StringFeatureAttribute(bool asEnum)
        {
            this.AsEnum = asEnum;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether as enum.</summary>
        /// <value>true if as enum, false if not.</value>
        public bool AsEnum { get; set; }

        /// <summary>Gets or sets the exclusion file.</summary>
        /// <value>The exclusion file.</value>
        public string ExclusionFile { get; set; }

        /// <summary>Gets or sets the separator.</summary>
        /// <value>The separator.</value>
        public string Separator { get; set; }

        /// <summary>Gets or sets the type of the split.</summary>
        /// <value>The type of the split.</value>
        public StringSplitType SplitType { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generates a property.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="property">The property.</param>
        /// <returns>The property.</returns>
        public override Property GenerateProperty(PropertyInfo property)
        {
            if (property.PropertyType != typeof(string))
            {
                throw new InvalidOperationException("Must use a string property.");
            }

            var sp = new StringProperty
                         {
                             Name = property.Name, SplitType = this.SplitType, Separator = this.Separator, 
                             AsEnum = this.AsEnum, Discrete = true
                         };

            sp.ImportExclusions(this.ExclusionFile);

            return sp;
        }

        #endregion
    }

    /// <summary>Attribute for string label.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StringLabelAttribute : LabelAttribute
    {
    }

    /// <summary>Attribute for date feature.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateFeatureAttribute : FeatureAttribute
    {
        #region Fields

        /// <summary>The dp.</summary>
        private DateTimeProperty dp;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DateFeatureAttribute" /> class. Constructor.
        /// </summary>
        /// <param name="features">
        ///     The features.
        /// </param>
        public DateFeatureAttribute(DateTimeFeature features)
        {
            this.dp = new DateTimeProperty(features);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DateFeatureAttribute" /> class. Constructor.
        /// </summary>
        /// <param name="portion">
        ///     The portion.
        /// </param>
        public DateFeatureAttribute(DatePortion portion)
        {
            this.dp = new DateTimeProperty(portion);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generates a property.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="property">The property.</param>
        /// <returns>The property.</returns>
        public override Property GenerateProperty(PropertyInfo property)
        {
            if (property.PropertyType != typeof(DateTime))
            {
                throw new InvalidOperationException("Invalid datetime property.");
            }

            this.dp.Discrete = true;
            this.dp.Name = property.Name;
            return this.dp;
        }

        #endregion
    }

    /// <summary>Attribute for enumerable feature.</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EnumerableFeatureAttribute : FeatureAttribute
    {
        #region Fields

        /// <summary>The length.</summary>
        private readonly int _length;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumerableFeatureAttribute" /> class. Constructor.
        /// </summary>
        /// <param name="length">
        ///     The length.
        /// </param>
        public EnumerableFeatureAttribute(int length)
        {
            this._length = length;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Generates a property.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="property">The property.</param>
        /// <returns>The property.</returns>
        public override Property GenerateProperty(PropertyInfo property)
        {
            if (!property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
            {
                throw new InvalidOperationException("Invalid Enumerable type.");
            }

            if (this._length <= 0)
            {
                throw new InvalidOperationException("Cannot have an enumerable feature of 0 or less.");
            }

            var type = property.PropertyType;
            var ep = new EnumerableProperty(this._length);

            // good assumption??
            ep.Discrete = type.BaseType == typeof(Enum) || type == typeof(bool) || type == typeof(char);
            ep.Name = property.Name;

            ep.Type = type.GetElementType();
            return ep;
        }

        #endregion
    }
}