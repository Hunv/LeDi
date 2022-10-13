using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeDi.Shared.DtoModel
{
    public class DtoDisplayText
    {
        /// <summary>
        /// The Language this Item is dedicated to
        /// </summary>
        public string Language { get; set; } = "EN";

        /// <summary>
        /// The text that should be shown in the language mentioned above
        /// </summary>
        public string Text { get; set; } = "";
    }
}
