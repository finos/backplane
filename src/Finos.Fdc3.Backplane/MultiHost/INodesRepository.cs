/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

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
        /// List of nodes uri running on other host under same user.
        /// </summary>
        IEnumerable<Uri> MemberNodes { get; }

        /// <summary>
        /// Add a node to repository.
        /// </summary>
        /// <param name="nodeUri"></param>
        void AddNode(Uri nodeUri);

        /// <summary>
        /// Remove node from repository
        /// </summary>
        /// <param name="nodeUri"></param>
        void RemoveNode(Uri nodeUri);

    }
}
