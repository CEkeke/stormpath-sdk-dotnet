﻿// <copyright file="QueryString.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Http
{
    /// <summary>
    /// Represents the query string component of a URI.
    /// </summary>
    public class QueryString : ImmutableValueObject<QueryString>
    {
        private static Func<QueryString, QueryString, bool> compareFunction = new Func<QueryString, QueryString, bool>(
            (a, b) => a.queryStringItems.SequenceEqual(b.queryStringItems));

        private readonly Dictionary<string, string> queryStringItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryString"/> class.
        /// </summary>
        public QueryString()
            : base(compareFunction)
        {
            this.queryStringItems = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryString"/> class
        /// by taking zero or more query parameters from a <see cref="Uri"/> instance.
        /// </summary>
        /// <param name="uri">An existing URI to copy.</param>
        public QueryString(Uri uri)
            : this(uri.Query)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryString"/> class
        /// by parsing zero or more query parameters from a string.
        /// </summary>
        /// <param name="queryString">The string to parse.</param>
        public QueryString(string queryString)
            : this()
        {
            this.queryStringItems = Parse(queryString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryString"/> class
        /// by copying the internal item dictionary from an existing instance.
        /// </summary>
        /// <param name="queryParams">The parameters to copy.</param>
        internal QueryString(Dictionary<string, string> queryParams)
            : this()
        {
            if (queryParams != null)
                this.queryStringItems = ToSortedDictionary(queryParams);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryString"/> class
        /// by copying an existing <see cref="QueryString"/> instance.
        /// </summary>
        /// <param name="existing">The existing instance to copy.</param>
        internal QueryString(QueryString existing)
            : this(existing.queryStringItems)
        {
        }

        /// <summary>
        /// Returns whether the <see cref="QueryString"/> instance contains any query parameters.
        /// </summary>
        /// <returns><c>true</c> if this instance contains one or more query parameters.</returns>
        public bool Any()
            => this.queryStringItems.IsNullOrEmpty();

        public bool ContainsKey(string key)
            => this.queryStringItems?.ContainsKey(key) ?? false;

        public string ToString(bool canonical)
        {
            bool isEmpty = this.queryStringItems == null || this.queryStringItems.Count == 0;
            if (isEmpty)
                return string.Empty;

            var items = this.queryStringItems.Select(x =>
            {
                var key = RequestHelper.UrlEncode(x.Key, false, canonical);
                var value = RequestHelper.UrlEncode(x.Value, false, canonical);

                if (IsDatetimeSearchCriteria(key) && !canonical)
                    value = FixDatetimeSearchEncoding(value);

                return $"{key}={value}";
            });

            return string.Join("&", items);
        }

        public override string ToString()
        {
            return this.ToString(false);
        }

        /// <summary>
        /// Merges the query parameters in this instance with
        /// the parameters in another <see cref="QueryString"/> instance.
        /// If there are merge conflicts, the values in this instance are always kept.
        /// </summary>
        /// <param name="replacementParams">Query parameters to copy and merge.</param>
        /// <returns>A new <see cref="QueryString"/> instance containing the merged parameters.</returns>
        internal QueryString Merge(QueryString replacementParams)
        {
            if (replacementParams == null)
                return this;

            if (!replacementParams.queryStringItems.Any())
                return this;

            var mergedItems = new List<KeyValuePair<string, string>>(this.queryStringItems);
            foreach (var param in replacementParams.queryStringItems)
            {
                // The values in the actual queryString are explicit and take precedence over any values passed in
                bool duplicateExists = this.queryStringItems.ContainsKey(param.Key);
                if (!duplicateExists)
                {
                    mergedItems.Add(param);
                }
            }

            return new QueryString(ToSortedDictionary(mergedItems));
        }

        private static Dictionary<string, string> Parse(string queryString)
        {
            var resultItems = new Dictionary<string, string>();

            if (queryString.Contains("?"))
                queryString = queryString.Split('?')?[1];

            if (string.IsNullOrEmpty(queryString))
                return resultItems; // empty

            foreach (var token in queryString.Split('&'))
            {
                var pair = token.Split('=');

                if (pair == null)
                    resultItems.Add(token, null);

                var key = pair[0];
                var value = string.Empty;
                if (pair.Length > 1 && !string.IsNullOrEmpty(pair[1]))
                    value = pair[1];
                resultItems.Add(key, value);
            }

            return ToSortedDictionary(resultItems);
        }

        private static bool IsDatetimeSearchCriteria(string key)
        {
            return
                key.Equals("createdAt", StringComparison.InvariantCultureIgnoreCase) ||
                key.Equals("modifiedAt", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// The URLEncoded version of a datetime search criteria string is over-encoded; we need to leave some symbols unencoded.
        /// </summary>
        /// <param name="encodedValue">URLEncoded datetime search criteria string</param>
        /// <returns>Properly encoded Stormpath datetime search criteria string</returns>
        private static string FixDatetimeSearchEncoding(string encodedValue)
        {
            return encodedValue
                .Replace("%5B", "[")
                .Replace("%5D", "]")
                .Replace("%3A", ":")
                .Replace("%2C", ",");
        }

        private static Dictionary<string, string> ToSortedDictionary(IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            return queryParams
                .OrderBy(x => x.Key, StringComparer.InvariantCultureIgnoreCase)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
