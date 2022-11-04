using System;

namespace Finos.Fdc3.Backplane.Core.Model
{
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
