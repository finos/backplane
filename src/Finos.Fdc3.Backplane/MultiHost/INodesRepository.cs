/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Models;
using System;
using System.Collections.Generic;

namespace Finos.Fdc3.Backplane.MultiHost
{
    /// <summary>
    /// Member backplane repository.
    /// </summary>
    public interface INodesRepository
    {
        /// <summary>
        /// List of other nodes running on other host under same user.
        /// </summary>
        IEnumerable<Node> MemberNodes { get; }

        /// <summary>
        /// Add/update active nodes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        void AddOrUpdateActiveNode(Uri value);

        /// <summary>
        /// Add or update deactive node list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        void AddOrUpdateDeactiveNode(Uri value);
    }
}
