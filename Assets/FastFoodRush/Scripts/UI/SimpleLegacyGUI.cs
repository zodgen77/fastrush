using UnityEngine;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Simplified Legacy GUI - No complex scaling, just basic buttons that should definitely appear
    /// Use this if the regular LegacyGUIController isn't showing buttons
    /// </summary>
    public class SimpleLegacyGUI : MonoBehaviour
    {
        [Header("Simple GUI Settings")]
        [SerializeField] private bool showGUI = true;
        [SerializeField] private bool showBackground = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.Tab;
        
        // Window state
        private enum WindowType { None, Finance, Dealer, Casino }
        private WindowType currentWindow = WindowType.None;
        private bool isWindowOpen = false;
        
        // Components
        private FinanceWindow financeWindow;
        private DealerWindow dealerWindow;
        private CasinoWindow casinoWindow;
        
        // Simple GUI styles
        private GUIStyle buttonStyle;
        private GUIStyle windowStyle;
        private GUIStyle labelStyle;
        private GUIStyle shadowStyle;
        private bool stylesInitialized = false;
        private bool componentsInitialized = false;
        
        void Start()
        {
            Debug.Log("=== Simple Legacy GUI Started ===");
            // Note: Component initialization moved to OnGUI to avoid GUI access errors
        }
        
        void InitializeComponents()
        {
            if (componentsInitialized) return;
            
            // Create window components - this will be called from OnGUI
            if (financeWindow == null)
            {
                GameObject financeGO = new GameObject("FinanceWindow");
                financeGO.transform.SetParent(this.transform);
                financeWindow = financeGO.AddComponent<FinanceWindow>();
                financeWindow.Initialize(new Rect(0, 0, Screen.width, Screen.height), 1f);
            }
            
            if (dealerWindow == null)
            {
                GameObject dealerGO = new GameObject("DealerWindow");
                dealerGO.transform.SetParent(this.transform);
                dealerWindow = dealerGO.AddComponent<DealerWindow>();
                dealerWindow.Initialize(new Rect(0, 0, Screen.width, Screen.height), 1f);
            }
            
            if (casinoWindow == null)
            {
                GameObject casinoGO = new GameObject("CasinoWindow");
                casinoGO.transform.SetParent(this.transform);
                casinoWindow = casinoGO.AddComponent<CasinoWindow>();
                casinoWindow.Initialize(new Rect(0, 0, Screen.width, Screen.height), 1f);
            }
            
            componentsInitialized = true;
            Debug.Log("✅ All window components created");
        }
        
        void InitializeStyles()
        {
            if (stylesInitialized) return;
            
            try
            {
                // Calculate responsive font sizes based on screen resolution
                int baseFontSize = Mathf.Max(12, Screen.width / 80); // Minimum 12, scales with screen width
                int buttonFontSize = Mathf.Max(16, Screen.width / 60); // Larger for buttons
                int labelFontSize = Mathf.Max(14, Screen.width / 70); // Medium for labels
                
                // Enhanced button style with better readability
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = buttonFontSize;
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.normal.textColor = Color.white;
                buttonStyle.hover.textColor = Color.yellow;
                buttonStyle.active.textColor = Color.cyan;
                buttonStyle.alignment = TextAnchor.MiddleCenter;
                
                // Enhanced window style
                windowStyle = new GUIStyle(GUI.skin.window);
                windowStyle.fontSize = baseFontSize;
                windowStyle.fontStyle = FontStyle.Bold;
                windowStyle.normal.textColor = Color.white;
                
                // Enhanced label style with better contrast
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontSize = labelFontSize;
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.normal.textColor = Color.white;
                labelStyle.alignment = TextAnchor.MiddleLeft;
                
                // Shadow style for text outline effect
                shadowStyle = new GUIStyle(labelStyle);
                shadowStyle.normal.textColor = Color.black;
                
                stylesInitialized = true;
                Debug.Log("SimpleLegacyGUI: Enhanced styles initialized successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"SimpleLegacyGUI: Failed to initialize styles - {e.Message}");
                // Create fallback styles
                buttonStyle = new GUIStyle();
                windowStyle = new GUIStyle();
                labelStyle = new GUIStyle();
                shadowStyle = new GUIStyle();
                stylesInitialized = true;
            }
        }
        
        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                showGUI = !showGUI;
                Debug.Log($"Simple GUI toggled: {showGUI}");
            }
            
            if (Input.GetKeyDown(KeyCode.Escape) && isWindowOpen)
            {
                CloseCurrentWindow();
            }
        }
        
        void OnGUI()
        {
            if (!showGUI) return;
            
            // Initialize components on first OnGUI call (when GUI functions are available)
            if (!componentsInitialized)
            {
                InitializeComponents();
            }
            
            InitializeStyles();
            
            // Always draw a test label to confirm OnGUI is working
            DrawLabelWithShadow(new Rect(10, 10, 400, 30), "Simple Legacy GUI Active");
            
            if (showBackground)
            {
                DrawBackground();
            }
            
            DrawMainButtons();
            DrawActiveWindow();
        }
        
        void DrawBackground()
        {
            // Simple semi-transparent background
            Color oldColor = GUI.color;
            GUI.color = new Color(0, 0, 0, 0.3f);
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            GUI.color = oldColor;
        }
        
        void DrawMainButtons()
        {
            // Responsive button layout at bottom of screen
            float buttonHeight = Mathf.Max(50, Screen.height / 15); // Responsive height
            float buttonWidth = Mathf.Max(150, Screen.width / 8); // Responsive width
            float spacing = 10;
            float totalWidth = (buttonWidth * 3) + (spacing * 2);
            float startX = (Screen.width - totalWidth) / 2f;
            float buttonY = Screen.height - buttonHeight - 20;
            
            // Finance button
            if (GUI.Button(new Rect(startX, buttonY, buttonWidth, buttonHeight), "FINANCE", buttonStyle))
            {
                ToggleWindow(WindowType.Finance);
                Debug.Log("Finance button clicked!");
            }
            
            // Dealer button
            startX += buttonWidth + spacing;
            if (GUI.Button(new Rect(startX, buttonY, buttonWidth, buttonHeight), "DEALER", buttonStyle))
            {
                ToggleWindow(WindowType.Dealer);
                Debug.Log("Dealer button clicked!");
            }
            
            // Casino button
            startX += buttonWidth + spacing;
            if (GUI.Button(new Rect(startX, buttonY, buttonWidth, buttonHeight), "CASINO", buttonStyle))
            {
                ToggleWindow(WindowType.Casino);
                Debug.Log("Casino button clicked!");
            }
            
            // Status display with improved readability
            float labelHeight = Mathf.Max(25, Screen.height / 30);
            string cashDisplay = RestaurantManager.Instance != null ? 
                $"Cash: {RestaurantManager.Instance.GetFormattedMoney(RestaurantManager.Instance.GetMoney())}" : 
                "Cash: $0";
            DrawLabelWithShadow(new Rect(10, 40, 300, labelHeight), cashDisplay);
            DrawLabelWithShadow(new Rect(10, 40 + labelHeight + 5, 300, labelHeight), $"Window: {currentWindow}");
            DrawLabelWithShadow(new Rect(10, 40 + (labelHeight + 5) * 2, 400, labelHeight), $"Press {toggleKey} to toggle GUI");
        }
        
        /// <summary>
        /// Draw text with shadow for better readability
        /// </summary>
        void DrawLabelWithShadow(Rect rect, string text)
        {
            // Draw shadow slightly offset
            Rect shadowRect = new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height);
            GUI.Label(shadowRect, text, shadowStyle);
            
            // Draw main text
            GUI.Label(rect, text, labelStyle);
        }
        
        void DrawActiveWindow()
        {
            if (!isWindowOpen) return;
            
            switch (currentWindow)
            {
                case WindowType.Finance:
                    if (financeWindow != null)
                        financeWindow.DrawWindow();
                    break;
                case WindowType.Dealer:
                    if (dealerWindow != null)
                        dealerWindow.DrawWindow();
                    break;
                case WindowType.Casino:
                    if (casinoWindow != null)
                        casinoWindow.DrawWindow();
                    break;
            }
        }
        
        void ToggleWindow(WindowType windowType)
        {
            if (currentWindow == windowType && isWindowOpen)
            {
                CloseCurrentWindow();
            }
            else
            {
                OpenWindow(windowType);
            }
        }
        
        void OpenWindow(WindowType windowType)
        {
            currentWindow = windowType;
            isWindowOpen = true;
            Debug.Log($"Opening {windowType} window");
        }
        
        void CloseCurrentWindow()
        {
            isWindowOpen = false;
            currentWindow = WindowType.None;
            Debug.Log("Closing window");
        }
        
        // Public methods
        public void ShowFinanceWindow() => OpenWindow(WindowType.Finance);
        public void ShowDealerWindow() => OpenWindow(WindowType.Dealer);
        public void ShowCasinoWindow() => OpenWindow(WindowType.Casino);
        public void CloseAllWindows() => CloseCurrentWindow();
    }
} 