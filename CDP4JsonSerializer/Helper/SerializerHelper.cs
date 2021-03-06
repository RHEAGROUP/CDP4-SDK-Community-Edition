﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerHelper.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2019 RHEA System S.A.
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

namespace CDP4JsonSerializer
{
    using System.Collections.Generic;

    using CDP4Common.Types;
    using CDP4Common.Helpers;
    using Newtonsoft.Json.Linq;
    
    using NLog;

    /// <summary>
    /// Utility method to convert a JSON token to a CDP4 type
    /// </summary>
    public static class SerializerHelper
    {
        /// <summary>
        /// Convert a string to a <see cref="ValueArray{T}"/>
        /// </summary>
        /// <typeparam name="T">The generic type of the <see cref="ValueArray{T}"/></typeparam>
        /// <param name="valueArrayString">The string to convert</param>
        /// <returns>The <see cref="ValueArray{T}"/></returns>
        public static ValueArray<T> ToValueArray<T>(string valueArrayString) => ValueArrayUtils.FromJsonToValueArray<T>(valueArrayString);

        /// <summary>
        /// Convert a <see cref="ValueArray{String}"/> to the JSON format
        /// </summary>
        /// <param name="valueArray">The <see cref="ValueArray{String}"/></param>
        /// <returns>The JSON string</returns>
        public static string ToJsonString(this ValueArray<string> valueArray) =>
            ValueArrayUtils.ToJsonString(valueArray);

        /// <summary>
        /// Serialize a <see cref="OrderedItem"/> to a <see cref="JObject"/>
        /// </summary>
        /// <param name="orderedItem">The <see cref="OrderedItem"/></param>
        /// <returns>The <see cref="JObject"/></returns>
        public static JObject ToJsonObject(this OrderedItem orderedItem)
        {
            var jsonObject = new JObject();
            jsonObject.Add("k", new JValue(orderedItem.K));

            if (orderedItem.M != null)
            {
                jsonObject.Add("m", new JValue(orderedItem.M));
            }

            jsonObject.Add("v", new JValue(orderedItem.V));
            return jsonObject;
        }

        /// <summary>
        /// Instantiate a <see cref="IEnumerable{OrderedItem}"/> from a <see cref="JToken"/>
        /// </summary>
        /// <param name="jsonToken">The <see cref="JToken"/></param>
        /// <returns>The <see cref="IEnumerable{OrderedItem}"/></returns>
        public static IEnumerable<OrderedItem> ToOrderedItemCollection(this JToken jsonToken)
        {
            var list = new List<OrderedItem>();
            foreach (var token in jsonToken)
            {
                var orderedItem = new OrderedItem
                {
                    K = token["k"].ToObject<long>(),
                    V = token["v"].ToString()
                };

                var move = token["m"];
                if (move != null)
                {
                    orderedItem.M = move.ToObject<long>();
                }

                list.Add(orderedItem);
            }

            return list;
        }

        /// <summary>
        /// Assert Whether a <see cref="JToken"/> is null or empty
        /// </summary>
        /// <param name="token">The <see cref="JToken"/></param>
        /// <returns>True if the <see cref="JToken"/> is null or empty</returns>
        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.Null);
        }
    }
}
