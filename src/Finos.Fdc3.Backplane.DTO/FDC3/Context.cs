/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Newtonsoft.Json.Linq;
using System;

namespace Finos.Fdc3.Backplane.DTO.FDC3
{
    /// <summary>
    /// Context data shared across apps. Its a JSON which must contain a property 'type' which is unique identifier for context.
    /// </summary>
    public class Context : JObject, IContext
    {
        /// <summary>
        /// context type
        /// </summary>
        public new string Type
        {
            get;
        }

        /// <summary>
        /// Creates context object.
        /// </summary>
        /// <param name="context">
        /// Fdc3 context JSON as JObject. It must contain 'type' property which uniquely identifies it. 
        /// Example context JSON:
        /// {
        ///   "type": "fdc3.instrument",
        ///   "name": "Microsoft",
        ///   "id": {
        ///     "ticker": "MSFT",
        ///     "RIC": "MSFT.OQ",
        ///     "ISIN": "US5949181045"
        ///     }
        /// }
        /// </param>
        /// <exception cref="ArgumentException">Invalid context json is provided.</exception>
        public Context(JObject context) : base(context)
        {
            if (!context.TryGetValue("type", StringComparison.InvariantCultureIgnoreCase, out JToken type) && type.Type != JTokenType.String)
            {
                throw new ArgumentException("'type' property with string value is required in context json");
            }
            Type = type.ToString();

        }
    }
}
