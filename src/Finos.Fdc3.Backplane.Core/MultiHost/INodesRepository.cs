/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Core.Model;
using System;
using System.Collections.Generic;

namespace Finos.Fdc3.Backplane.Core.MultiHost
{
    /// <summary>
    /// This is interface for member backplane list.  
    /// </summary>
    public interface INodesRepository
    {
        /// <summary>
        /// List of other nodes running on other host under same user.
        /// </summary>
        IReadOnlyList<Node> MemberNodes { get; }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool AddOrUpdateActiveNode(Uri value);

        /// <summary>
        /// Add or update deactive node list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool AddOrUpdateDeactiveNode(Uri value);
    }
}
