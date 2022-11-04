/**
* SPDX-License-Identifier: Apache-2.0
* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
*/

using System;
namespace Finos.Fdc3.Backplane.Models
{
    /// <summary>
    /// Backplane node
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Backplane node uri
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Is node active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
