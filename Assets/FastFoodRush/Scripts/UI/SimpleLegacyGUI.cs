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
        private bool stylesInitialized = false;
        
        void Start()
        {
            Debug.Log("=== Simple Legacy GUI Started ===");
            InitializeComponents();
        }
        
        void InitializeComponents()
        {
            // Create window components
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
            
            Debug.Log("✅ All window components created");
        }
        
        void InitializeStyles()
        {
            if (stylesInitialized) return;
            
            // Simple button style
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 16;
            buttonStyle.fontStyle = FontStyle.Bold;
            
            // Simple window style
            windowStyle = new GUIStyle(GUI.skin.window);
            windowStyle.fontSize = 14;
            
            stylesInitialized = true;
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
            
            InitializeStyles();
            
            // Always draw a test label to confirm OnGUI is working
            GUI.Label(new Rect(10, 10, 300, 20), "Simple Legacy GUI Active");
            
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
            // Simple button layout at bottom of screen
            float buttonHeight = 50;
            float buttonWidth = 150;
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
            
            // Status display
            GUI.Label(new Rect(10, 40, 200, 20), $"Cash: $10,000");
            GUI.Label(new Rect(10, 60, 200, 20), $"Window: {currentWindow}");
            GUI.Label(new Rect(10, 80, 300, 20), $"Press {toggleKey} to toggle GUI");
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