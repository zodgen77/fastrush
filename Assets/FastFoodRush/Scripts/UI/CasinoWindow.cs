using UnityEngine;
using System.Collections.Generic;
using System;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Casino Window for online betting with various games
    /// Auto banks in winnings to player account
    /// </summary>
    public class CasinoWindow : MonoBehaviour
    {
        [System.Serializable]
        public class CasinoGame
        {
            public string name;
            public string description;
            public float minBet;
            public float maxBet;
            public float houseEdge;
            public bool isActive;
            public int playersOnline;
        }
        
        [System.Serializable]
        public class BetHistory
        {
            public string gameName;
            public float betAmount;
            public float winAmount;
            public DateTime timeStamp;
            public bool isWin;
            
            public string GetResultText()
            {
                return isWin ? $"WON ${winAmount:F2}" : $"LOST ${betAmount:F2}";
            }
            
            public Color GetResultColor()
            {
                return isWin ? new Color(0, 0.8f, 0, 1) : new Color(0.8f, 0, 0, 1);
            }
        }
        
        // GUI properties
        private Rect windowRect;
        private float scaleFactor;
        private GUIStyle windowStyle;
        private GUIStyle labelStyle;
        private GUIStyle buttonStyle;
        private GUIStyle gameButtonStyle;
        private GUIStyle scrollViewStyle;
        private GUIStyle headerStyle;
        private bool stylesInitialized = false;
        
        // Scrolling
        private Vector2 scrollPosition = Vector2.zero;
        private Vector2 historyScrollPosition = Vector2.zero;
        
        // Casino data
        private List<CasinoGame> casinoGames;
        private List<BetHistory> betHistory;
        private CasinoGame selectedGame;
        
        // Betting
        private float currentBet = 10f;
        private float totalWinnings = 0f;
        private float totalLosses = 0f;
        
        // Betting tracking and title system
        private int totalBetsPlaced = 0;
        private bool hasDegenerateGamblerTitle = false;
        private float luckBonus = 0f; // Additional luck percentage
        
        // Property to access centralized money system
        private long PlayerCash => RestaurantManager.Instance.GetMoney();
        private string FormattedCash => RestaurantManager.Instance.GetFormattedMoney(PlayerCash);
        
        // Game state
        private enum CasinoView { Games, CurrentGame, History }
        private CasinoView currentView = CasinoView.Games;
        
        // Current game state
        private bool isGameInProgress = false;
        private string gameResult = "";
        private float lastGameTime = 0f;
        
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
            GenerateCasinoGames();
            betHistory = new List<BetHistory>();
        }
        
        private void SetupStyles()
        {
            // Window style
            windowStyle = new GUIStyle(GUI.skin.box);
            windowStyle.normal.background = MakeTexture(new Color(0.0f, 0.1f, 0.0f, 0.9f)); // Dark green casino theme
            
            // Label style
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = Mathf.RoundToInt(11 * scaleFactor);
            labelStyle.normal.textColor = Color.white;
            
            // Button style
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.normal.background = MakeTexture(new Color(0.2f, 0.4f, 0.2f, 0.8f));
            buttonStyle.hover.background = MakeTexture(new Color(0.3f, 0.6f, 0.3f, 0.9f));
            
            // Game button style (larger)
            gameButtonStyle = new GUIStyle(buttonStyle);
            gameButtonStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            gameButtonStyle.fontStyle = FontStyle.Bold;
            gameButtonStyle.normal.background = MakeTexture(new Color(0.1f, 0.3f, 0.1f, 0.8f));
            gameButtonStyle.hover.background = MakeTexture(new Color(0.2f, 0.5f, 0.2f, 0.9f));
            
            // Header style
            headerStyle = new GUIStyle(labelStyle);
            headerStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = new Color(1f, 0.8f, 0f, 1f); // Gold color
            
            // Scroll view style
            scrollViewStyle = new GUIStyle(GUI.skin.scrollView);
            scrollViewStyle.normal.background = MakeTexture(new Color(0.05f, 0.1f, 0.05f, 0.8f));
        }
        
        private void GenerateCasinoGames()
        {
            casinoGames = new List<CasinoGame>
            {
                new CasinoGame 
                { 
                    name = "Blackjack", 
                    description = "Classic 21 card game", 
                    minBet = 5f, 
                    maxBet = 1000f, 
                    houseEdge = 0.5f, 
                    isActive = true, 
                    playersOnline = UnityEngine.Random.Range(50, 200) 
                },
                new CasinoGame 
                { 
                    name = "Roulette", 
                    description = "Spin the wheel of fortune", 
                    minBet = 1f, 
                    maxBet = 5000f, 
                    houseEdge = 2.7f, 
                    isActive = true, 
                    playersOnline = UnityEngine.Random.Range(80, 300) 
                },
                new CasinoGame 
                { 
                    name = "Poker", 
                    description = "Texas Hold'em tournaments", 
                    minBet = 10f, 
                    maxBet = 10000f, 
                    houseEdge = 1.0f, 
                    isActive = true, 
                    playersOnline = UnityEngine.Random.Range(30, 150) 
                },
                new CasinoGame 
                { 
                    name = "Slots", 
                    description = "Lucky 7's jackpot machine", 
                    minBet = 0.25f, 
                    maxBet = 100f, 
                    houseEdge = 3.5f, 
                    isActive = true, 
                    playersOnline = UnityEngine.Random.Range(200, 500) 
                },
                new CasinoGame 
                { 
                    name = "Baccarat", 
                    description = "High roller card game", 
                    minBet = 25f, 
                    maxBet = 25000f, 
                    houseEdge = 1.2f, 
                    isActive = true, 
                    playersOnline = UnityEngine.Random.Range(20, 80) 
                },
                new CasinoGame 
                { 
                    name = "Craps", 
                    description = "Roll the dice for big wins", 
                    minBet = 5f, 
                    maxBet = 2000f, 
                    houseEdge = 1.4f, 
                    isActive = true, 
                    playersOnline = UnityEngine.Random.Range(40, 120) 
                },
                new CasinoGame 
                { 
                    name = "Sports Betting", 
                    description = "Bet on live sports events", 
                    minBet = 10f, 
                    maxBet = 50000f, 
                    houseEdge = 4.5f, 
                    isActive = true, 
                    playersOnline = UnityEngine.Random.Range(100, 400) 
                },
                new CasinoGame 
                { 
                    name = "Lottery", 
                    description = "Weekly mega jackpot draw", 
                    minBet = 2f, 
                    maxBet = 100f, 
                    houseEdge = 50f, 
                    isActive = true, 
                    playersOnline = UnityEngine.Random.Range(1000, 5000) 
                }
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
            
            // Update player count simulation
            UpdatePlayerCounts();
            
            // Draw window background
            GUI.Box(windowRect, "", windowStyle);
            
            // Draw header with navigation
            DrawHeader();
            
            // Draw content based on current view - adjusted for taller header
            Rect contentRect = new Rect(
                windowRect.x + 10 * scaleFactor,
                windowRect.y + 110 * scaleFactor,
                windowRect.width - 20 * scaleFactor,
                windowRect.height - 120 * scaleFactor
            );
            
            switch (currentView)
            {
                case CasinoView.Games:
                    DrawGamesView(contentRect);
                    break;
                case CasinoView.CurrentGame:
                    DrawCurrentGameView(contentRect);
                    break;
                case CasinoView.History:
                    DrawHistoryView(contentRect);
                    break;
            }
        }
        
        private void DrawHeader()
        {
            // Casino title and stats - made taller for bigger text and moved buttons
            Rect headerRect = new Rect(windowRect.x + 10 * scaleFactor, windowRect.y + 10 * scaleFactor, 
                                      windowRect.width - 20 * scaleFactor, 100 * scaleFactor);
            
            GUI.Label(new Rect(headerRect.x, headerRect.y, headerRect.width * 0.4f, 30 * scaleFactor), 
                     "🎰 VIRTUAL CASINO", headerStyle);
            
            // Create bigger money text style
            GUIStyle moneyStyle = headerStyle != null ? new GUIStyle(headerStyle) : new GUIStyle();
            moneyStyle.fontSize = Mathf.RoundToInt(18 * scaleFactor); // Bigger than regular text
            moneyStyle.normal.textColor = new Color(0f, 1f, 0f, 1f); // Bright green for money
            
            GUI.Label(new Rect(headerRect.x, headerRect.y + 30 * scaleFactor, headerRect.width * 0.6f, 25 * scaleFactor), 
                     $"Cash: {FormattedCash}", moneyStyle);
            
            // Player stats and title
            if (hasDegenerateGamblerTitle)
            {
                GUIStyle titleStyle = headerStyle != null ? new GUIStyle(headerStyle) : new GUIStyle();
                titleStyle.normal.textColor = Color.yellow;
                titleStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor); // Bigger title text
                titleStyle.fontStyle = FontStyle.Bold;
                
                GUI.Label(new Rect(headerRect.x, headerRect.y + 80 * scaleFactor, headerRect.width * 0.7f, 25 * scaleFactor), 
                         "🏆 DEGENERATE GAMBLER 🍀 +25% LUCK", titleStyle);
            }
            
            // Create bigger details text style
            GUIStyle detailsStyle = labelStyle != null ? new GUIStyle(labelStyle) : new GUIStyle();
            detailsStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor); // Bigger than regular text
            detailsStyle.fontStyle = FontStyle.Bold;
            
            GUI.Label(new Rect(headerRect.x, headerRect.y + 60 * scaleFactor, headerRect.width * 0.4f, 20 * scaleFactor), 
                     $"Total Bets: {totalBetsPlaced} / 100", detailsStyle);
            
            // Navigation buttons - moved down lower in the header
            float buttonWidth = headerRect.width * 0.15f;
            float buttonHeight = 30 * scaleFactor; // Slightly taller buttons
            float buttonX = headerRect.x + headerRect.width - buttonWidth * 2 - 10 * scaleFactor;
            
            if (GUI.Button(new Rect(buttonX, headerRect.y + 65 * scaleFactor, buttonWidth, buttonHeight), "GAMES", buttonStyle))
            {
                currentView = CasinoView.Games;
                scrollPosition = Vector2.zero;
            }
            
            buttonX += buttonWidth + 5 * scaleFactor;
            if (GUI.Button(new Rect(buttonX, headerRect.y + 65 * scaleFactor, buttonWidth, buttonHeight), "HISTORY", buttonStyle))
            {
                currentView = CasinoView.History;
                historyScrollPosition = Vector2.zero;
            }
            
            // Winnings/Losses stats with bigger text
            GUIStyle statsStyle = detailsStyle != null ? new GUIStyle(detailsStyle) : new GUIStyle();
            statsStyle.normal.textColor = new Color(1f, 1f, 0.7f, 1f); // Light yellow for stats
            
            GUI.Label(new Rect(headerRect.x + headerRect.width * 0.5f, headerRect.y + 10 * scaleFactor, 
                              headerRect.width * 0.4f, 22 * scaleFactor), 
                     $"Total Won: ${totalWinnings:F2}", statsStyle);
            GUI.Label(new Rect(headerRect.x + headerRect.width * 0.5f, headerRect.y + 35 * scaleFactor, 
                              headerRect.width * 0.4f, 22 * scaleFactor), 
                     $"Total Lost: ${totalLosses:F2}", statsStyle);
            
            // Show luck bonus if active
            if (hasDegenerateGamblerTitle)
            {
                GUIStyle luckStyle = detailsStyle != null ? new GUIStyle(detailsStyle) : new GUIStyle();
                luckStyle.normal.textColor = Color.green;
                luckStyle.fontStyle = FontStyle.Bold;
                
                GUI.Label(new Rect(headerRect.x + headerRect.width * 0.5f, headerRect.y + 60 * scaleFactor, 
                                  headerRect.width * 0.4f, 22 * scaleFactor), 
                         $"Win Chance: +{luckBonus * 100:F0}%", luckStyle);
            }
        }
        
        private void DrawGamesView(Rect contentRect)
        {
            scrollPosition = GUI.BeginScrollView(contentRect, scrollPosition, 
                new Rect(0, 0, contentRect.width - 20 * scaleFactor, casinoGames.Count * 100 * scaleFactor), 
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            
            float yPos = 10 * scaleFactor;
            
            foreach (var game in casinoGames)
            {
                Rect gameRect = new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 90 * scaleFactor);
                GUI.Box(gameRect, "", windowStyle);
                
                // Game name and description
                GUI.Label(new Rect(gameRect.x + 10 * scaleFactor, gameRect.y + 5 * scaleFactor, 
                                  gameRect.width * 0.5f, 25 * scaleFactor), 
                         game.name, headerStyle);
                
                GUI.Label(new Rect(gameRect.x + 10 * scaleFactor, gameRect.y + 25 * scaleFactor, 
                                  gameRect.width * 0.5f, 20 * scaleFactor), 
                         game.description, labelStyle);
                
                // Game stats
                GUI.Label(new Rect(gameRect.x + 10 * scaleFactor, gameRect.y + 45 * scaleFactor, 
                                  gameRect.width * 0.3f, 20 * scaleFactor), 
                         $"Min Bet: ${game.minBet:F2}", labelStyle);
                
                GUI.Label(new Rect(gameRect.x + gameRect.width * 0.3f, gameRect.y + 45 * scaleFactor, 
                                  gameRect.width * 0.3f, 20 * scaleFactor), 
                         $"Max Bet: ${game.maxBet:F2}", labelStyle);
                
                GUI.Label(new Rect(gameRect.x + 10 * scaleFactor, gameRect.y + 65 * scaleFactor, 
                                  gameRect.width * 0.4f, 20 * scaleFactor), 
                         $"Players Online: {game.playersOnline}", labelStyle);
                
                // Play button
                float playButtonWidth = 80 * scaleFactor;
                float playButtonHeight = 40 * scaleFactor;
                
                if (GUI.Button(new Rect(gameRect.x + gameRect.width - playButtonWidth - 10 * scaleFactor, 
                                       gameRect.y + 25 * scaleFactor, playButtonWidth, playButtonHeight), 
                              "PLAY", gameButtonStyle))
                {
                    selectedGame = game;
                    currentView = CasinoView.CurrentGame;
                    currentBet = Mathf.Max(game.minBet, 10f);
                    gameResult = "";
                    isGameInProgress = false;
                }
                
                yPos += 100 * scaleFactor;
            }
            
            GUI.EndScrollView();
        }
        
        private void DrawCurrentGameView(Rect contentRect)
        {
            if (selectedGame == null) return;
            
            // Game header
            GUI.Label(new Rect(contentRect.x + 10 * scaleFactor, contentRect.y + 10 * scaleFactor, 
                              contentRect.width - 20 * scaleFactor, 30 * scaleFactor), 
                     $"Playing: {selectedGame.name}", headerStyle);
            
            // Back button
            if (GUI.Button(new Rect(contentRect.x + contentRect.width - 80 * scaleFactor, contentRect.y + 10 * scaleFactor, 
                                   70 * scaleFactor, 25 * scaleFactor), "BACK", buttonStyle))
            {
                currentView = CasinoView.Games;
                selectedGame = null;
            }
            
            // Betting interface - made taller for better text visibility
            Rect bettingRect = new Rect(contentRect.x + 10 * scaleFactor, contentRect.y + 50 * scaleFactor, 
                                       contentRect.width - 20 * scaleFactor, 250 * scaleFactor);
            GUI.Box(bettingRect, "", windowStyle);
            
            // Bet amount controls
            GUI.Label(new Rect(bettingRect.x + 10 * scaleFactor, bettingRect.y + 10 * scaleFactor, 
                              150 * scaleFactor, 20 * scaleFactor), 
                     $"Bet Amount: ${currentBet:F2}", labelStyle);
            
            // Bet adjustment buttons
            float buttonWidth = 50 * scaleFactor;
            float buttonHeight = 25 * scaleFactor;
            float buttonY = bettingRect.y + 35 * scaleFactor;
            
            if (GUI.Button(new Rect(bettingRect.x + 10 * scaleFactor, buttonY, buttonWidth, buttonHeight), "-1", buttonStyle))
            {
                currentBet = Mathf.Max(selectedGame.minBet, currentBet - 1f);
            }
            
            if (GUI.Button(new Rect(bettingRect.x + 70 * scaleFactor, buttonY, buttonWidth, buttonHeight), "-10", buttonStyle))
            {
                currentBet = Mathf.Max(selectedGame.minBet, currentBet - 10f);
            }
            
            if (GUI.Button(new Rect(bettingRect.x + 130 * scaleFactor, buttonY, buttonWidth, buttonHeight), "+10", buttonStyle))
            {
                currentBet = Mathf.Min(selectedGame.maxBet, currentBet + 10f);
            }
            
            if (GUI.Button(new Rect(bettingRect.x + 190 * scaleFactor, buttonY, buttonWidth, buttonHeight), "+100", buttonStyle))
            {
                currentBet = Mathf.Min(selectedGame.maxBet, currentBet + 100f);
            }
            
            if (GUI.Button(new Rect(bettingRect.x + 250 * scaleFactor, buttonY, buttonWidth, buttonHeight), "MAX", buttonStyle))
            {
                currentBet = Mathf.Min(selectedGame.maxBet, PlayerCash);
            }
            
            // Place bet button
            GUI.enabled = !isGameInProgress && PlayerCash >= currentBet;
            if (GUI.Button(new Rect(bettingRect.x + 10 * scaleFactor, bettingRect.y + 70 * scaleFactor, 
                                   150 * scaleFactor, 40 * scaleFactor), 
                          isGameInProgress ? "PLAYING..." : "PLACE BET", gameButtonStyle))
            {
                PlaceBet();
            }
            GUI.enabled = true;
            
            // Game result - positioned lower for better readability
            if (!string.IsNullOrEmpty(gameResult))
            {
                GUIStyle resultStyle = headerStyle != null ? new GUIStyle(headerStyle) : new GUIStyle();
                resultStyle.normal.textColor = gameResult.Contains("WON") ? Color.green : Color.red;
                resultStyle.alignment = TextAnchor.MiddleCenter; // Center the text for better visibility
                
                GUI.Label(new Rect(bettingRect.x + 10 * scaleFactor, bettingRect.y + 140 * scaleFactor, 
                                  bettingRect.width - 20 * scaleFactor, 40 * scaleFactor), 
                         gameResult, resultStyle);
            }
            
            // Game simulation display - adjusted position for larger betting box
            DrawGameSimulation(new Rect(contentRect.x + 10 * scaleFactor, contentRect.y + 310 * scaleFactor, 
                                       contentRect.width - 20 * scaleFactor, contentRect.height - 320 * scaleFactor));
        }
        
        private void DrawGameSimulation(Rect gameRect)
        {
            GUI.Box(gameRect, "", windowStyle);
            
            if (selectedGame == null) return;
            
            // Simple game visualization based on game type
            switch (selectedGame.name)
            {
                case "Slots":
                    DrawSlotMachine(gameRect);
                    break;
                case "Roulette":
                    DrawRouletteWheel(gameRect);
                    break;
                case "Blackjack":
                    DrawBlackjackTable(gameRect);
                    break;
                default:
                    GUI.Label(new Rect(gameRect.x + 10 * scaleFactor, gameRect.y + 10 * scaleFactor, 
                                      gameRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                             $"Playing {selectedGame.name}...", labelStyle);
                    break;
            }
        }
        
        private void DrawSlotMachine(Rect rect)
        {
            GUI.Label(new Rect(rect.x + 10 * scaleFactor, rect.y + 10 * scaleFactor, 
                              rect.width - 20 * scaleFactor, 30 * scaleFactor), 
                     "🎰 SLOT MACHINE 🎰", headerStyle);
            
            // Draw slot reels
            string[] symbols = { "🍒", "🍋", "🔔", "⭐", "💎", "7️⃣" };
            for (int i = 0; i < 3; i++)
            {
                int symbolIndex = (int)(Time.time * 10 + i) % symbols.Length;
                GUI.Label(new Rect(rect.x + 50 * scaleFactor + i * 80 * scaleFactor, rect.y + 50 * scaleFactor, 
                                  60 * scaleFactor, 40 * scaleFactor), 
                         symbols[symbolIndex], headerStyle);
            }
        }
        
        private void DrawRouletteWheel(Rect rect)
        {
            GUI.Label(new Rect(rect.x + 10 * scaleFactor, rect.y + 10 * scaleFactor, 
                              rect.width - 20 * scaleFactor, 30 * scaleFactor), 
                     "🎲 ROULETTE WHEEL 🎲", headerStyle);
            
            int number = (int)(Time.time * 5) % 37;
            Color numberColor = (number % 2 == 0) ? Color.red : Color.black;
            if (number == 0) numberColor = Color.green;
            
            GUIStyle numberStyle = headerStyle != null ? new GUIStyle(headerStyle) : new GUIStyle();
            numberStyle.normal.textColor = numberColor;
            
            GUI.Label(new Rect(rect.x + rect.width * 0.5f - 30 * scaleFactor, rect.y + 50 * scaleFactor, 
                              60 * scaleFactor, 40 * scaleFactor), 
                     number.ToString(), numberStyle);
        }
        
        private void DrawBlackjackTable(Rect rect)
        {
            GUI.Label(new Rect(rect.x + 10 * scaleFactor, rect.y + 10 * scaleFactor, 
                              rect.width - 20 * scaleFactor, 30 * scaleFactor), 
                     "♠️ BLACKJACK TABLE ♥️", headerStyle);
            
            string[] cards = { "A♠", "K♥", "Q♦", "J♣", "10♠", "9♥", "8♦", "7♣" };
            for (int i = 0; i < 2; i++)
            {
                int cardIndex = (int)(Time.time * 3 + i) % cards.Length;
                GUI.Label(new Rect(rect.x + 50 * scaleFactor + i * 60 * scaleFactor, rect.y + 50 * scaleFactor, 
                                  50 * scaleFactor, 30 * scaleFactor), 
                         cards[cardIndex], labelStyle);
            }
        }
        
        private void DrawHistoryView(Rect contentRect)
        {
            historyScrollPosition = GUI.BeginScrollView(contentRect, historyScrollPosition, 
                new Rect(0, 0, contentRect.width - 20 * scaleFactor, betHistory.Count * 60 * scaleFactor), 
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            
            float yPos = 10 * scaleFactor;
            
            if (betHistory.Count == 0)
            {
                GUI.Label(new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 30 * scaleFactor), 
                         "No betting history yet. Start playing to see your results here!", labelStyle);
            }
            else
            {
                foreach (var bet in betHistory)
                {
                    Rect betRect = new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 50 * scaleFactor);
                    GUI.Box(betRect, "", windowStyle);
                    
                    GUI.Label(new Rect(betRect.x + 10 * scaleFactor, betRect.y + 5 * scaleFactor, 
                                      betRect.width * 0.3f, 20 * scaleFactor), 
                             bet.gameName, labelStyle);
                    
                    GUI.Label(new Rect(betRect.x + 10 * scaleFactor, betRect.y + 25 * scaleFactor, 
                                      betRect.width * 0.3f, 20 * scaleFactor), 
                             bet.timeStamp.ToString("MM/dd HH:mm"), labelStyle);
                    
                    GUIStyle resultStyle = labelStyle != null ? new GUIStyle(labelStyle) : new GUIStyle();
                    resultStyle.normal.textColor = bet.GetResultColor();
                    
                    GUI.Label(new Rect(betRect.x + betRect.width * 0.4f, betRect.y + 15 * scaleFactor, 
                                      betRect.width * 0.3f, 20 * scaleFactor), 
                             bet.GetResultText(), resultStyle);
                    
                    yPos += 60 * scaleFactor;
                }
            }
            
            GUI.EndScrollView();
        }
        
        private void PlaceBet()
        {
            if (PlayerCash < currentBet) return;
            
            // Deduct bet amount immediately when placing bet
            RestaurantManager.Instance.AdjustMoney(-(int)currentBet);
            
            isGameInProgress = true;
            lastGameTime = Time.time;
            
            // Simulate game result after a delay
            Invoke(nameof(ProcessGameResult), UnityEngine.Random.Range(2f, 5f));
        }
        
        private void ProcessGameResult()
        {
            if (selectedGame == null) return;
            
            // Track betting for title system
            totalBetsPlaced++;
            CheckForDegenerateGamblerTitle();
            
            // Calculate REALISTIC win probability based on house edge
            // House edge represents casino advantage, so player win rate should be roughly 50% minus house edge
            float baseWinProbability = GetBaseWinProbability(selectedGame.name);
            float finalWinProbability = baseWinProbability + luckBonus;
            
            bool isWin = UnityEngine.Random.Range(0f, 1f) < finalWinProbability;
            
            BetHistory newBet = new BetHistory
            {
                gameName = selectedGame.name,
                betAmount = currentBet,
                timeStamp = DateTime.Now,
                isWin = isWin
            };
            
            if (isWin)
            {
                // Calculate winnings based on game type
                float multiplier = GetWinMultiplier(selectedGame.name);
                newBet.winAmount = currentBet * multiplier;
                RestaurantManager.Instance.AdjustMoney((int)newBet.winAmount);
                totalWinnings += newBet.winAmount;
                
                string titleBonus = hasDegenerateGamblerTitle ? " 🍀 LUCK BONUS!" : "";
                gameResult = $"🎉 YOU WON ${newBet.winAmount:F2}!{titleBonus} 🎉";
                
                // Auto bank in winnings
                AutoBankWinnings(newBet.winAmount);
            }
            else
            {
                newBet.winAmount = 0f;
                // No need to deduct money here - already deducted when bet was placed
                totalLosses += currentBet;
                gameResult = $"😞 You lost ${currentBet:F2}. Better luck next time!";
            }
            
            betHistory.Insert(0, newBet); // Add to beginning of list
            isGameInProgress = false;
        }
        
        private float GetBaseWinProbability(string gameName)
        {
            // Consistent 9:20 ratio (45% win rate) for balanced gameplay
            return 0.45f; // 9 wins out of 20 attempts for all games
        }
        
        private float GetWinMultiplier(string gameName)
        {
            switch (gameName)
            {
                case "Slots": return UnityEngine.Random.Range(2f, 100f); // Higher since lower win rate
                case "Roulette": return UnityEngine.Random.Range(2f, 36f); // Varies by bet type
                case "Blackjack": return 2f; // Standard 2:1
                case "Poker": return UnityEngine.Random.Range(2f, 10f);
                case "Baccarat": return 1.95f;
                case "Craps": return UnityEngine.Random.Range(2f, 30f); // Varies by bet
                case "Sports Betting": return UnityEngine.Random.Range(1.5f, 8f);
                case "Lottery": return UnityEngine.Random.Range(50f, 10000f); // Massive payouts
                default: return 2f;
            }
        }
        
        private void CheckForDegenerateGamblerTitle()
        {
            if (!hasDegenerateGamblerTitle && totalBetsPlaced >= 100)
            {
                hasDegenerateGamblerTitle = true;
                luckBonus = 0.25f; // 25% luck bonus
                
                gameResult = "🎰🏆 TITLE UNLOCKED: DEGENERATE GAMBLER! 🏆🎰\n" +
                           "🍀 +25% LUCK BONUS APPLIED! 🍀\n" +
                           $"You've placed {totalBetsPlaced} bets!";
                
                Debug.Log($"Player earned Degenerate Gambler title after {totalBetsPlaced} bets!");
            }
        }
        
        private void AutoBankWinnings(float amount)
        {
            // This would integrate with your banking system
            Debug.Log($"Auto-deposited ${amount:F2} to bank account");
        }
        
        private void UpdatePlayerCounts()
        {
            // Simulate fluctuating player counts
            if (Time.time % 10f < 0.1f) // Update every 10 seconds
            {
                foreach (var game in casinoGames)
                {
                    int variance = UnityEngine.Random.Range(-20, 21);
                    game.playersOnline = Mathf.Max(1, game.playersOnline + variance);
                }
            }
        }
    }
}