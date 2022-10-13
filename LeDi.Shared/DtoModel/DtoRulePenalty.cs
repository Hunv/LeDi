using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeDi.Shared.DtoModel
{
    public class DtoRulePenalty
    {
        public string? Name { get; set; }

        public int PenaltySeconds { get; set; }

        public bool TotalDismissal { get; set; }

        public List<DtoDisplayText> Display { get; set; } = new List<DtoDisplayText>();
    }
}
