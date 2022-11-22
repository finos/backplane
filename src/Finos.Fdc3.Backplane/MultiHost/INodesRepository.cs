/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using System;
using System.Collections.Generic;

namespace Finos.Fdc3.Backplane.MultiHost
{
    /// <summary>
    /// Member nodes repository.
    /// This is updated with latest live nodes through health check.
    /// </summary>
    public interface INodesRepository
    {
        /// <summary>
        /// List of nodes uri running on other host under same user.
        /// Broadcast context propagation happens over nodes in this list only.
        /// </summary>
        IEnumerable<Uri> MemberNodes { get; }

        /// <summary>
        /// Add a node to repository.For example dead node comes alive, it adds itself as member node
        /// </summary>
        /// <param name="nodeUri"></param>
        void AddNode(Uri nodeUri);

        /// <summary>
        /// Remove node from repository. For example a dead/non-responding node.
        /// </summary>
        /// <param name="nodeUri"></param>
        void RemoveNode(Uri nodeUri);

    }
}
