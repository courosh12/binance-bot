﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.Bot.Models
{
    public class AssetRapport
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Price { get; set; }
        public decimal PercentageOfPortofolio { get; set; }
        public decimal TargetPercentageOfPortofolio { get; set; }

        public decimal DifferenceInTargetAndCurrent => TargetPercentageOfPortofolio - PercentageOfPortofolio;
    }
}
