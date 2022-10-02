using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeDi.Shared.DtoModel
{
    public class DtoMatchCore
    {
        /// <summary>
        /// The time left for the requested match
        /// </summary>
        public int TimeLeftSeconds { get; set; }

        /// <summary>
        /// A hash over all other properties of the match
        /// </summary>
        public int PropertyHash { get; set; }
    }
}
