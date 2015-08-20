// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryHelpers.cs" company="ChewyMoon">
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
//   A dictionary helpers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace numl.Utils
{
    using System;
    using System.Collections.Generic;

    /// <summary>A dictionary helpers.</summary>
    public static class DictionaryHelpers
    {
        #region Public Methods and Operators

        /// <summary>A Dictionary&lt;K,V&gt; extension method that adds an or update.</summary>
        /// <tparam name="K">Generic type parameter.</tparam>
        /// <tparam name="V">Generic type parameter.</tparam>
        /// <param name="dictionary">The dictionary to act on.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void AddOrUpdate<K, V>(this Dictionary<K, List<V>> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
            {
                if (!dictionary[key].Contains(value))
                {
                    dictionary[key].Add(value);
                }
            }
            else
            {
                dictionary[key] = new List<V> { value };
            }
        }

        /// <summary>A Dictionary&lt;K,V&gt; extension method that adds an or update.</summary>
        /// <tparam name="K">Generic type parameter.</tparam>
        /// <tparam name="V">Generic type parameter.</tparam>
        /// <param name="dictionary">The dictionary to act on.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void AddOrUpdate<K, V>(this Dictionary<K, V> dictionary, K key, Func<V, V> value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value(dictionary[key]);
            }
            else
            {
                dictionary[key] = value(default(V));
            }
        }

        /// <summary>A Dictionary&lt;K,V&gt; extension method that adds an or update.</summary>
        /// <tparam name="K">Generic type parameter.</tparam>
        /// <tparam name="V">Generic type parameter.</tparam>
        /// <param name="dictionary">The dictionary to act on.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="seed">The seed.</param>
        public static void AddOrUpdate<K, V>(this Dictionary<K, V> dictionary, K key, Func<V, V> value, V seed)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value(dictionary[key]);
            }
            else
            {
                dictionary[key] = value(seed);
            }
        }

        #endregion
    }
}