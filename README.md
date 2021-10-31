# binance-bot
### Strategy : Volatility
Bot that buys/sells x amount of y when price rises/drops z percentage.
For each bot add separate setting in Botsettings:

    "BotSettings": [
        {  
            "Symbol": "MATICBUSD",  //symbol
            "TimeSpan": "15",  //Minutes to calculate change in price
            "ChangeInPrice": "2",  //percentage change to trigger buy or sell
            "QuantityInDollar": "11"  //speaks for itself
      }, 
      { 
            "Symbol": "SCRTBUSD",  
            "TimeSpan": "15",  
            "ChangeInPrice": "2",  
            "QuantityInDollar": "11"  
      }, 
      { 
            "Symbol": "ATOMBUSD",  
            "TimeSpan": "15",  
            "ChangeInPrice": "2",  
            "QuantityInDollar": "11"  
      } 
    ]
