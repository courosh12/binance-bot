using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Bot.Models
{
    public class PortofolioDistribuitonRapport
    {
        public decimal TotalValue { get; set; }
        public List<AssetRapport> Assets { get; set; }

    }
}
