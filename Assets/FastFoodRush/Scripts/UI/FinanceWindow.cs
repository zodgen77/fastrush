using UnityEngine;
using System.Collections.Generic;
using System;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Finance Window with Bank, Stocks, Bonds, and Crypto tabs
    /// Contains simulated market data and trading functionality
    /// </summary>
    public class FinanceWindow : MonoBehaviour
    {
        [System.Serializable]
        public class FinancialAsset
        {
            public string symbol;
            public string name;
            public float currentPrice;
            public float previousPrice;
            public float changePercent;
            public int sharesOwned;
            public float marketCap;
            public string sector;
            
            public Color GetChangeColor()
            {
                return changePercent >= 0 ? new Color(0, 0.8f, 0, 1) : new Color(0.8f, 0, 0, 1);
            }
            
            public string GetChangeText()
            {
                string sign = changePercent >= 0 ? "+" : "";
                return $"{sign}{changePercent:F2}%";
            }
        }
        
        [System.Serializable]
        public class BankAccount
        {
            public string accountType;
            public string accountNumber;
            public float balance;
            public float interestRate;
            public DateTime lastUpdated;
        }
        
        // Tab system
        private enum FinanceTab { Bank, Stocks, Bonds, Crypto }
        private FinanceTab currentTab = FinanceTab.Bank;
        
        // GUI properties
        private Rect windowRect;
        private float scaleFactor;
        private GUIStyle tabStyle;
        private GUIStyle activeTabStyle;
        private GUIStyle windowStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;
        private GUIStyle scrollViewStyle;
        
        // Scrolling
        private Vector2 scrollPosition = Vector2.zero;
        
        // Market data
        private List<FinancialAsset> stocks;
        private List<FinancialAsset> bonds;
        private List<FinancialAsset> cryptos;
        private List<BankAccount> bankAccounts;
        
        // Market simulation
        private float lastMarketUpdate;
        private float marketUpdateInterval = 5f; // Update every 5 seconds
        
        // Trading
        private float playerCash = 50000f;
        private int selectedTradeAmount = 1;
        
        public void Initialize(Rect screenRect, float scale)
        {
            scaleFactor = scale;
            
            // Set window rect (covers most of the screen except bottom button area)
            windowRect = new Rect(
                screenRect.x + 10 * scaleFactor,
                screenRect.y + 60 * scaleFactor,
                screenRect.width - 20 * scaleFactor,
                screenRect.height - 160 * scaleFactor
            );
            
            SetupStyles();
            GenerateMarketData();
            GenerateBankAccounts();
        }
        
        private void SetupStyles()
        {
            // Tab style
            tabStyle = new GUIStyle(GUI.skin.button);
            tabStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            tabStyle.fontStyle = FontStyle.Bold;
            tabStyle.normal.textColor = Color.white;
            tabStyle.normal.background = MakeTexture(new Color(0.3f, 0.3f, 0.3f, 0.8f));
            
            // Active tab style
            activeTabStyle = new GUIStyle(tabStyle);
            activeTabStyle.normal.background = MakeTexture(new Color(0.1f, 0.4f, 0.7f, 0.9f));
            
            // Window style
            windowStyle = new GUIStyle(GUI.skin.box);
            windowStyle.normal.background = MakeTexture(new Color(0.1f, 0.1f, 0.1f, 0.9f));
            
            // Label style
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = Mathf.RoundToInt(11 * scaleFactor);
            labelStyle.normal.textColor = Color.white;
            
            // Button style
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = Mathf.RoundToInt(10 * scaleFactor);
            buttonStyle.normal.textColor = Color.white;
            
            // Scroll view style
            scrollViewStyle = new GUIStyle(GUI.skin.scrollView);
            scrollViewStyle.normal.background = MakeTexture(new Color(0.05f, 0.05f, 0.05f, 0.8f));
        }
        
        private void GenerateMarketData()
        {
            // Generate stocks
            stocks = new List<FinancialAsset>
            {
                new FinancialAsset { symbol = "AAPL", name = "Apple Inc.", currentPrice = 175.43f, previousPrice = 172.81f, changePercent = 1.52f, sharesOwned = 0, marketCap = 2.8f, sector = "Technology" },
                new FinancialAsset { symbol = "MSFT", name = "Microsoft Corp.", currentPrice = 378.85f, previousPrice = 381.22f, changePercent = -0.62f, sharesOwned = 0, marketCap = 2.9f, sector = "Technology" },
                new FinancialAsset { symbol = "GOOGL", name = "Alphabet Inc.", currentPrice = 132.76f, previousPrice = 129.44f, changePercent = 2.57f, sharesOwned = 0, marketCap = 1.7f, sector = "Technology" },
                new FinancialAsset { symbol = "AMZN", name = "Amazon.com Inc.", currentPrice = 144.73f, previousPrice = 147.29f, changePercent = -1.74f, sharesOwned = 0, marketCap = 1.5f, sector = "Consumer Cyclical" },
                new FinancialAsset { symbol = "TSLA", name = "Tesla Inc.", currentPrice = 248.91f, previousPrice = 245.12f, changePercent = 1.55f, sharesOwned = 0, marketCap = 0.8f, sector = "Consumer Cyclical" },
                new FinancialAsset { symbol = "NVDA", name = "NVIDIA Corp.", currentPrice = 875.28f, previousPrice = 892.11f, changePercent = -1.89f, sharesOwned = 0, marketCap = 2.2f, sector = "Technology" },
                new FinancialAsset { symbol = "META", name = "Meta Platforms", currentPrice = 484.20f, previousPrice = 478.33f, changePercent = 1.23f, sharesOwned = 0, marketCap = 1.2f, sector = "Communication" },
                new FinancialAsset { symbol = "JPM", name = "JPMorgan Chase", currentPrice = 179.64f, previousPrice = 181.77f, changePercent = -1.17f, sharesOwned = 0, marketCap = 0.5f, sector = "Financial Services" }
            };
            
            // Generate bonds
            bonds = new List<FinancialAsset>
            {
                new FinancialAsset { symbol = "US10Y", name = "US 10-Year Treasury", currentPrice = 4.52f, previousPrice = 4.48f, changePercent = 0.89f, sharesOwned = 0, marketCap = 0, sector = "Government" },
                new FinancialAsset { symbol = "US30Y", name = "US 30-Year Treasury", currentPrice = 4.68f, previousPrice = 4.71f, changePercent = -0.64f, sharesOwned = 0, marketCap = 0, sector = "Government" },
                new FinancialAsset { symbol = "CORP20", name = "Corporate Bond AAA", currentPrice = 5.12f, previousPrice = 5.09f, changePercent = 0.59f, sharesOwned = 0, marketCap = 0, sector = "Corporate" },
                new FinancialAsset { symbol = "MUNI15", name = "Municipal Bond 15Y", currentPrice = 3.87f, previousPrice = 3.91f, changePercent = -1.02f, sharesOwned = 0, marketCap = 0, sector = "Municipal" },
                new FinancialAsset { symbol = "JUNK", name = "High Yield Bond", currentPrice = 8.23f, previousPrice = 8.31f, changePercent = -0.96f, sharesOwned = 0, marketCap = 0, sector = "High Yield" },
                new FinancialAsset { symbol = "INTL", name = "International Bond", currentPrice = 6.45f, previousPrice = 6.39f, changePercent = 0.94f, sharesOwned = 0, marketCap = 0, sector = "International" }
            };
            
            // Generate cryptocurrencies
            cryptos = new List<FinancialAsset>
            {
                new FinancialAsset { symbol = "BTC", name = "Bitcoin", currentPrice = 67420.30f, previousPrice = 65892.44f, changePercent = 2.32f, sharesOwned = 0, marketCap = 1.3f, sector = "Cryptocurrency" },
                new FinancialAsset { symbol = "ETH", name = "Ethereum", currentPrice = 3847.22f, previousPrice = 3912.88f, changePercent = -1.68f, sharesOwned = 0, marketCap = 0.5f, sector = "Cryptocurrency" },
                new FinancialAsset { symbol = "BNB", name = "Binance Coin", currentPrice = 618.73f, previousPrice = 604.21f, changePercent = 2.40f, sharesOwned = 0, marketCap = 0.1f, sector = "Cryptocurrency" },
                new FinancialAsset { symbol = "ADA", name = "Cardano", currentPrice = 0.57f, previousPrice = 0.55f, changePercent = 3.64f, sharesOwned = 0, marketCap = 0.02f, sector = "Cryptocurrency" },
                new FinancialAsset { symbol = "SOL", name = "Solana", currentPrice = 201.44f, previousPrice = 195.73f, changePercent = 2.92f, sharesOwned = 0, marketCap = 0.09f, sector = "Cryptocurrency" },
                new FinancialAsset { symbol = "DOT", name = "Polkadot", currentPrice = 6.83f, previousPrice = 7.12f, changePercent = -4.07f, sharesOwned = 0, marketCap = 0.01f, sector = "Cryptocurrency" },
                new FinancialAsset { symbol = "MATIC", name = "Polygon", currentPrice = 0.89f, previousPrice = 0.86f, changePercent = 3.49f, sharesOwned = 0, marketCap = 0.008f, sector = "Cryptocurrency" },
                new FinancialAsset { symbol = "DOGE", name = "Dogecoin", currentPrice = 0.16f, previousPrice = 0.15f, changePercent = 6.67f, sharesOwned = 0, marketCap = 0.02f, sector = "Cryptocurrency" }
            };
        }
        
        private void GenerateBankAccounts()
        {
            bankAccounts = new List<BankAccount>
            {
                new BankAccount { accountType = "Checking", accountNumber = "****1234", balance = 15420.33f, interestRate = 0.01f, lastUpdated = DateTime.Now },
                new BankAccount { accountType = "Savings", accountNumber = "****5678", balance = 45230.12f, interestRate = 2.15f, lastUpdated = DateTime.Now },
                new BankAccount { accountType = "Money Market", accountNumber = "****9012", balance = 78540.89f, interestRate = 3.45f, lastUpdated = DateTime.Now },
                new BankAccount { accountType = "CD 12-Month", accountNumber = "****3456", balance = 25000.00f, interestRate = 4.25f, lastUpdated = DateTime.Now.AddDays(-90) }
            };
        }
        
        private Texture2D MakeTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
        
        public void DrawWindow()
        {
            // Update market data periodically
            if (Time.time - lastMarketUpdate > marketUpdateInterval)
            {
                UpdateMarketData();
                lastMarketUpdate = Time.time;
            }
            
            // Draw window background
            GUI.Box(windowRect, "", windowStyle);
            
            // Draw tabs
            DrawTabs();
            
            // Draw content based on active tab
            Rect contentRect = new Rect(
                windowRect.x + 10 * scaleFactor,
                windowRect.y + 60 * scaleFactor,
                windowRect.width - 20 * scaleFactor,
                windowRect.height - 70 * scaleFactor
            );
            
            switch (currentTab)
            {
                case FinanceTab.Bank:
                    DrawBankContent(contentRect);
                    break;
                case FinanceTab.Stocks:
                    DrawStocksContent(contentRect);
                    break;
                case FinanceTab.Bonds:
                    DrawBondsContent(contentRect);
                    break;
                case FinanceTab.Crypto:
                    DrawCryptoContent(contentRect);
                    break;
            }
        }
        
        private void DrawTabs()
        {
            float tabWidth = windowRect.width / 4f;
            float tabHeight = 40 * scaleFactor;
            float tabY = windowRect.y + 10 * scaleFactor;
            
            // Bank tab
            if (GUI.Button(new Rect(windowRect.x, tabY, tabWidth, tabHeight), "BANK", 
                          currentTab == FinanceTab.Bank ? activeTabStyle : tabStyle))
            {
                currentTab = FinanceTab.Bank;
                scrollPosition = Vector2.zero;
            }
            
            // Stocks tab
            if (GUI.Button(new Rect(windowRect.x + tabWidth, tabY, tabWidth, tabHeight), "STOCKS", 
                          currentTab == FinanceTab.Stocks ? activeTabStyle : tabStyle))
            {
                currentTab = FinanceTab.Stocks;
                scrollPosition = Vector2.zero;
            }
            
            // Bonds tab
            if (GUI.Button(new Rect(windowRect.x + tabWidth * 2, tabY, tabWidth, tabHeight), "BONDS", 
                          currentTab == FinanceTab.Bonds ? activeTabStyle : tabStyle))
            {
                currentTab = FinanceTab.Bonds;
                scrollPosition = Vector2.zero;
            }
            
            // Crypto tab
            if (GUI.Button(new Rect(windowRect.x + tabWidth * 3, tabY, tabWidth, tabHeight), "CRYPTO", 
                          currentTab == FinanceTab.Crypto ? activeTabStyle : tabStyle))
            {
                currentTab = FinanceTab.Crypto;
                scrollPosition = Vector2.zero;
            }
        }
        
        private void DrawBankContent(Rect contentRect)
        {
            scrollPosition = GUI.BeginScrollView(contentRect, scrollPosition, 
                new Rect(0, 0, contentRect.width - 20 * scaleFactor, bankAccounts.Count * 120 * scaleFactor), 
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar, scrollViewStyle);
            
            float yPos = 10 * scaleFactor;
            
            // Draw total balance
            float totalBalance = 0;
            foreach (var account in bankAccounts)
                totalBalance += account.balance;
            
            GUI.Label(new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 25 * scaleFactor), 
                     $"Total Balance: ${totalBalance:N2}", labelStyle);
            yPos += 35 * scaleFactor;
            
            // Draw each bank account
            foreach (var account in bankAccounts)
            {
                Rect accountRect = new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 100 * scaleFactor);
                GUI.Box(accountRect, "", windowStyle);
                
                // Account details
                GUI.Label(new Rect(accountRect.x + 10 * scaleFactor, accountRect.y + 5 * scaleFactor, 
                                  accountRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                         $"{account.accountType} {account.accountNumber}", labelStyle);
                
                GUI.Label(new Rect(accountRect.x + 10 * scaleFactor, accountRect.y + 25 * scaleFactor, 
                                  accountRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                         $"Balance: ${account.balance:N2}", labelStyle);
                
                GUI.Label(new Rect(accountRect.x + 10 * scaleFactor, accountRect.y + 45 * scaleFactor, 
                                  accountRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                         $"Interest Rate: {account.interestRate:F2}%", labelStyle);
                
                GUI.Label(new Rect(accountRect.x + 10 * scaleFactor, accountRect.y + 65 * scaleFactor, 
                                  accountRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                         $"Last Updated: {account.lastUpdated:MM/dd/yyyy}", labelStyle);
                
                yPos += 110 * scaleFactor;
            }
            
            GUI.EndScrollView();
        }
        
        private void DrawStocksContent(Rect contentRect)
        {
            DrawAssetList(contentRect, stocks, "Stock");
        }
        
        private void DrawBondsContent(Rect contentRect)
        {
            DrawAssetList(contentRect, bonds, "Bond");
        }
        
        private void DrawCryptoContent(Rect contentRect)
        {
            DrawAssetList(contentRect, cryptos, "Crypto");
        }
        
        private void DrawAssetList(Rect contentRect, List<FinancialAsset> assets, string assetType)
        {
            scrollPosition = GUI.BeginScrollView(contentRect, scrollPosition, 
                new Rect(0, 0, contentRect.width - 20 * scaleFactor, assets.Count * 80 * scaleFactor), 
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar, scrollViewStyle);
            
            float yPos = 10 * scaleFactor;
            
            foreach (var asset in assets)
            {
                Rect assetRect = new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 70 * scaleFactor);
                GUI.Box(assetRect, "", windowStyle);
                
                // Symbol and name
                GUI.Label(new Rect(assetRect.x + 10 * scaleFactor, assetRect.y + 5 * scaleFactor, 
                                  assetRect.width * 0.4f, 20 * scaleFactor), 
                         $"{asset.symbol} - {asset.name}", labelStyle);
                
                // Price
                GUI.Label(new Rect(assetRect.x + 10 * scaleFactor, assetRect.y + 25 * scaleFactor, 
                                  assetRect.width * 0.3f, 20 * scaleFactor), 
                         assetType == "Bond" ? $"{asset.currentPrice:F2}%" : $"${asset.currentPrice:F2}", labelStyle);
                
                // Change
                GUIStyle changeStyle = new GUIStyle(labelStyle);
                changeStyle.normal.textColor = asset.GetChangeColor();
                GUI.Label(new Rect(assetRect.x + assetRect.width * 0.3f, assetRect.y + 25 * scaleFactor, 
                                  assetRect.width * 0.2f, 20 * scaleFactor), 
                         asset.GetChangeText(), changeStyle);
                
                // Owned shares
                GUI.Label(new Rect(assetRect.x + 10 * scaleFactor, assetRect.y + 45 * scaleFactor, 
                                  assetRect.width * 0.3f, 20 * scaleFactor), 
                         $"Owned: {asset.sharesOwned}", labelStyle);
                
                // Buy/Sell buttons
                float buttonWidth = 60 * scaleFactor;
                float buttonHeight = 25 * scaleFactor;
                
                if (GUI.Button(new Rect(assetRect.x + assetRect.width - buttonWidth * 2 - 15 * scaleFactor, 
                                       assetRect.y + 5 * scaleFactor, buttonWidth, buttonHeight), "BUY", buttonStyle))
                {
                    BuyAsset(asset);
                }
                
                if (GUI.Button(new Rect(assetRect.x + assetRect.width - buttonWidth - 5 * scaleFactor, 
                                       assetRect.y + 5 * scaleFactor, buttonWidth, buttonHeight), "SELL", buttonStyle))
                {
                    SellAsset(asset);
                }
                
                yPos += 80 * scaleFactor;
            }
            
            GUI.EndScrollView();
        }
        
        private void BuyAsset(FinancialAsset asset)
        {
            float cost = asset.currentPrice * selectedTradeAmount;
            if (playerCash >= cost)
            {
                playerCash -= cost;
                asset.sharesOwned += selectedTradeAmount;
                Debug.Log($"Bought {selectedTradeAmount} shares of {asset.symbol} for ${cost:F2}");
            }
            else
            {
                Debug.Log("Insufficient funds!");
            }
        }
        
        private void SellAsset(FinancialAsset asset)
        {
            if (asset.sharesOwned >= selectedTradeAmount)
            {
                float proceeds = asset.currentPrice * selectedTradeAmount;
                playerCash += proceeds;
                asset.sharesOwned -= selectedTradeAmount;
                Debug.Log($"Sold {selectedTradeAmount} shares of {asset.symbol} for ${proceeds:F2}");
            }
            else
            {
                Debug.Log("Insufficient shares!");
            }
        }
        
        private void UpdateMarketData()
        {
            // Simulate market movements
            foreach (var stock in stocks)
            {
                stock.previousPrice = stock.currentPrice;
                float change = UnityEngine.Random.Range(-0.05f, 0.05f); // -5% to +5% change
                stock.currentPrice *= (1 + change);
                stock.changePercent = (stock.currentPrice - stock.previousPrice) / stock.previousPrice * 100f;
            }
            
            foreach (var bond in bonds)
            {
                bond.previousPrice = bond.currentPrice;
                float change = UnityEngine.Random.Range(-0.01f, 0.01f); // -1% to +1% change
                bond.currentPrice *= (1 + change);
                bond.changePercent = (bond.currentPrice - bond.previousPrice) / bond.previousPrice * 100f;
            }
            
            foreach (var crypto in cryptos)
            {
                crypto.previousPrice = crypto.currentPrice;
                float change = UnityEngine.Random.Range(-0.1f, 0.1f); // -10% to +10% change
                crypto.currentPrice *= (1 + change);
                crypto.changePercent = (crypto.currentPrice - crypto.previousPrice) / crypto.previousPrice * 100f;
            }
        }
    }
}