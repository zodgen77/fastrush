using UnityEngine;
using System.Collections.Generic;
using System;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Finance Window with Bank, Stocks, Bonds, and Crypto tabs
    /// Contains simulated market data and trading functionality
    /// Enhanced with interactive banking features
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
            public bool isLoanAccount;
            public float creditLimit;
            public float minimumPayment;
            
            public bool CanWithdraw(float amount)
            {
                if (isLoanAccount)
                {
                    return (balance + amount) <= creditLimit;
                }
                return balance >= amount;
            }
            
            public float GetAvailableCredit()
            {
                if (isLoanAccount)
                {
                    return creditLimit - balance;
                }
                return balance;
            }
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
        private GUIStyle headerStyle;
        private GUIStyle textFieldStyle;
        private bool stylesInitialized = false;
        
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
        private int selectedTradeAmount = 1;
        
        // Banking
        private string loanAmount = "1000";
        private int selectedAccountIndex = 0;
        private bool showLoanOptions = false;
        private bool showTransactionHistory = false;
        private List<string> transactionHistory = new List<string>();
        
        // Preset transaction amounts
        private readonly float[] presetAmounts = { 100f, 1000f, 10000f };
        
        // Property to access centralized money system
        private long PlayerCash => RestaurantManager.Instance.GetMoney();
        private string FormattedCash => RestaurantManager.Instance.GetFormattedMoney(PlayerCash);
        
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
            
            // Note: SetupStyles() moved to DrawWindow() to avoid GUI access errors
            GenerateMarketData();
            GenerateBankAccounts();
            
            // Initialize transaction history
            transactionHistory.Add($"{DateTime.Now:MM/dd HH:mm} - Account opened");
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
            
            // Header style
            headerStyle = new GUIStyle(labelStyle);
            headerStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = new Color(1f, 0.8f, 0f, 1f);
            
            // TextField style
            textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            textFieldStyle.normal.textColor = Color.white;
            
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
                new BankAccount { accountType = "Checking", accountNumber = "****1234", balance = 500.00f, interestRate = 0.01f, lastUpdated = DateTime.Now, isLoanAccount = false, creditLimit = 0, minimumPayment = 0 },
                new BankAccount { accountType = "Savings", accountNumber = "****5678", balance = 1200.00f, interestRate = 2.15f, lastUpdated = DateTime.Now, isLoanAccount = false, creditLimit = 0, minimumPayment = 0 },
                new BankAccount { accountType = "Credit Line", accountNumber = "****9999", balance = 0.00f, interestRate = 8.99f, lastUpdated = DateTime.Now, isLoanAccount = true, creditLimit = 10000.00f, minimumPayment = 0 },
                new BankAccount { accountType = "Business Loan", accountNumber = "****7777", balance = 0.00f, interestRate = 6.25f, lastUpdated = DateTime.Now, isLoanAccount = true, creditLimit = 50000.00f, minimumPayment = 0 }
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
            // Setup styles on first draw call (when GUI functions are available)
            if (!stylesInitialized)
            {
                SetupStyles();
                stylesInitialized = true;
            }
            
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
            // Calculate dynamic height based on content
            float contentHeight = 800 * scaleFactor;
            if (showLoanOptions) contentHeight += 200 * scaleFactor;
            if (showTransactionHistory) contentHeight += 300 * scaleFactor;
            
            scrollPosition = GUI.BeginScrollView(contentRect, scrollPosition, 
                new Rect(0, 0, contentRect.width - 20 * scaleFactor, contentHeight), 
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            
            float yPos = 10 * scaleFactor;
            
            // Draw player cash prominently
            GUI.Label(new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 30 * scaleFactor), 
                     $"Player Cash: {FormattedCash}", headerStyle);
            yPos += 40 * scaleFactor;
            
            // Draw total bank balance
            float totalBalance = 0;
            float totalDebt = 0;
            foreach (var account in bankAccounts)
            {
                if (account.isLoanAccount && account.balance > 0)
                    totalDebt += account.balance;
                else if (!account.isLoanAccount)
                    totalBalance += account.balance;
            }
            
            GUI.Label(new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 25 * scaleFactor), 
                     $"Total Bank Balance: ${totalBalance:N2}", labelStyle);
            yPos += 30 * scaleFactor;
            
            if (totalDebt > 0)
            {
                GUIStyle debtStyle = new GUIStyle(labelStyle);
                debtStyle.normal.textColor = new Color(0.9f, 0.3f, 0.3f, 1f);
                GUI.Label(new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 25 * scaleFactor), 
                         $"Total Debt: ${totalDebt:N2}", debtStyle);
                yPos += 30 * scaleFactor;
            }
            
            // Quick action buttons
            float buttonWidth = (contentRect.width - 60 * scaleFactor) / 3f;
            
            if (GUI.Button(new Rect(10 * scaleFactor, yPos, buttonWidth, 40 * scaleFactor), "LOAN OPTIONS", buttonStyle))
            {
                showLoanOptions = !showLoanOptions;
            }
            
            if (GUI.Button(new Rect(20 * scaleFactor + buttonWidth, yPos, buttonWidth, 40 * scaleFactor), "TRANSACTION HISTORY", buttonStyle))
            {
                showTransactionHistory = !showTransactionHistory;
            }
            
            if (GUI.Button(new Rect(30 * scaleFactor + buttonWidth * 2, yPos, buttonWidth, 40 * scaleFactor), "REFRESH RATES", buttonStyle))
            {
                UpdateInterestRates();
            }
            
            yPos += 50 * scaleFactor;
            
            // Draw loan options if expanded
            if (showLoanOptions)
            {
                DrawLoanOptions(ref yPos, contentRect.width - 40 * scaleFactor);
            }
            
            // Draw transaction history if expanded
            if (showTransactionHistory)
            {
                DrawTransactionHistory(ref yPos, contentRect.width - 40 * scaleFactor);
            }
            
            // Draw each bank account with interactive features
            for (int i = 0; i < bankAccounts.Count; i++)
            {
                DrawBankAccount(bankAccounts[i], i, ref yPos, contentRect.width - 40 * scaleFactor);
            }
            
            GUI.EndScrollView();
        }
        
        private void DrawLoanOptions(ref float yPos, float width)
        {
            Rect loanRect = new Rect(10 * scaleFactor, yPos, width, 180 * scaleFactor);
            GUI.Box(loanRect, "", windowStyle);
            
            GUI.Label(new Rect(loanRect.x + 10 * scaleFactor, loanRect.y + 5 * scaleFactor, 
                              loanRect.width - 20 * scaleFactor, 25 * scaleFactor), 
                     "Loan Options", headerStyle);
            
            // Loan amount input
            GUI.Label(new Rect(loanRect.x + 10 * scaleFactor, loanRect.y + 35 * scaleFactor, 
                              100 * scaleFactor, 20 * scaleFactor), 
                     "Loan Amount:", labelStyle);
            
            loanAmount = GUI.TextField(new Rect(loanRect.x + 120 * scaleFactor, loanRect.y + 35 * scaleFactor, 
                                              100 * scaleFactor, 25 * scaleFactor), 
                                     loanAmount, textFieldStyle);
            
            // Available loan products
            GUI.Label(new Rect(loanRect.x + 10 * scaleFactor, loanRect.y + 70 * scaleFactor, 
                              loanRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                     "Available Loan Products:", labelStyle);
            
            float loanButtonY = loanRect.y + 95 * scaleFactor;
            float loanButtonWidth = (loanRect.width - 40 * scaleFactor) / 2f;
            
            if (GUI.Button(new Rect(loanRect.x + 10 * scaleFactor, loanButtonY, loanButtonWidth, 30 * scaleFactor), 
                          "Credit Line (8.99%)", buttonStyle))
            {
                TakeLoan(2, float.Parse(loanAmount.Length > 0 ? loanAmount : "0"));
            }
            
            if (GUI.Button(new Rect(loanRect.x + 20 * scaleFactor + loanButtonWidth, loanButtonY, loanButtonWidth, 30 * scaleFactor), 
                          "Business Loan (6.25%)", buttonStyle))
            {
                TakeLoan(3, float.Parse(loanAmount.Length > 0 ? loanAmount : "0"));
            }
            
            GUI.Label(new Rect(loanRect.x + 10 * scaleFactor, loanRect.y + 135 * scaleFactor, 
                              loanRect.width - 20 * scaleFactor, 40 * scaleFactor), 
                     "Note: Loans add money to your account and can be withdrawn as cash. Interest accrues daily.", labelStyle);
            
            yPos += 190 * scaleFactor;
        }
        
        private void DrawTransactionHistory(ref float yPos, float width)
        {
            Rect historyRect = new Rect(10 * scaleFactor, yPos, width, 280 * scaleFactor);
            GUI.Box(historyRect, "", windowStyle);
            
            GUI.Label(new Rect(historyRect.x + 10 * scaleFactor, historyRect.y + 5 * scaleFactor, 
                              historyRect.width - 20 * scaleFactor, 25 * scaleFactor), 
                     "Transaction History", headerStyle);
            
            // Display recent transactions
            float historyY = historyRect.y + 35 * scaleFactor;
            int maxTransactions = Mathf.Min(transactionHistory.Count, 10);
            
            for (int i = transactionHistory.Count - 1; i >= Mathf.Max(0, transactionHistory.Count - maxTransactions); i--)
            {
                GUI.Label(new Rect(historyRect.x + 10 * scaleFactor, historyY, 
                                  historyRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                         transactionHistory[i], labelStyle);
                historyY += 25 * scaleFactor;
            }
            
            yPos += 290 * scaleFactor;
        }
        
        private void DrawBankAccount(BankAccount account, int index, ref float yPos, float width)
        {
            Rect accountRect = new Rect(10 * scaleFactor, yPos, width, 200 * scaleFactor);
            GUI.Box(accountRect, "", windowStyle);
            
            // Account header
            string accountLabel = account.isLoanAccount ? 
                $"{account.accountType} {account.accountNumber} (Available: ${account.GetAvailableCredit():N2})" :
                $"{account.accountType} {account.accountNumber}";
            
            GUI.Label(new Rect(accountRect.x + 10 * scaleFactor, accountRect.y + 5 * scaleFactor, 
                              accountRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                     accountLabel, headerStyle);
            
            // Balance/Debt display
            GUIStyle balanceStyle = new GUIStyle(labelStyle);
            string balanceText;
            
            if (account.isLoanAccount)
            {
                if (account.balance > 0)
                {
                    balanceStyle.normal.textColor = new Color(0.9f, 0.3f, 0.3f, 1f);
                    balanceText = $"Debt: ${account.balance:N2}";
                }
                else
                {
                    balanceStyle.normal.textColor = new Color(0.3f, 0.9f, 0.3f, 1f);
                    balanceText = "No Outstanding Debt";
                }
            }
            else
            {
                balanceStyle.normal.textColor = account.balance > 0 ? new Color(0.3f, 0.9f, 0.3f, 1f) : Color.white;
                balanceText = $"Balance: ${account.balance:N2}";
            }
            
            GUI.Label(new Rect(accountRect.x + 10 * scaleFactor, accountRect.y + 30 * scaleFactor, 
                              accountRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                     balanceText, balanceStyle);
            
            // Interest rate
            GUI.Label(new Rect(accountRect.x + 10 * scaleFactor, accountRect.y + 55 * scaleFactor, 
                              accountRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                     $"Interest Rate: {account.interestRate:F2}%", labelStyle);
            
            // Deposit section
            float depositY = accountRect.y + 85 * scaleFactor;
            GUI.Label(new Rect(accountRect.x + 10 * scaleFactor, depositY, 
                              accountRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                     "Deposit:", labelStyle);
            
            // Deposit amount buttons
            float buttonWidth = (accountRect.width - 60 * scaleFactor) / 4f;
            float buttonY = depositY + 25 * scaleFactor;
            
            for (int i = 0; i < presetAmounts.Length; i++)
            {
                if (GUI.Button(new Rect(accountRect.x + 10 * scaleFactor + i * (buttonWidth + 5 * scaleFactor), buttonY, 
                                       buttonWidth, 25 * scaleFactor), 
                              $"${presetAmounts[i]:N0}", buttonStyle))
                {
                    DepositToAccount(index, presetAmounts[i]);
                }
            }
            
            // MAX deposit button
            float maxAmount = PlayerCash;
            if (GUI.Button(new Rect(accountRect.x + 10 * scaleFactor + 3 * (buttonWidth + 5 * scaleFactor), buttonY, 
                                   buttonWidth, 25 * scaleFactor), 
                          "MAX", buttonStyle))
            {
                DepositToAccount(index, maxAmount);
            }
            
            // Withdraw section
            float withdrawY = accountRect.y + 125 * scaleFactor;
            GUI.Label(new Rect(accountRect.x + 10 * scaleFactor, withdrawY, 
                              accountRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                     "Withdraw:", labelStyle);
            
            // Withdraw amount buttons
            buttonY = withdrawY + 25 * scaleFactor;
            
            for (int i = 0; i < presetAmounts.Length; i++)
            {
                bool canWithdraw = account.CanWithdraw(presetAmounts[i]);
                
                // Different button style if can't withdraw
                GUIStyle withdrawButtonStyle = canWithdraw ? buttonStyle : new GUIStyle(buttonStyle);
                if (!canWithdraw)
                {
                    withdrawButtonStyle.normal.textColor = Color.gray;
                }
                
                if (GUI.Button(new Rect(accountRect.x + 10 * scaleFactor + i * (buttonWidth + 5 * scaleFactor), buttonY, 
                                       buttonWidth, 25 * scaleFactor), 
                              $"${presetAmounts[i]:N0}", withdrawButtonStyle))
                {
                    if (canWithdraw)
                    {
                        WithdrawFromAccount(index, presetAmounts[i]);
                    }
                }
            }
            
            // MAX withdraw button
            float maxWithdrawAmount = account.GetAvailableCredit();
            bool canWithdrawMax = maxWithdrawAmount > 0;
            
            GUIStyle maxWithdrawButtonStyle = canWithdrawMax ? buttonStyle : new GUIStyle(buttonStyle);
            if (!canWithdrawMax)
            {
                maxWithdrawButtonStyle.normal.textColor = Color.gray;
            }
            
            if (GUI.Button(new Rect(accountRect.x + 10 * scaleFactor + 3 * (buttonWidth + 5 * scaleFactor), buttonY, 
                                   buttonWidth, 25 * scaleFactor), 
                          "MAX", maxWithdrawButtonStyle))
            {
                if (canWithdrawMax)
                {
                    WithdrawFromAccount(index, maxWithdrawAmount);
                }
            }
            
            // Pay debt button for loan accounts
            if (account.isLoanAccount && account.balance > 0)
            {
                float payDebtY = accountRect.y + 165 * scaleFactor;
                GUI.Label(new Rect(accountRect.x + 10 * scaleFactor, payDebtY, 
                                  accountRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                         "Pay Debt:", labelStyle);
                
                buttonY = payDebtY + 25 * scaleFactor;
                
                // Pay debt amount buttons
                for (int i = 0; i < presetAmounts.Length; i++)
                {
                    float paymentAmount = Mathf.Min(presetAmounts[i], account.balance);
                    bool canPay = PlayerCash >= paymentAmount && paymentAmount > 0;
                    
                    GUIStyle payButtonStyle = canPay ? buttonStyle : new GUIStyle(buttonStyle);
                    if (!canPay)
                    {
                        payButtonStyle.normal.textColor = Color.gray;
                    }
                    
                    if (GUI.Button(new Rect(accountRect.x + 10 * scaleFactor + i * (buttonWidth + 5 * scaleFactor), buttonY, 
                                           buttonWidth, 25 * scaleFactor), 
                                  $"${paymentAmount:N0}", payButtonStyle))
                    {
                        if (canPay)
                        {
                            PayDebt(index, paymentAmount);
                        }
                    }
                }
                
                // PAY ALL debt button
                float payAllAmount = Mathf.Min(PlayerCash, account.balance);
                bool canPayAll = payAllAmount > 0;
                
                GUIStyle payAllButtonStyle = canPayAll ? buttonStyle : new GUIStyle(buttonStyle);
                if (!canPayAll)
                {
                    payAllButtonStyle.normal.textColor = Color.gray;
                }
                
                if (GUI.Button(new Rect(accountRect.x + 10 * scaleFactor + 3 * (buttonWidth + 5 * scaleFactor), buttonY, 
                                       buttonWidth, 25 * scaleFactor), 
                              "PAY ALL", payAllButtonStyle))
                {
                    if (canPayAll)
                    {
                        PayDebt(index, payAllAmount);
                    }
                }
                
                yPos += 240 * scaleFactor;
            }
            else
            {
                yPos += 210 * scaleFactor;
            }
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
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            
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
                GUIStyle changeStyle = labelStyle != null ? new GUIStyle(labelStyle) : new GUIStyle();
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
            if (PlayerCash >= cost)
            {
                RestaurantManager.Instance.AdjustMoney(-(int)cost);
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
                RestaurantManager.Instance.AdjustMoney((int)proceeds);
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
        
        // Banking functionality methods
        private void TakeLoan(int accountIndex, float amount)
        {
            if (amount <= 0 || accountIndex < 0 || accountIndex >= bankAccounts.Count)
            {
                Debug.Log("Invalid loan amount or account.");
                return;
            }
            
            BankAccount account = bankAccounts[accountIndex];
            
            if (!account.isLoanAccount)
            {
                Debug.Log("This account does not support loans.");
                return;
            }
            
            // Check if loan amount exceeds available credit
            if (account.balance + amount > account.creditLimit)
            {
                Debug.Log($"Loan amount exceeds available credit. Available: ${account.GetAvailableCredit():N2}");
                return;
            }
            
            // Add loan to account balance (increases debt)
            account.balance += amount;
            account.lastUpdated = DateTime.Now;
            
            // Add loan proceeds to bank account (can be withdrawn as cash)
            // For loan accounts, we'll add it to the first non-loan account (checking)
            if (bankAccounts.Count > 0 && !bankAccounts[0].isLoanAccount)
            {
                bankAccounts[0].balance += amount;
                bankAccounts[0].lastUpdated = DateTime.Now;
            }
            
            // Log transaction
            string transaction = $"{DateTime.Now:MM/dd HH:mm} - Loan: ${amount:N2} from {account.accountType}";
            transactionHistory.Add(transaction);
            
            Debug.Log($"Loan of ${amount:N2} approved and deposited to account.");
        }
        
        private void DepositToAccount(int accountIndex, float amount)
        {
            if (amount <= 0 || accountIndex < 0 || accountIndex >= bankAccounts.Count)
            {
                Debug.Log("Invalid deposit amount or account.");
                return;
            }
            
            if (PlayerCash < amount)
            {
                Debug.Log("Insufficient cash for deposit.");
                return;
            }
            
            BankAccount account = bankAccounts[accountIndex];
            
            // For loan accounts, deposit pays down debt
            if (account.isLoanAccount)
            {
                float paymentAmount = Mathf.Min(amount, account.balance);
                account.balance -= paymentAmount;
                if (account.balance < 0) account.balance = 0;
                
                // Deduct from player cash
                RestaurantManager.Instance.AdjustMoney(-(int)paymentAmount);
                
                // Log transaction
                string transaction = $"{DateTime.Now:MM/dd HH:mm} - Debt Payment: ${paymentAmount:N2} to {account.accountType}";
                transactionHistory.Add(transaction);
                
                Debug.Log($"Debt payment of ${paymentAmount:N2} made to {account.accountType}");
            }
            else
            {
                // Normal deposit to savings/checking account
                account.balance += amount;
                account.lastUpdated = DateTime.Now;
                
                // Deduct from player cash
                RestaurantManager.Instance.AdjustMoney(-(int)amount);
                
                // Log transaction
                string transaction = $"{DateTime.Now:MM/dd HH:mm} - Deposit: ${amount:N2} to {account.accountType}";
                transactionHistory.Add(transaction);
                
                Debug.Log($"Deposited ${amount:N2} to {account.accountType}");
            }
        }
        
        private void WithdrawFromAccount(int accountIndex, float amount)
        {
            if (amount <= 0 || accountIndex < 0 || accountIndex >= bankAccounts.Count)
            {
                Debug.Log("Invalid withdrawal amount or account.");
                return;
            }
            
            BankAccount account = bankAccounts[accountIndex];
            
            if (!account.CanWithdraw(amount))
            {
                if (account.isLoanAccount)
                {
                    Debug.Log($"Withdrawal would exceed credit limit. Available: ${account.GetAvailableCredit():N2}");
                }
                else
                {
                    Debug.Log($"Insufficient account balance. Available: ${account.balance:N2}");
                }
                return;
            }
            
            if (account.isLoanAccount)
            {
                // For loan accounts, withdrawal increases debt
                account.balance += amount;
                account.lastUpdated = DateTime.Now;
            }
            else
            {
                // Normal withdrawal from savings/checking account
                account.balance -= amount;
                account.lastUpdated = DateTime.Now;
            }
            
            // Add to player cash
            RestaurantManager.Instance.AdjustMoney((int)amount);
            
            // Log transaction
            string transaction = $"{DateTime.Now:MM/dd HH:mm} - Withdrawal: ${amount:N2} from {account.accountType}";
            transactionHistory.Add(transaction);
            
            Debug.Log($"Withdrew ${amount:N2} from {account.accountType}");
        }
        
        private void PayDebt(int accountIndex, float amount)
        {
            if (amount <= 0 || accountIndex < 0 || accountIndex >= bankAccounts.Count)
            {
                Debug.Log("Invalid payment amount or account.");
                return;
            }
            
            BankAccount account = bankAccounts[accountIndex];
            
            if (!account.isLoanAccount || account.balance <= 0)
            {
                Debug.Log("No debt to pay on this account.");
                return;
            }
            
            if (PlayerCash < amount)
            {
                Debug.Log("Insufficient cash for debt payment.");
                return;
            }
            
            // Calculate actual payment amount (can't pay more than owed)
            float paymentAmount = Mathf.Min(amount, account.balance);
            
            // Reduce debt
            account.balance -= paymentAmount;
            if (account.balance < 0) account.balance = 0;
            account.lastUpdated = DateTime.Now;
            
            // Deduct from player cash
            RestaurantManager.Instance.AdjustMoney(-(int)paymentAmount);
            
            // Log transaction
            string transaction = $"{DateTime.Now:MM/dd HH:mm} - Debt Payment: ${paymentAmount:N2} to {account.accountType}";
            transactionHistory.Add(transaction);
            
            Debug.Log($"Debt payment of ${paymentAmount:N2} made to {account.accountType}");
        }
        
        private void UpdateInterestRates()
        {
            // Simulate interest rate changes
            foreach (var account in bankAccounts)
            {
                if (account.isLoanAccount)
                {
                    // Loan rates can fluctuate more
                    float change = UnityEngine.Random.Range(-0.5f, 0.5f);
                    account.interestRate = Mathf.Max(1.0f, account.interestRate + change);
                }
                else
                {
                    // Savings rates fluctuate less
                    float change = UnityEngine.Random.Range(-0.2f, 0.2f);
                    account.interestRate = Mathf.Max(0.01f, account.interestRate + change);
                }
            }
            
            // Log the rate update
            string transaction = $"{DateTime.Now:MM/dd HH:mm} - Interest rates updated";
            transactionHistory.Add(transaction);
            
            Debug.Log("Interest rates updated");
        }
    }
}