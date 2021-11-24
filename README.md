# binance-bot
### Strategy : Volatility
Bot that buys/sells x amount of y when price rises/drops z percentage.
For each bot add separate setting in Botsettings:

    "BotSettings": [
        {  
            "Symbol": "BTCUSDT",  //symbol
            "TimeSpan": "15",  //Minutes to calculate change in price
            "ChangeInPriceUp": "2",  //percentage change to trigger sell
            "ChangeInPriceDown": "2", //percentage change to trigger buy 
            "QuantityInDollar": "11"  //speaks for itself
      }, 
        {  
            "Symbol": "ETHUSDT",  //symbol
            "TimeSpan": "15",  //Minutes to calculate change in price
            "ChangeInPriceUp": "2",  //percentage change to trigger sell
            "ChangeInPriceDown": "2", //percentage change to trigger buy 
            "QuantityInDollar": "11"  //speaks for itself
      }, 
    ]
