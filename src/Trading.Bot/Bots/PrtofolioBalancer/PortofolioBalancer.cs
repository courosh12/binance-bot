using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trading.Bot.Models;
using Trading.Bot.Repositories;
using Trading.Bot.ServerClients;

namespace Trading.Bot.Bots.PrtofolioBalancer
{
    public class PortofolioBalancer : TradingBotBase<PortofolioBalancer, PortofolioBalancerOptions>
    {

        public PortofolioBalancer(ILogger<PortofolioBalancer> logger, TradeHistoryRepository tradeHistoryRepository)
            : base(logger, tradeHistoryRepository, Enums.BotName.PORTOFOLIO_BALANCER)
        { }

        protected override void SetBotIdentifier()
        {
            var botId = $"{Botname}.{BotOptions.Exchange}";
            foreach (var asset in BotOptions.Allocation)
            {
                botId += $".{asset.Name}.{asset.Percantage}";
            }
            BotIdentifier = botId;
        }

        protected override async Task ExecuteBotStepsAsync(CancellationToken canceltToken)
        {
            var balance = await GetCurrentBalancesAsync();
            var rapport = await CalculateCurrenBalanceDistributionAsync(balance);
            throw new NotImplementedException();
        }

        private async Task<List<AssetBalance>> GetCurrentBalancesAsync()
        {
            var allBalances = await TradingClient.GetAllBalances();
            var portofolio = allBalances.Where(p => BotOptions.Allocation.Exists(q => q.Name == p.Name)).ToList();
            return portofolio;
        }

        private async Task<PortofolioDistribuitonRapport> CalculateCurrenBalanceDistributionAsync(List<AssetBalance> balance)
        {
            var portofolio = new PortofolioDistribuitonRapport();
            var assetRaportList = new List<AssetRapport>();

            foreach (var asset in balance)
            {
                var assetRapport = new AssetRapport();
                assetRapport.Name = asset.Name;
                assetRapport.Value = asset.Value;

                if (asset.Name == BotOptions.Market)
                    assetRapport.Price = 1;
                else
                    assetRapport.Price = await TradingClient.GetHighestBidOrderAsync(asset.Name + BotOptions.Market);

                assetRaportList.Add(assetRapport);
            }

            portofolio.TotalValue = assetRaportList.Sum(p => p.Value * p.Price);

            var logRapport = $"{Environment.NewLine}Portofolio balances: Total: {decimal.Round(portofolio.TotalValue,2) + Environment.NewLine}";

            foreach (var asset in assetRaportList)
            {
                asset.PercantageOfPortofolio =
                    decimal.Round((((asset.Value * asset.Price) / portofolio.TotalValue) * 100), 2);

                logRapport += $"Name: {asset.Name} Value: {asset.Value} ValueInMarketBase: {decimal.Round(asset.Value * asset.Price,2)}" +
                    $" PercentageOfPortofolio:{asset.PercantageOfPortofolio}" + Environment.NewLine;
            }

            portofolio.Assets = assetRaportList;
            Logger.LogInformation(logRapport);

            return portofolio;
        }

        protected override Task ExitingBotAsync()
        {
            throw new NotImplementedException();
        }
    }
}
