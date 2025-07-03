using UnityEngine;
using System.Collections.Generic;
using System;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Dealer Window with Dealer, Activity, Invest, and Legal tabs
    /// Simulates drug dealing business management
    /// </summary>
    public class DealerWindow : MonoBehaviour
    {
        [System.Serializable]
        public class Dealer
        {
            public string name;
            public string location;
            public float reputation;
            public float trustLevel;
            public List<Drug> drugsAvailable;
            public bool isActive;
            public float commission;
            public DateTime lastContact;
            
            public Dealer()
            {
                drugsAvailable = new List<Drug>();
            }
        }
        
        [System.Serializable]
        public class Drug
        {
            public string name;
            public string type;
            public float pricePerUnit;
            public int unitsAvailable;
            public float purity;
            public float riskLevel;
            public string description;
            
            public Color GetRiskColor()
            {
                if (riskLevel < 30f) return new Color(0, 0.8f, 0, 1); // Green - Low risk
                if (riskLevel < 70f) return new Color(1f, 0.8f, 0, 1); // Yellow - Medium risk
                return new Color(0.8f, 0, 0, 1); // Red - High risk
            }
        }
        
        [System.Serializable]
        public class ActivityReport
        {
            public DateTime date;
            public string dealerName;
            public string activity;
            public float revenue;
            public float expenses;
            public float profit;
            public float riskFactor;
            public string status;
        }
        
        [System.Serializable]
        public class Investment
        {
            public string name;
            public string description;
            public float costToInvest;
            public float expectedReturn;
            public float timeToMaturity;
            public float riskLevel;
            public bool isActive;
            public DateTime startDate;
        }
        
        [System.Serializable]
        public class LegalService
        {
            public string serviceName;
            public string description;
            public float cost;
            public float successRate;
            public bool isAvailable;
            public DateTime lastUsed;
        }
        
        // Tab system
        private enum DealerTab { Dealer, Activity, Invest, Legal }
        private DealerTab currentTab = DealerTab.Dealer;
        
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
        private GUIStyle warningStyle;
        
        // Scrolling
        private Vector2 scrollPosition = Vector2.zero;
        
        // Data
        private List<Dealer> dealers;
        private List<ActivityReport> activityReports;
        private List<Investment> investments;
        private List<LegalService> legalServices;
        
        // Business stats
        private float totalRevenue = 125000f;
        private float totalExpenses = 78000f;
        private float currentCash = 45000f;
        private float heatLevel = 35f; // Police attention level
        
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
            GenerateDealers();
            GenerateActivityReports();
            GenerateInvestments();
            GenerateLegalServices();
        }
        
        private void SetupStyles()
        {
            // Tab style
            tabStyle = new GUIStyle(GUI.skin.button);
            tabStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            tabStyle.fontStyle = FontStyle.Bold;
            tabStyle.normal.textColor = Color.white;
            tabStyle.normal.background = MakeTexture(new Color(0.3f, 0.1f, 0.1f, 0.8f)); // Dark red theme
            
            // Active tab style
            activeTabStyle = new GUIStyle(tabStyle);
            activeTabStyle.normal.background = MakeTexture(new Color(0.6f, 0.2f, 0.2f, 0.9f));
            
            // Window style
            windowStyle = new GUIStyle(GUI.skin.box);
            windowStyle.normal.background = MakeTexture(new Color(0.1f, 0.05f, 0.05f, 0.9f));
            
            // Label style
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = Mathf.RoundToInt(11 * scaleFactor);
            labelStyle.normal.textColor = Color.white;
            
            // Button style
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = Mathf.RoundToInt(10 * scaleFactor);
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.normal.background = MakeTexture(new Color(0.4f, 0.2f, 0.2f, 0.8f));
            buttonStyle.hover.background = MakeTexture(new Color(0.6f, 0.3f, 0.3f, 0.9f));
            
            // Header style
            headerStyle = new GUIStyle(labelStyle);
            headerStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = new Color(1f, 0.7f, 0.7f, 1f);
            
            // Warning style
            warningStyle = new GUIStyle(labelStyle);
            warningStyle.normal.textColor = new Color(1f, 0.3f, 0.3f, 1f);
            warningStyle.fontStyle = FontStyle.Bold;
            
            // Scroll view style
            scrollViewStyle = new GUIStyle(GUI.skin.scrollView);
            scrollViewStyle.normal.background = MakeTexture(new Color(0.05f, 0.02f, 0.02f, 0.8f));
        }
        
        private void GenerateDealers()
        {
            dealers = new List<Dealer>();
            
            string[] dealerNames = { "Snake", "Viper", "Ghost", "Shadow", "Razor", "Bullet", "Ice", "Phoenix" };
            string[] locations = { "Downtown", "East Side", "West End", "North District", "South Bay", "Industrial", "Harbor", "Uptown" };
            
            for (int i = 0; i < 6; i++)
            {
                Dealer dealer = new Dealer
                {
                    name = dealerNames[i],
                    location = locations[i],
                    reputation = UnityEngine.Random.Range(60f, 95f),
                    trustLevel = UnityEngine.Random.Range(70f, 90f),
                    isActive = UnityEngine.Random.Range(0f, 1f) > 0.2f,
                    commission = UnityEngine.Random.Range(10f, 25f),
                    lastContact = DateTime.Now.AddDays(-UnityEngine.Random.Range(1, 7))
                };
                
                // Generate drugs for each dealer
                GenerateDrugsForDealer(dealer);
                dealers.Add(dealer);
            }
        }
        
        private void GenerateDrugsForDealer(Dealer dealer)
        {
            string[] drugNames = { "White Powder", "Green Leaf", "Blue Crystal", "Red Pills", "Yellow Rocks", "Purple Haze", "Black Tar", "Clear Ice" };
            string[] drugTypes = { "Stimulant", "Depressant", "Hallucinogen", "Synthetic", "Natural", "Prescription", "Designer", "Traditional" };
            string[] descriptions = {
                "High quality product from overseas",
                "Locally sourced premium grade",
                "Lab-tested pure compound",
                "Street grade with good reviews",
                "Premium imported stock",
                "Bulk quantity available",
                "Limited time special offer",
                "Exclusive dealer product"
            };
            
            int drugCount = UnityEngine.Random.Range(3, 6);
            for (int i = 0; i < drugCount; i++)
            {
                Drug drug = new Drug
                {
                    name = drugNames[UnityEngine.Random.Range(0, drugNames.Length)],
                    type = drugTypes[UnityEngine.Random.Range(0, drugTypes.Length)],
                    pricePerUnit = UnityEngine.Random.Range(50f, 500f),
                    unitsAvailable = UnityEngine.Random.Range(10, 100),
                    purity = UnityEngine.Random.Range(70f, 98f),
                    riskLevel = UnityEngine.Random.Range(20f, 90f),
                    description = descriptions[UnityEngine.Random.Range(0, descriptions.Length)]
                };
                
                dealer.drugsAvailable.Add(drug);
            }
        }
        
        private void GenerateActivityReports()
        {
            activityReports = new List<ActivityReport>();
            
            string[] activities = {
                "Street sales operation",
                "Wholesale distribution",
                "Territory expansion",
                "Product quality testing",
                "Customer recruitment",
                "Supplier meeting",
                "Security enhancement",
                "Competition monitoring"
            };
            
            string[] statuses = { "Successful", "Completed", "Ongoing", "Under Investigation", "Suspended", "High Risk" };
            
            for (int i = 0; i < 15; i++)
            {
                ActivityReport report = new ActivityReport
                {
                    date = DateTime.Now.AddDays(-i),
                    dealerName = dealers[UnityEngine.Random.Range(0, dealers.Count)].name,
                    activity = activities[UnityEngine.Random.Range(0, activities.Length)],
                    revenue = UnityEngine.Random.Range(5000f, 25000f),
                    expenses = UnityEngine.Random.Range(2000f, 12000f),
                    riskFactor = UnityEngine.Random.Range(10f, 80f),
                    status = statuses[UnityEngine.Random.Range(0, statuses.Length)]
                };
                
                report.profit = report.revenue - report.expenses;
                activityReports.Add(report);
            }
        }
        
        private void GenerateInvestments()
        {
            investments = new List<Investment>
            {
                new Investment 
                { 
                    name = "Lab Equipment Upgrade", 
                    description = "Improve product quality and reduce detection risk", 
                    costToInvest = 50000f, 
                    expectedReturn = 15f, 
                    timeToMaturity = 6f, 
                    riskLevel = 25f, 
                    isActive = false 
                },
                new Investment 
                { 
                    name = "Territory Expansion", 
                    description = "Expand operations to new districts", 
                    costToInvest = 75000f, 
                    expectedReturn = 25f, 
                    timeToMaturity = 12f, 
                    riskLevel = 60f, 
                    isActive = false 
                },
                new Investment 
                { 
                    name = "Security Network", 
                    description = "Enhanced surveillance and protection", 
                    costToInvest = 30000f, 
                    expectedReturn = 10f, 
                    timeToMaturity = 3f, 
                    riskLevel = 15f, 
                    isActive = true,
                    startDate = DateTime.Now.AddDays(-30)
                },
                new Investment 
                { 
                    name = "Transport Fleet", 
                    description = "Secure vehicle network for distribution", 
                    costToInvest = 100000f, 
                    expectedReturn = 20f, 
                    timeToMaturity = 18f, 
                    riskLevel = 40f, 
                    isActive = false 
                },
                new Investment 
                { 
                    name = "Money Laundering", 
                    description = "Legal business fronts for cash cleaning", 
                    costToInvest = 150000f, 
                    expectedReturn = 12f, 
                    timeToMaturity = 24f, 
                    riskLevel = 30f, 
                    isActive = false 
                }
            };
        }
        
        private void GenerateLegalServices()
        {
            legalServices = new List<LegalService>
            {
                new LegalService 
                { 
                    serviceName = "Criminal Defense Lawyer", 
                    description = "Top-tier legal representation for serious charges", 
                    cost = 50000f, 
                    successRate = 85f, 
                    isAvailable = true,
                    lastUsed = DateTime.Now.AddDays(-120)
                },
                new LegalService 
                { 
                    serviceName = "Police Bribe", 
                    description = "Reduce heat level and investigation pressure", 
                    cost = 25000f, 
                    successRate = 70f, 
                    isAvailable = heatLevel > 40f,
                    lastUsed = DateTime.Now.AddDays(-45)
                },
                new LegalService 
                { 
                    serviceName = "Post Bail", 
                    description = "Immediate release from custody", 
                    cost = 100000f, 
                    successRate = 95f, 
                    isAvailable = true,
                    lastUsed = DateTime.Now.AddDays(-200)
                },
                new LegalService 
                { 
                    serviceName = "Evidence Tampering", 
                    description = "Remove or alter incriminating evidence", 
                    cost = 75000f, 
                    successRate = 60f, 
                    isAvailable = heatLevel > 60f,
                    lastUsed = DateTime.Now.AddDays(-90)
                },
                new LegalService 
                { 
                    serviceName = "Witness Protection", 
                    description = "Ensure witness silence or disappearance", 
                    cost = 40000f, 
                    successRate = 80f, 
                    isAvailable = true,
                    lastUsed = DateTime.Now.AddDays(-60)
                },
                new LegalService 
                { 
                    serviceName = "Judge Influence", 
                    description = "Favorable court decisions and sentencing", 
                    cost = 200000f, 
                    successRate = 75f, 
                    isAvailable = heatLevel > 50f,
                    lastUsed = DateTime.Now.AddDays(-300)
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
            // Update heat level over time
            UpdateHeatLevel();
            
            // Draw window background
            GUI.Box(windowRect, "", windowStyle);
            
            // Draw tabs
            DrawTabs();
            
            // Draw heat level warning
            DrawHeatLevelWarning();
            
            // Draw content based on active tab
            Rect contentRect = new Rect(
                windowRect.x + 10 * scaleFactor,
                windowRect.y + 80 * scaleFactor,
                windowRect.width - 20 * scaleFactor,
                windowRect.height - 90 * scaleFactor
            );
            
            switch (currentTab)
            {
                case DealerTab.Dealer:
                    DrawDealerContent(contentRect);
                    break;
                case DealerTab.Activity:
                    DrawActivityContent(contentRect);
                    break;
                case DealerTab.Invest:
                    DrawInvestContent(contentRect);
                    break;
                case DealerTab.Legal:
                    DrawLegalContent(contentRect);
                    break;
            }
        }
        
        private void DrawTabs()
        {
            float tabWidth = windowRect.width / 4f;
            float tabHeight = 40 * scaleFactor;
            float tabY = windowRect.y + 10 * scaleFactor;
            
            // Dealer tab
            if (GUI.Button(new Rect(windowRect.x, tabY, tabWidth, tabHeight), "DEALER", 
                          currentTab == DealerTab.Dealer ? activeTabStyle : tabStyle))
            {
                currentTab = DealerTab.Dealer;
                scrollPosition = Vector2.zero;
            }
            
            // Activity tab
            if (GUI.Button(new Rect(windowRect.x + tabWidth, tabY, tabWidth, tabHeight), "ACTIVITY", 
                          currentTab == DealerTab.Activity ? activeTabStyle : tabStyle))
            {
                currentTab = DealerTab.Activity;
                scrollPosition = Vector2.zero;
            }
            
            // Invest tab
            if (GUI.Button(new Rect(windowRect.x + tabWidth * 2, tabY, tabWidth, tabHeight), "INVEST", 
                          currentTab == DealerTab.Invest ? activeTabStyle : tabStyle))
            {
                currentTab = DealerTab.Invest;
                scrollPosition = Vector2.zero;
            }
            
            // Legal tab
            if (GUI.Button(new Rect(windowRect.x + tabWidth * 3, tabY, tabWidth, tabHeight), "LEGAL", 
                          currentTab == DealerTab.Legal ? activeTabStyle : tabStyle))
            {
                currentTab = DealerTab.Legal;
                scrollPosition = Vector2.zero;
            }
        }
        
        private void DrawHeatLevelWarning()
        {
            Rect heatRect = new Rect(windowRect.x + 10 * scaleFactor, windowRect.y + 55 * scaleFactor, 
                                    windowRect.width - 20 * scaleFactor, 20 * scaleFactor);
            
            string heatText = $"Heat Level: {heatLevel:F1}% | Cash: ${currentCash:N0}";
            Color heatColor = Color.white;
            
            if (heatLevel > 80f)
            {
                heatText += " ⚠️ EXTREME DANGER ⚠️";
                heatColor = Color.red;
            }
            else if (heatLevel > 60f)
            {
                heatText += " ⚠️ HIGH RISK";
                heatColor = new Color(1f, 0.5f, 0f, 1f);
            }
            else if (heatLevel > 40f)
            {
                heatText += " ⚠️ MEDIUM RISK";
                heatColor = Color.yellow;
            }
            else
            {
                heatText += " ✓ LOW RISK";
                heatColor = Color.green;
            }
            
            GUIStyle heatStyle = new GUIStyle(labelStyle);
            heatStyle.normal.textColor = heatColor;
            
            GUI.Label(heatRect, heatText, heatStyle);
        }
        
        private void DrawDealerContent(Rect contentRect)
        {
            scrollPosition = GUI.BeginScrollView(contentRect, scrollPosition, 
                new Rect(0, 0, contentRect.width - 20 * scaleFactor, dealers.Count * 200 * scaleFactor), 
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            
            float yPos = 10 * scaleFactor;
            
            foreach (var dealer in dealers)
            {
                Rect dealerRect = new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 190 * scaleFactor);
                GUI.Box(dealerRect, "", windowStyle);
                
                // Dealer header
                Color dealerColor = dealer.isActive ? Color.white : Color.gray;
                GUIStyle dealerStyle = new GUIStyle(headerStyle);
                dealerStyle.normal.textColor = dealerColor;
                
                GUI.Label(new Rect(dealerRect.x + 10 * scaleFactor, dealerRect.y + 5 * scaleFactor, 
                                  dealerRect.width * 0.5f, 20 * scaleFactor), 
                         $"{dealer.name} - {dealer.location}", dealerStyle);
                
                // Dealer stats
                GUI.Label(new Rect(dealerRect.x + 10 * scaleFactor, dealerRect.y + 25 * scaleFactor, 
                                  dealerRect.width * 0.3f, 15 * scaleFactor), 
                         $"Rep: {dealer.reputation:F1}%", labelStyle);
                
                GUI.Label(new Rect(dealerRect.x + dealerRect.width * 0.3f, dealerRect.y + 25 * scaleFactor, 
                                  dealerRect.width * 0.3f, 15 * scaleFactor), 
                         $"Trust: {dealer.trustLevel:F1}%", labelStyle);
                
                GUI.Label(new Rect(dealerRect.x + 10 * scaleFactor, dealerRect.y + 40 * scaleFactor, 
                                  dealerRect.width * 0.4f, 15 * scaleFactor), 
                         $"Commission: {dealer.commission:F1}%", labelStyle);
                
                GUI.Label(new Rect(dealerRect.x + dealerRect.width * 0.4f, dealerRect.y + 40 * scaleFactor, 
                                  dealerRect.width * 0.4f, 15 * scaleFactor), 
                         $"Last Contact: {dealer.lastContact:MM/dd}", labelStyle);
                
                // Status and action button
                string statusText = dealer.isActive ? "ACTIVE" : "INACTIVE";
                Color statusColor = dealer.isActive ? Color.green : Color.red;
                
                GUIStyle statusStyle = new GUIStyle(labelStyle);
                statusStyle.normal.textColor = statusColor;
                
                GUI.Label(new Rect(dealerRect.x + dealerRect.width - 100 * scaleFactor, dealerRect.y + 5 * scaleFactor, 
                                  80 * scaleFactor, 20 * scaleFactor), 
                         statusText, statusStyle);
                
                if (GUI.Button(new Rect(dealerRect.x + dealerRect.width - 80 * scaleFactor, dealerRect.y + 25 * scaleFactor, 
                                       70 * scaleFactor, 25 * scaleFactor), "CONTACT", buttonStyle))
                {
                    ContactDealer(dealer);
                }
                
                // Drugs available
                GUI.Label(new Rect(dealerRect.x + 10 * scaleFactor, dealerRect.y + 60 * scaleFactor, 
                                  dealerRect.width - 20 * scaleFactor, 15 * scaleFactor), 
                         "Available Products:", headerStyle);
                
                float drugY = dealerRect.y + 80 * scaleFactor;
                foreach (var drug in dealer.drugsAvailable)
                {
                    // Drug info
                    GUI.Label(new Rect(dealerRect.x + 20 * scaleFactor, drugY, 
                                      dealerRect.width * 0.3f, 15 * scaleFactor), 
                             $"{drug.name} ({drug.type})", labelStyle);
                    
                    GUI.Label(new Rect(dealerRect.x + dealerRect.width * 0.3f, drugY, 
                                      dealerRect.width * 0.2f, 15 * scaleFactor), 
                             $"${drug.pricePerUnit:F0}/unit", labelStyle);
                    
                    GUI.Label(new Rect(dealerRect.x + dealerRect.width * 0.5f, drugY, 
                                      dealerRect.width * 0.15f, 15 * scaleFactor), 
                             $"{drug.unitsAvailable} units", labelStyle);
                    
                    GUI.Label(new Rect(dealerRect.x + dealerRect.width * 0.65f, drugY, 
                                      dealerRect.width * 0.15f, 15 * scaleFactor), 
                             $"{drug.purity:F1}% pure", labelStyle);
                    
                    // Risk level with color
                    GUIStyle riskStyle = new GUIStyle(labelStyle);
                    riskStyle.normal.textColor = drug.GetRiskColor();
                    GUI.Label(new Rect(dealerRect.x + dealerRect.width * 0.8f, drugY, 
                                      dealerRect.width * 0.15f, 15 * scaleFactor), 
                             $"Risk: {drug.riskLevel:F0}%", riskStyle);
                    
                    drugY += 20 * scaleFactor;
                }
                
                yPos += 200 * scaleFactor;
            }
            
            GUI.EndScrollView();
        }
        
        private void DrawActivityContent(Rect contentRect)
        {
            // Daily summary
            GUI.Label(new Rect(contentRect.x + 10 * scaleFactor, contentRect.y + 10 * scaleFactor, 
                              contentRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                     "📊 DAILY BUSINESS REPORTS", headerStyle);
            
            scrollPosition = GUI.BeginScrollView(
                new Rect(contentRect.x, contentRect.y + 40 * scaleFactor, contentRect.width, contentRect.height - 40 * scaleFactor), 
                scrollPosition, 
                new Rect(0, 0, contentRect.width - 20 * scaleFactor, activityReports.Count * 80 * scaleFactor), 
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            
            float yPos = 10 * scaleFactor;
            
            foreach (var report in activityReports)
            {
                Rect reportRect = new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 70 * scaleFactor);
                GUI.Box(reportRect, "", windowStyle);
                
                // Date and dealer
                GUI.Label(new Rect(reportRect.x + 10 * scaleFactor, reportRect.y + 5 * scaleFactor, 
                                  reportRect.width * 0.3f, 15 * scaleFactor), 
                         $"{report.date:MM/dd/yyyy} - {report.dealerName}", headerStyle);
                
                // Activity
                GUI.Label(new Rect(reportRect.x + 10 * scaleFactor, reportRect.y + 20 * scaleFactor, 
                                  reportRect.width * 0.5f, 15 * scaleFactor), 
                         report.activity, labelStyle);
                
                // Financial info
                GUI.Label(new Rect(reportRect.x + 10 * scaleFactor, reportRect.y + 35 * scaleFactor, 
                                  reportRect.width * 0.25f, 15 * scaleFactor), 
                         $"Revenue: ${report.revenue:N0}", labelStyle);
                
                GUI.Label(new Rect(reportRect.x + reportRect.width * 0.25f, reportRect.y + 35 * scaleFactor, 
                                  reportRect.width * 0.25f, 15 * scaleFactor), 
                         $"Expenses: ${report.expenses:N0}", labelStyle);
                
                // Profit with color
                Color profitColor = report.profit >= 0 ? Color.green : Color.red;
                GUIStyle profitStyle = new GUIStyle(labelStyle);
                profitStyle.normal.textColor = profitColor;
                
                GUI.Label(new Rect(reportRect.x + reportRect.width * 0.5f, reportRect.y + 35 * scaleFactor, 
                                  reportRect.width * 0.25f, 15 * scaleFactor), 
                         $"Profit: ${report.profit:N0}", profitStyle);
                
                // Risk and status
                GUIStyle riskStyle = new GUIStyle(labelStyle);
                if (report.riskFactor > 60f) riskStyle.normal.textColor = Color.red;
                else if (report.riskFactor > 30f) riskStyle.normal.textColor = Color.yellow;
                else riskStyle.normal.textColor = Color.green;
                
                GUI.Label(new Rect(reportRect.x + reportRect.width * 0.75f, reportRect.y + 20 * scaleFactor, 
                                  reportRect.width * 0.2f, 15 * scaleFactor), 
                         $"Risk: {report.riskFactor:F0}%", riskStyle);
                
                GUI.Label(new Rect(reportRect.x + reportRect.width * 0.75f, reportRect.y + 35 * scaleFactor, 
                                  reportRect.width * 0.2f, 15 * scaleFactor), 
                         report.status, labelStyle);
                
                yPos += 80 * scaleFactor;
            }
            
            GUI.EndScrollView();
        }
        
        private void DrawInvestContent(Rect contentRect)
        {
            scrollPosition = GUI.BeginScrollView(contentRect, scrollPosition, 
                new Rect(0, 0, contentRect.width - 20 * scaleFactor, investments.Count * 120 * scaleFactor), 
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            
            float yPos = 10 * scaleFactor;
            
            foreach (var investment in investments)
            {
                Rect investRect = new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 110 * scaleFactor);
                GUI.Box(investRect, "", windowStyle);
                
                // Investment name and status
                Color statusColor = investment.isActive ? Color.green : Color.white;
                GUIStyle investStyle = new GUIStyle(headerStyle);
                investStyle.normal.textColor = statusColor;
                
                GUI.Label(new Rect(investRect.x + 10 * scaleFactor, investRect.y + 5 * scaleFactor, 
                                  investRect.width * 0.6f, 20 * scaleFactor), 
                         investment.name, investStyle);
                
                string statusText = investment.isActive ? "ACTIVE" : "AVAILABLE";
                GUI.Label(new Rect(investRect.x + investRect.width * 0.7f, investRect.y + 5 * scaleFactor, 
                                  investRect.width * 0.25f, 20 * scaleFactor), 
                         statusText, investStyle);
                
                // Description
                GUI.Label(new Rect(investRect.x + 10 * scaleFactor, investRect.y + 25 * scaleFactor, 
                                  investRect.width - 20 * scaleFactor, 15 * scaleFactor), 
                         investment.description, labelStyle);
                
                // Investment details
                GUI.Label(new Rect(investRect.x + 10 * scaleFactor, investRect.y + 45 * scaleFactor, 
                                  investRect.width * 0.25f, 15 * scaleFactor), 
                         $"Cost: ${investment.costToInvest:N0}", labelStyle);
                
                GUI.Label(new Rect(investRect.x + investRect.width * 0.25f, investRect.y + 45 * scaleFactor, 
                                  investRect.width * 0.25f, 15 * scaleFactor), 
                         $"Return: {investment.expectedReturn:F1}%", labelStyle);
                
                GUI.Label(new Rect(investRect.x + investRect.width * 0.5f, investRect.y + 45 * scaleFactor, 
                                  investRect.width * 0.25f, 15 * scaleFactor), 
                         $"Time: {investment.timeToMaturity:F0} months", labelStyle);
                
                // Risk level
                GUIStyle riskStyle = new GUIStyle(labelStyle);
                if (investment.riskLevel > 50f) riskStyle.normal.textColor = Color.red;
                else if (investment.riskLevel > 25f) riskStyle.normal.textColor = Color.yellow;
                else riskStyle.normal.textColor = Color.green;
                
                GUI.Label(new Rect(investRect.x + investRect.width * 0.75f, investRect.y + 45 * scaleFactor, 
                                  investRect.width * 0.2f, 15 * scaleFactor), 
                         $"Risk: {investment.riskLevel:F0}%", riskStyle);
                
                // Action button
                string buttonText = investment.isActive ? "MANAGE" : "INVEST";
                GUI.enabled = !investment.isActive ? currentCash >= investment.costToInvest : true;
                
                if (GUI.Button(new Rect(investRect.x + 10 * scaleFactor, investRect.y + 70 * scaleFactor, 
                                       100 * scaleFactor, 30 * scaleFactor), 
                              buttonText, buttonStyle))
                {
                    if (investment.isActive)
                    {
                        ManageInvestment(investment);
                    }
                    else
                    {
                        MakeInvestment(investment);
                    }
                }
                
                GUI.enabled = true;
                
                // Progress bar for active investments
                if (investment.isActive)
                {
                    float progress = (float)(DateTime.Now - investment.startDate).TotalDays / (investment.timeToMaturity * 30f);
                    progress = Mathf.Clamp01(progress);
                    
                    Rect progressRect = new Rect(investRect.x + 120 * scaleFactor, investRect.y + 75 * scaleFactor, 
                                               investRect.width - 140 * scaleFactor, 20 * scaleFactor);
                    GUI.Box(progressRect, "", windowStyle);
                    
                    Rect fillRect = new Rect(progressRect.x, progressRect.y, progressRect.width * progress, progressRect.height);
                    GUI.Box(fillRect, "", new GUIStyle(GUI.skin.box) { normal = { background = MakeTexture(Color.green) } });
                    
                    GUI.Label(progressRect, $"Progress: {progress * 100f:F1}%", labelStyle);
                }
                
                yPos += 120 * scaleFactor;
            }
            
            GUI.EndScrollView();
        }
        
        private void DrawLegalContent(Rect contentRect)
        {
            // Warning header
            GUI.Label(new Rect(contentRect.x + 10 * scaleFactor, contentRect.y + 10 * scaleFactor, 
                              contentRect.width - 20 * scaleFactor, 20 * scaleFactor), 
                     "⚖️ LEGAL SERVICES & RISK MANAGEMENT", headerStyle);
            
            scrollPosition = GUI.BeginScrollView(
                new Rect(contentRect.x, contentRect.y + 40 * scaleFactor, contentRect.width, contentRect.height - 40 * scaleFactor), 
                scrollPosition, 
                new Rect(0, 0, contentRect.width - 20 * scaleFactor, legalServices.Count * 100 * scaleFactor), 
                false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            
            float yPos = 10 * scaleFactor;
            
            foreach (var service in legalServices)
            {
                Rect serviceRect = new Rect(10 * scaleFactor, yPos, contentRect.width - 40 * scaleFactor, 90 * scaleFactor);
                GUI.Box(serviceRect, "", windowStyle);
                
                // Service name and availability
                Color availabilityColor = service.isAvailable ? Color.white : Color.gray;
                GUIStyle serviceStyle = new GUIStyle(headerStyle);
                serviceStyle.normal.textColor = availabilityColor;
                
                GUI.Label(new Rect(serviceRect.x + 10 * scaleFactor, serviceRect.y + 5 * scaleFactor, 
                                  serviceRect.width * 0.6f, 20 * scaleFactor), 
                         service.serviceName, serviceStyle);
                
                string availabilityText = service.isAvailable ? "AVAILABLE" : "UNAVAILABLE";
                GUI.Label(new Rect(serviceRect.x + serviceRect.width * 0.7f, serviceRect.y + 5 * scaleFactor, 
                                  serviceRect.width * 0.25f, 20 * scaleFactor), 
                         availabilityText, serviceStyle);
                
                // Description
                GUI.Label(new Rect(serviceRect.x + 10 * scaleFactor, serviceRect.y + 25 * scaleFactor, 
                                  serviceRect.width - 20 * scaleFactor, 15 * scaleFactor), 
                         service.description, labelStyle);
                
                // Service details
                GUI.Label(new Rect(serviceRect.x + 10 * scaleFactor, serviceRect.y + 45 * scaleFactor, 
                                  serviceRect.width * 0.3f, 15 * scaleFactor), 
                         $"Cost: ${service.cost:N0}", labelStyle);
                
                GUI.Label(new Rect(serviceRect.x + serviceRect.width * 0.3f, serviceRect.y + 45 * scaleFactor, 
                                  serviceRect.width * 0.3f, 15 * scaleFactor), 
                         $"Success Rate: {service.successRate:F0}%", labelStyle);
                
                GUI.Label(new Rect(serviceRect.x + serviceRect.width * 0.6f, serviceRect.y + 45 * scaleFactor, 
                                  serviceRect.width * 0.35f, 15 * scaleFactor), 
                         $"Last Used: {service.lastUsed:MM/dd/yyyy}", labelStyle);
                
                // Action button
                GUI.enabled = service.isAvailable && currentCash >= service.cost;
                
                if (GUI.Button(new Rect(serviceRect.x + 10 * scaleFactor, serviceRect.y + 65 * scaleFactor, 
                                       120 * scaleFactor, 20 * scaleFactor), 
                              "HIRE SERVICE", buttonStyle))
                {
                    HireLegalService(service);
                }
                
                GUI.enabled = true;
                
                yPos += 100 * scaleFactor;
            }
            
            GUI.EndScrollView();
        }
        
        private void ContactDealer(Dealer dealer)
        {
            dealer.lastContact = DateTime.Now;
            Debug.Log($"Contacted dealer: {dealer.name}");
        }
        
        private void MakeInvestment(Investment investment)
        {
            if (currentCash >= investment.costToInvest)
            {
                currentCash -= investment.costToInvest;
                investment.isActive = true;
                investment.startDate = DateTime.Now;
                Debug.Log($"Invested in: {investment.name}");
            }
        }
        
        private void ManageInvestment(Investment investment)
        {
            Debug.Log($"Managing investment: {investment.name}");
        }
        
        private void HireLegalService(LegalService service)
        {
            if (currentCash >= service.cost)
            {
                currentCash -= service.cost;
                service.lastUsed = DateTime.Now;
                
                // Apply service effects
                if (service.serviceName.Contains("Bribe"))
                {
                    heatLevel = Mathf.Max(0f, heatLevel - 20f);
                }
                else if (service.serviceName.Contains("Evidence"))
                {
                    heatLevel = Mathf.Max(0f, heatLevel - 15f);
                }
                
                Debug.Log($"Hired: {service.serviceName}");
            }
        }
        
        private void UpdateHeatLevel()
        {
            // Gradually increase heat level over time
            if (Time.time % 30f < 0.1f) // Every 30 seconds
            {
                heatLevel += UnityEngine.Random.Range(0.5f, 2f);
                heatLevel = Mathf.Clamp(heatLevel, 0f, 100f);
                
                // Update service availability based on heat level
                foreach (var service in legalServices)
                {
                    if (service.serviceName.Contains("Bribe"))
                        service.isAvailable = heatLevel > 40f;
                    else if (service.serviceName.Contains("Evidence"))
                        service.isAvailable = heatLevel > 60f;
                    else if (service.serviceName.Contains("Judge"))
                        service.isAvailable = heatLevel > 50f;
                }
            }
        }
    }
}