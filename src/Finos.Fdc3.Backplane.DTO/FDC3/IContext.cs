/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Finos.Fdc3.Backplane.DTO.FDC3
{
    /// <summary>
    /// Interface for context data shared across apps. Its a JSON which must contain a property 'type' which is unique identifier for context.For example:
    /// {
    ///   "type": "fdc3.instrument",
    ///   "name": "Microsoft",
    ///   "id": {
    ///     "ticker": "MSFT",
    ///     "RIC": "MSFT.OQ",
    ///     "ISIN": "US5949181045"
    ///     }
    /// }
    /// </summary>
    public interface IContext : IDictionary<string, JToken>
    {
        /// <summary>
        /// unique type identifier, for example: fdc3.instrument.
        /// </summary>
        string Type { get; }
    }
}