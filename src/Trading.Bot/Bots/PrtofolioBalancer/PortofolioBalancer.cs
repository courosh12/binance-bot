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

        protected override void StartingUpBot()
        {
            var botId = $"{Botname}.{BotOptions.Exchange}";
            foreach (var asset in BotOptions.Allocation)
            {
                botId += $".{asset.Name}.{asset.Percantage}";
            }
            BotIdentifier = botId;

            var sum = BotOptions.Allocation.Sum(p => p.Percantage);
            if (sum != 100)
                throw new Exception("Sum of percanteges for Allocation must be 100");
        }

        protected override async Task ExecuteBotStepsAsync(CancellationToken canceltToken)
        {
            var balance = await GetCurrentBalancesAsync();
            var rapport = await CalculateCurrentBalanceDistributionAsync(balance);
            var orders = GetRebalanceOrders(rapport);
            var placedOrders = await TradingClient.PlaceMarketOrdersAsync(orders);
            await UpdateOrderHistory(placedOrders);
            balance = await GetCurrentBalancesAsync();
            rapport = await CalculateCurrentBalanceDistributionAsync(balance);
        }

        private List<Order> GetRebalanceOrders(PortofolioDistribuitonRapport rapport)
        {
            var assetsInSurplus = new List<AssetRapport>();
            var assetsInDeficit = new List<AssetRapport>();

            foreach (var asset in rapport.Assets)
            {
                if (Math.Abs(asset.DifferenceInTargetAndCurrent) > BotOptions.Trigger)
                {
                    if (asset.DifferenceInTargetAndCurrent > 0)
                        assetsInDeficit.Add(asset);
                    else
                        assetsInSurplus.Add(asset);
                }
            }

            var message = $"Assets in surplus: {Environment.NewLine}";
            assetsInSurplus.ForEach(asset => message += $"Name: {asset.Name} Percentage: {Math.Abs(asset.DifferenceInTargetAndCurrent)}" + Environment.NewLine);
            message += $"Assets in deficit: {Environment.NewLine}";
            assetsInDeficit.ForEach(asset => message += $"Name: {asset.Name} Percentage: {asset.DifferenceInTargetAndCurrent}" + Environment.NewLine);

            Logger.LogInformation(message);

            var orders = new List<Order>();

            foreach (var asset in assetsInSurplus)
            {
                if (asset.Name == BotOptions.Market)
                    continue;

                orders.Add(new Order()
                {
                    Symbol = asset.Name + BotOptions.Market,
                    OrderSide = Enums.OrderSide.SELL,
                    Price = asset.Price,
                    Quantity = (((Math.Abs(asset.DifferenceInTargetAndCurrent) / 100) * rapport.TotalValue) / asset.Price)
                });
            }

            foreach (var asset in assetsInDeficit)
            {
                if (asset.Name == BotOptions.Market)
                    continue;

                orders.Add(new Order()
                {
                    Symbol = asset.Name + BotOptions.Market,
                    OrderSide = Enums.OrderSide.BUY,
                    Price = asset.Price,
                    Quantity = (((Math.Abs(asset.DifferenceInTargetAndCurrent) / 100) * rapport.TotalValue) / asset.Price)
                });
            }

            return orders;
        }


        private async Task<List<AssetBalance>> GetCurrentBalancesAsync()
        {
            var allBalances = await TradingClient.GetAllBalances();
            var portofolio = allBalances.Where(p => BotOptions.Allocation.Exists(q => q.Name == p.Name)).ToList();
            return portofolio;
        }

        private async Task<PortofolioDistribuitonRapport> CalculateCurrentBalanceDistributionAsync(List<AssetBalance> balance)
        {
            var portofolio = new PortofolioDistribuitonRapport();
            var assetRaportList = new List<AssetRapport>();

            foreach (var asset in balance)
            {
                var assetRapport = new AssetRapport();
                assetRapport.Name = asset.Name;
                assetRapport.Value = asset.Value;
                assetRapport.TargetPercentageOfPortofolio = BotOptions.Allocation.Where(p => p.Name == asset.Name).Single().Percantage;

                if (asset.Name == BotOptions.Market)
                    assetRapport.Price = 1;
                else
                    assetRapport.Price = await TradingClient.GetHighestBidOrderAsync(asset.Name + BotOptions.Market);

                assetRaportList.Add(assetRapport);
            }

            portofolio.TotalValue = assetRaportList.Sum(p => p.Value * p.Price);

            var logRapport = $"{Environment.NewLine}Portofolio balances: Total: {decimal.Round(portofolio.TotalValue, 2) + Environment.NewLine}";

            foreach (var asset in assetRaportList)
            {
                asset.PercentageOfPortofolio =
                    decimal.Round((((asset.Value * asset.Price) / portofolio.TotalValue) * 100), 2);

                logRapport += $"Name: {asset.Name} Value: {asset.Value} ValueInMarketBase: {decimal.Round(asset.Value * asset.Price, 2)}" +
                    $" PercentageOfPortofolio: {asset.PercentageOfPortofolio}" + Environment.NewLine;
            }

            portofolio.Assets = assetRaportList;
            Logger.LogInformation(logRapport);

            return portofolio;
        }

        protected override Task ExitingBotAsync()
        {
            return Task.CompletedTask;
        }
    }
}
