﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ject.cs" company="ChewyMoon">
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
//   This class is used for fast reflection over types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    /// <summary>This class is used for fast reflection over types.</summary>
    public static class Ject
    {
        #region Static Fields

        /// <summary>The descendants.</summary>
        private static readonly Dictionary<Type, Type[]> _descendants = new Dictionary<Type, Type[]>();

        /// <summary>The types.</summary>
        private static readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();

        /// <summary>The accessors.</summary>
        private static readonly ConcurrentDictionary<Type, Dictionary<string, Func<object, object>>> accessors =
            new ConcurrentDictionary<Type, Dictionary<string, Func<object, object>>>();

        /// <summary>Constructors</summary>
        private static readonly ConcurrentDictionary<Type, Func<object>> ctors =
            new ConcurrentDictionary<Type, Func<object>>();

        /// <summary>The setters.</summary>
        private static readonly ConcurrentDictionary<Type, Dictionary<string, Action<object, object>>> setters =
            new ConcurrentDictionary<Type, Dictionary<string, Action<object, object>>>();

        #endregion

        #region Public Methods and Operators

        /// <summary>Determine if we can use simple type.</summary>
        /// <param name="t">The Type to process.</param>
        /// <returns>true if we can use simple type, false if not.</returns>
        public static bool CanUseSimpleType(Type t)
        {
            return t == typeof(string) || t == typeof(bool) || t == typeof(char) || t.BaseType == typeof(Enum)
                   || t == typeof(TimeSpan) || TypeDescriptor.GetConverter(t).CanConvertTo(typeof(double));
        }

        /// <summary>
        ///     Conversion of standard univariate types. Will throw exception on all multivariate types.
        /// </summary>
        /// <exception cref="InvalidCastException">
        ///     Thrown when an object cannot be cast to a required
        ///     type.
        /// </exception>
        /// <param name="o">value in question.</param>
        /// <returns>double representation.</returns>
        public static double Convert(object o)
        {
            // null check for object
            // return NaN if its null
            // since mathematicall null
            // should be NaN (not 0, -1,
            // etc)
            if (o == null)
            {
                return double.NaN;
            }

            var t = o.GetType();

            if (t == typeof(bool))
            {
                return (bool)o ? 1d : -1d;
            }
            else if (t == typeof(char))
            {
                // ascii number of character
                return (double)Encoding.ASCII.GetBytes(new[] { (char)o })[0];
            }
            else if (t.BaseType == typeof(Enum))
            {
                return (int)o;
            }
            else if (t == typeof(TimeSpan))
            {
                // get total seconds
                return ((TimeSpan)o).TotalSeconds;
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(t);
                if (converter.CanConvertTo(typeof(double)))
                {
                    return (double)converter.ConvertTo(o, typeof(double));
                }
                else
                {
                    throw new InvalidCastException(string.Format("Cannot convert {0} to double", o));
                }
            }
        }

        /// <summary>
        ///     Conversion of standard univariate types. Will throw exception on all multivariate types.
        /// </summary>
        /// <exception cref="InvalidCastException">
        ///     Thrown when an object cannot be cast to a required
        ///     type.
        /// </exception>
        /// <param name="val">The value.</param>
        /// <param name="t">The Type to process.</param>
        /// <returns>double representation.</returns>
        public static object Convert(double val, Type t)
        {
            if (t == typeof(char))
            {
                return (char)((int)val);
            }
            else if (t == typeof(bool))
            {
                return val >= 0;
            }
            else if (t.BaseType == typeof(Enum))
            {
                return Enum.ToObject(t, System.Convert.ChangeType(val, Enum.GetUnderlyingType(t)));
            }
            else if (t == typeof(TimeSpan))
            {
                // get total seconds
                return new TimeSpan(0, 0, (int)val);
            }
            else if (t == typeof(decimal))
            {
                return (decimal)val;
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(typeof(double));
                if (converter.CanConvertTo(t))
                {
                    return converter.ConvertTo(val, t);
                }
                else
                {
                    throw new InvalidCastException(string.Format("Cannot convert {0} to {1}", val, t.Name));
                }
            }
        }

        /// <summary>
        ///     Creates a type with an empty ctor. Faster
        ///     than Activator.CreateInstance
        /// </summary>
        /// <param name="type">Type to create (must have empty ctor)</param>
        /// <returns>Created type</returns>
        public static object Create(Type type)
        {
            var ctor = GetCtor(type);
            return ctor.Invoke();
        }

        /// <summary>Searches for the first type.</summary>
        /// <exception cref="TypeLoadException">Thrown when a Type Load error condition occurs.</exception>
        /// <param name="s">The string.</param>
        /// <returns>The found type.</returns>
        public static Type FindType(string s)
        {
            if (_types.ContainsKey(s))
            {
                return _types[s];
            }

            var type = Type.GetType(s);

            if (type == null)
            {
                // need to look elsewhere
                // someones notational laziness causes me to look
                // everywhere... sorry... I know it's slow...
                // that's why I am caching things...
                var q = (from p in AppDomain.CurrentDomain.GetAssemblies()
                         from t in p.GetTypesSafe()
                         where t.FullName == s || t.Name == s
                         select t).ToArray();

                if (q.Length == 1)
                {
                    type = q[0];
                }
            }

            if (type != null)
            {
                // cache
                _types[s] = type;
                return type;
            }
            else
            {
                throw new TypeLoadException(string.Format("Cannot find type {0}", s));
            }
        }

        /// <summary>Get a property value dynamically from an object.</summary>
        /// <param name="o">object.</param>
        /// <param name="name">parameter to extract.</param>
        /// <returns>parameter value.</returns>
        public static object Get(object o, string name)
        {
            var type = o.GetType();
            if (typeof(IDictionary<string, object>).IsAssignableFrom(type))
            {
                type = typeof(IDictionary<string, object>);
            }

            var accessor = GetAccessor(type, name);
            return accessor.Invoke(o);
        }

        /// <summary>Get a property value dynamically from a set of objects.</summary>
        /// <tparam name="T">Generic type parameter.</tparam>
        /// <param name="items">set of objects.</param>
        /// <param name="name">paramater to extract.</param>
        /// <returns>lazy list of parameter values.</returns>
        /// ###
        /// <typeparam name="T">Type of value to return.</typeparam>
        public static IEnumerable<T> Get<T>(IEnumerable items, string name)
        {
            Type type = null;
            Func<object, object> accessor = null;
            foreach (var o in items)
            {
                if (type == null)
                {
                    type = o.GetType();
                    accessor = GetAccessor(type, name);
                }

                yield return (T)accessor.Invoke(o);
            }
        }

        /// <summary>Get a property value dynamically from an object.</summary>
        /// <param name="items">set of objects.</param>
        /// <param name="name">parameter to extract.</param>
        /// <param name="cast">The cast.</param>
        /// <returns>parameter value.</returns>
        public static IEnumerable Get(IEnumerable items, string name, Type cast)
        {
            Type type = null;
            Func<object, object> accessor = null;
            var converter = new TypeConverter();
            foreach (var o in items)
            {
                if (type == null)
                {
                    type = o.GetType();
                    accessor = GetAccessor(type, name);
                }

                yield return converter.ConvertTo(accessor.Invoke(o), cast);
            }
        }

        /// <summary>Sets.</summary>
        /// <param name="o">object.</param>
        /// <param name="name">parameter to extract.</param>
        /// <param name="value">The value.</param>
        public static void Set(object o, string name, object value)
        {
            var type = o.GetType();
            var setter = GetSetter(type, name);
            setter.Invoke(o, value);
        }

        #endregion

        #region Methods

        /// <summary>Creates an accessor.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="type">The type.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>The new accessor.</returns>
        internal static Func<object, object> CreateAccessor(Type type, string valueName)
        {
            var prop = type.GetProperty(valueName);
            if (prop != null)
            {
                // std property
                var param = Expression.Parameter(typeof(object), "o");
                var exp =
                    Expression.Lambda<Func<object, object>>(
                        Expression.Convert(Expression.Property(Expression.Convert(param, type), prop), typeof(object)), 
                        param);

                return exp.Compile();
            }
            else
            {
                // dictionary type access
                var method = type.GetMethod("get_Item", new[] { typeof(string) });
                if (method == null)
                {
                    throw new InvalidOperationException(
                        string.Format("Could not find a property named \"{0}\" in containing object.", valueName));
                }

                var param = Expression.Parameter(typeof(object), "o");

                // form method call with closure over name argument
                var exp =
                    Expression.Lambda<Func<object, object>>(
                        Expression.Call(
                            Expression.Convert(param, type), 
                            method, 
                            Expression.Constant(valueName, typeof(string))), 
                        param);

                return exp.Compile();
            }
        }

        /// <summary>Creates a setter.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="type">The type.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>The new setter.</returns>
        internal static Action<object, object> CreateSetter(Type type, string valueName)
        {
            var prop = type.GetProperty(valueName);
            if (prop != null)
            {
                // std property
                var param = Expression.Parameter(typeof(object), "o");
                var pass = Expression.Parameter(typeof(object), "value");

                var exp =
                    Expression.Lambda<Action<object, object>>(
                        Expression.Assign(
                            Expression.Property(Expression.Convert(param, type), prop), 
                            Expression.Convert(pass, prop.PropertyType)), 
                        param, 
                        pass);

                return exp.Compile();
            }
            else
            {
                // dictionary type access
                var method = type.GetMethod("set_Item", new[] { typeof(string), typeof(object) });
                if (method == null)
                {
                    throw new InvalidOperationException(
                        string.Format("Could not find a property named \"{0}\" in containing object.", valueName));
                }

                var param = Expression.Parameter(typeof(object), "o");
                var pass = Expression.Parameter(typeof(object), "value");

                // form method call with closure over name argument
                var exp =
                    Expression.Lambda<Action<object, object>>(
                        Expression.Call(
                            Expression.Convert(param, type), 
                            method, 
                            Expression.Constant(valueName, typeof(string)), 
                            pass), 
                        param, 
                        pass);

                return exp.Compile();
            }
        }

        /// <summary>Searches for all assignable from.</summary>
        /// <param name="type">The type.</param>
        /// <returns>The found all assignable from.</returns>
        internal static Type[] FindAllAssignableFrom(Type type)
        {
            if (!_descendants.ContainsKey(type))
            {
                // find all descendants of given type
                _descendants[type] =
                    (from p in AppDomain.CurrentDomain.GetAssemblies()
                     from t in p.GetTypesSafe()
                     where type.IsAssignableFrom(t)
                     select t).ToArray();
            }

            return _descendants[type];
        }

        /// <summary>Gets an accessor.</summary>
        /// <param name="type">The type.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>The accessor.</returns>
        internal static Func<object, object> GetAccessor(Type type, string valueName)
        {
            Func<object, object> result;
            Dictionary<string, Func<object, object>> typeAccessors;

            // Let's see if we have an accessor already for this type/valueName
            if (accessors.TryGetValue(type, out typeAccessors))
            {
                if (typeAccessors.TryGetValue(valueName, out result))
                {
                    return result;
                }
            }

            // okay, create one and store it for later
            result = CreateAccessor(type, valueName);
            if (typeAccessors == null)
            {
                typeAccessors = new Dictionary<string, Func<object, object>>();
                accessors[type] = typeAccessors;
            }

            typeAccessors[valueName] = result;
            return result;
        }

        /// <summary>Gets a setter.</summary>
        /// <param name="type">The type.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>The setter.</returns>
        internal static Action<object, object> GetSetter(Type type, string valueName)
        {
            Action<object, object> result;
            Dictionary<string, Action<object, object>> typeSetters;

            // Let's see if we have an accessor already for this type/valueName
            if (setters.TryGetValue(type, out typeSetters))
            {
                if (typeSetters.TryGetValue(valueName, out result))
                {
                    return result;
                }
            }

            // okay, create one and store it for later
            result = CreateSetter(type, valueName);
            if (typeSetters == null)
            {
                typeSetters = new Dictionary<string, Action<object, object>>();
                setters[type] = typeSetters;
            }

            typeSetters[valueName] = result;
            return result;
        }

        /// <summary>
        ///     Gets (or creates) fast path to an empty
        ///     ctor of a provided type
        /// </summary>
        /// <param name="type">provided type</param>
        /// <returns>constructor</returns>
        private static Func<object> GetCtor(Type type)
        {
            if (!ctors.ContainsKey(type))
            {
                var ctor = type.GetConstructor(new Type[] { });
                var exp = Expression.Lambda<Func<object>>(Expression.New(ctor));
                ctors[type] = exp.Compile();
            }

            return ctors[type];
        }

        /// <summary>Gets the types safes in this collection.</summary>
        /// <param name="a">a to act on.</param>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the types safes in this collection.
        /// </returns>
        private static IEnumerable<Type> GetTypesSafe(this Assembly a)
        {
            try
            {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
        }

        #endregion
    }
}