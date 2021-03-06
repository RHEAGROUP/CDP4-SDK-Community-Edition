﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueArrayUtils.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2020 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft
//
//    This file is part of CDP4-SDK Community Edition
//
//    The CDP4-SDK Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-SDK Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public License
//    along with this program; if not, write to the Free Software Foundation,
//    Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Common.Helpers
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    using CDP4Common.Types;

    /// <summary>
    /// The purpose of the <see cref="ValueArrayUtils"/> is to provide static helper methods for handling
    /// business logic related to <see cref="ValueArray{T}"/>
    /// </summary>
    public static class ValueArrayUtils
    {
        /// <summary>
        /// Creates a <see cref="ValueArray{String}"/> with as many slots containing "-" as the provided <paramref name="size"/>
        /// </summary>
        /// <param name="size">
        /// An integer denoting the number of slots, this may not be lower than one.
        /// </param>
        /// <returns>
        /// An instance of <see cref="ValueArray{String}"/>
        /// </returns>
        public static ValueArray<string> CreateDefaultValueArray(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException($"The {nameof(size)} may not be smaller than 1.", nameof(size));
            }
            
            var defaultValue = new List<string>(size);

            for (int i = 0; i < size; i++)
            {
                defaultValue.Add("-");
            }

            var result = new ValueArray<string>(defaultValue);

            return result;
        }

        /// <summary>
        /// Regex used for conversion of HStore value to string
        /// </summary>
        private static readonly Regex HstoreToValueArrayRegex = new Regex(@"^\{(.*)\}$", RegexOptions.Singleline);

        /// <summary>
        /// Convert a string to a <see cref="ValueArray{T}"/>
        /// </summary>
        /// <typeparam name="T">The generic type of the <see cref="ValueArray{T}"/></typeparam>
        /// <param name="valueArrayString">The string to convert</param>
        /// <returns>The <see cref="ValueArray{T}"/></returns>
        public static ValueArray<T> FromHstoreToValueArray<T>(string valueArrayString) =>
            ToValueArray<T>(valueArrayString, HstoreToValueArrayRegex);

        /// <summary>
        /// Regex used for conversion of Json value to string
        /// </summary>
        private static readonly Regex JsonToValueArrayRegex = new Regex(@"^\[(.*)\]$", RegexOptions.Singleline);

        /// <summary>
        /// Convert a string to a <see cref="ValueArray{T}"/>
        /// </summary>
        /// <typeparam name="T">The generic type of the <see cref="ValueArray{T}"/></typeparam>
        /// <param name="valueArrayString">The string to convert</param>
        /// <returns>The <see cref="ValueArray{T}"/></returns>
        public static ValueArray<T> FromJsonToValueArray<T>(string valueArrayString) =>
            ToValueArray<T>(valueArrayString, JsonToValueArrayRegex);

        /// <summary>
        /// Convert a string to a <see cref="ValueArray{T}"/>
        /// </summary>
        /// <typeparam name="T">The generic type of the <see cref="ValueArray{T}"/></typeparam>
        /// <param name="valueArrayString">The string to convert</param>
        /// <param name="regex">The Regex use for conversion</param>
        /// <returns>The <see cref="ValueArray{T}"/></returns>
        private static ValueArray<T> ToValueArray<T>(string valueArrayString, Regex regex)
        {
            var arrayExtractResult = regex.Match(valueArrayString);
            var extractedArrayString = arrayExtractResult.Groups[1].Value;

            // match within 2 unescape double-quote the following content:
            // 1) (no special char \ or ") 0..* times
            // 2) (a pattern that starts with \ followed by any character (special included) and 0..* "non special" characters) 0..* times
            var valueExtractionRegex = new Regex(@"""([^""\\]*(\\.[^""\\]*)*)""", RegexOptions.Singleline);
            var test = valueExtractionRegex.Matches(extractedArrayString);

            var stringValues = new List<string>();

            foreach (Match match in test)
            {
                stringValues.Add(UnescapeString(match.Groups[1].Value));
            }

            var convertedStringList = stringValues.Select(m => (T)Convert.ChangeType(m, typeof(T))).ToList();

            return new ValueArray<T>(convertedStringList);
        }

        /// <summary>
        /// Convert a <see cref="ValueArray{String}"/> to the JSON format
        /// </summary>
        /// <param name="valueArray">The <see cref="ValueArray{String}"/></param>
        /// <returns>The JSON string</returns>
        public static string ToJsonString(ValueArray<string> valueArray)
        {
            var items = ValueArrayToStringList(valueArray);
            return $"[{string.Join(",", items)}]";
        }

        /// <summary>
        /// Convert a <see cref="ValueArray{String}"/> to the HStore format
        /// </summary>
        /// <param name="valueArray">The <see cref="ValueArray{String}"/></param>
        /// <returns>The HStore string</returns>
        public static string ToHstoreString(ValueArray<string> valueArray)
        {
            var items = ValueArrayToStringList(valueArray);
            return $"{{{string.Join(";", items)}}}";
        }

        /// <summary>
        /// Escape double quote and backslash
        /// </summary>
        /// <param name="valueArray"></param>
        /// <returns>IEnumerable containing escaped strings</returns>
        private static IEnumerable<string> ValueArrayToStringList(ValueArray<string> valueArray)
        {
            var items = valueArray.ToList();

            for (var i = 0; i < items.Count; i++)
            {
                items[i] = $"\"{EscapeString(items[i])}\"";
            }

            return items;
        }

        /// <summary>
        /// Contains a list of Keys and Values that can be used to replace each other when escaping and unescaping a <see cref="ValueArray{String}"/>
        /// Details: Section 9 (String) of file: http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-404.pdf
        /// </summary>
        private static readonly Dictionary<string, string> EscapePairs = new Dictionary<string, string>
        {
            {"\\\\", "\\"},
            {"\\\"", "\""},
            {"\\b", "\b"},
            {"\\f", "\f"},
            {"\\n", "\n"},
            {"\\r", "\r"},
            {"\\t", "\t"},
            {"\\/", "/"}
        };

        /// <summary>
        /// Escapes all characters that need to be treated as special characters in a <see cref="ValueArray{String}"/>
        /// </summary>
        /// <param name="unescapedString"></param>
        /// <returns>The escaped string</returns>
        public static string EscapeString(string unescapedString)
        {
            var escapedString = unescapedString;

            foreach (var pair in EscapePairs)
            {
                escapedString = escapedString.Replace(pair.Value, pair.Key);
            }

            return escapedString;
        }

        /// <summary>
        /// Unescapes all characters that need to be treated as special characters in a <see cref="ValueArray{String}"/>
        /// </summary>
        /// <param name="escapedString"></param>
        /// <returns>The unescaped string</returns>
        public static string UnescapeString(string escapedString)
        {
            var unescapeString = escapedString;

            foreach (var pair in EscapePairs.Reverse())
            {
                unescapeString = unescapeString.Replace(pair.Key, pair.Value);
            }

            return unescapeString;
        }
    }
}