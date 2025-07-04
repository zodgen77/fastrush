using UnityEngine;
using DG.Tweening;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Legacy GUI Controller for mobile devices with 9:20 aspect ratio
    /// Manages the main interface and window switching
    /// </summary>
    public class LegacyGUIController : MonoBehaviour
    {
        [Header("GUI Settings")]
        [SerializeField] private bool useAdaptiveGUI = true;
        [SerializeField] private float targetAspectRatio = 0.45f; // 9:20 ratio
        [SerializeField] private Vector2 baseResolution = new Vector2(450, 1000);
        
        [Header("Window References")]
        [SerializeField] private FinanceWindow financeWindow;
        [SerializeField] private DealerWindow dealerWindow;
        [SerializeField] private CasinoWindow casinoWindow;
        
        [Header("GUI Styling")]
        [SerializeField] private GUISkin mobileSkin;
        [SerializeField] private Color primaryColor = new Color(0.2f, 0.3f, 0.4f, 0.9f);
        [SerializeField] private Color buttonColor = new Color(0.1f, 0.2f, 0.3f, 0.8f);
        [SerializeField] private Color activeButtonColor = new Color(0.3f, 0.5f, 0.7f, 0.9f);
        
        // Private variables
        private Rect screenRect;
        private float scaleFactor;
        private GUIStyle buttonStyle;
        private GUIStyle windowStyle;
        private GUIStyle labelStyle;
        
        // Window state
        private enum WindowType { None, Finance, Dealer, Casino }
        private WindowType currentWindow = WindowType.None;
        private bool isWindowOpen = false;
        
        // Button dimensions
        private float buttonHeight;
        private float buttonWidth;
        private float bottomMargin;
        
        void Start()
        {
            CalculateScreenMetrics();
            InitializeWindows();
            SetupGUIStyles();
        }
        
        void Update()
        {
            // Handle back button on mobile
            if (Input.GetKeyDown(KeyCode.Escape) && isWindowOpen)
            {
                CloseCurrentWindow();
            }
        }
        
        private void CalculateScreenMetrics()
        {
            // Calculate scale factor for 9:20 aspect ratio
            float currentAspectRatio = (float)Screen.width / Screen.height;
            scaleFactor = Mathf.Min(Screen.width / baseResolution.x, Screen.height / baseResolution.y);
            
            // Adjust for different aspect ratios
            if (currentAspectRatio > targetAspectRatio)
            {
                scaleFactor *= currentAspectRatio / targetAspectRatio;
            }
            
            // Set up screen rect for 9:20 ratio
            float guiWidth = baseResolution.x * scaleFactor;
            float guiHeight = baseResolution.y * scaleFactor;
            
            screenRect = new Rect(
                (Screen.width - guiWidth) * 0.5f,
                (Screen.height - guiHeight) * 0.5f,
                guiWidth,
                guiHeight
            );
            
            // Calculate button dimensions
            buttonHeight = 80 * scaleFactor;
            buttonWidth = (screenRect.width - 40 * scaleFactor) / 3f;
            bottomMargin = 20 * scaleFactor;
        }
        
        private void InitializeWindows()
        {
            // Find or create window components
            if (financeWindow == null)
            {
                financeWindow = FindObjectOfType<FinanceWindow>();
                if (financeWindow == null)
                {
                    GameObject financeGO = new GameObject("FinanceWindow");
                    financeGO.transform.SetParent(this.transform);
                    financeWindow = financeGO.AddComponent<FinanceWindow>();
                }
            }
            
            if (dealerWindow == null)
            {
                dealerWindow = FindObjectOfType<DealerWindow>();
                if (dealerWindow == null)
                {
                    GameObject dealerGO = new GameObject("DealerWindow");
                    dealerGO.transform.SetParent(this.transform);
                    dealerWindow = dealerGO.AddComponent<DealerWindow>();
                }
            }
            
            if (casinoWindow == null)
            {
                casinoWindow = FindObjectOfType<CasinoWindow>();
                if (casinoWindow == null)
                {
                    GameObject casinoGO = new GameObject("CasinoWindow");
                    casinoGO.transform.SetParent(this.transform);
                    casinoWindow = casinoGO.AddComponent<CasinoWindow>();
                }
            }
                
            // Initialize each window
            if (financeWindow != null)
                financeWindow.Initialize(screenRect, scaleFactor);
            if (dealerWindow != null)
                dealerWindow.Initialize(screenRect, scaleFactor);
            if (casinoWindow != null)
                casinoWindow.Initialize(screenRect, scaleFactor);
        }
        
        private void SetupGUIStyles()
        {
            // Button style
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = Mathf.RoundToInt(16 * scaleFactor);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.normal.background = MakeTexture(buttonColor);
            buttonStyle.hover.background = MakeTexture(activeButtonColor);
            buttonStyle.active.background = MakeTexture(activeButtonColor);
            
            // Window style
            windowStyle = new GUIStyle(GUI.skin.window);
            windowStyle.fontSize = Mathf.RoundToInt(14 * scaleFactor);
            windowStyle.normal.background = MakeTexture(primaryColor);
            
            // Label style
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = Mathf.RoundToInt(12 * scaleFactor);
            labelStyle.normal.textColor = Color.white;
        }
        
        private Texture2D MakeTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
        
        void OnGUI()
        {
            if (mobileSkin != null)
                GUI.skin = mobileSkin;
                
            // Set up GUI matrix for scaling
            Matrix4x4 originalMatrix = GUI.matrix;
            
            // Draw main interface
            DrawMainInterface();
            
            // Draw active window
            DrawActiveWindow();
            
            GUI.matrix = originalMatrix;
        }
        
        private void DrawMainInterface()
        {
            // Draw main background
            GUI.Box(screenRect, "", windowStyle);
            
            // Draw bottom buttons
            float buttonY = screenRect.y + screenRect.height - buttonHeight - bottomMargin;
            float buttonX = screenRect.x + 10 * scaleFactor;
            
            // Finance button
            if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "FINANCE", buttonStyle))
            {
                ToggleWindow(WindowType.Finance);
            }
            
            // Dealer button
            buttonX += buttonWidth + 5 * scaleFactor;
            if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "DEALER", buttonStyle))
            {
                ToggleWindow(WindowType.Dealer);
            }
            
            // Casino button
            buttonX += buttonWidth + 5 * scaleFactor;
            if (GUI.Button(new Rect(buttonX, buttonY, buttonWidth, buttonHeight), "CASINO", buttonStyle))
            {
                ToggleWindow(WindowType.Casino);
            }
            
            // Draw status bar at top
            DrawStatusBar();
        }
        
        private void DrawStatusBar()
        {
            Rect statusRect = new Rect(screenRect.x + 10 * scaleFactor, screenRect.y + 10 * scaleFactor, 
                                     screenRect.width - 20 * scaleFactor, 40 * scaleFactor);
            
            GUI.Box(statusRect, "", windowStyle);
            
            // Draw cash amount using centralized money system with proper formatting
            string cashText = RestaurantManager.Instance != null ? 
                $"Cash: {RestaurantManager.Instance.GetFormattedMoney(RestaurantManager.Instance.GetMoney())}" : 
                "Cash: $0";
            GUI.Label(new Rect(statusRect.x + 10 * scaleFactor, statusRect.y + 10 * scaleFactor, 
                              statusRect.width - 20 * scaleFactor, 20 * scaleFactor), cashText, labelStyle);
        }
        
        private void DrawActiveWindow()
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
        
        private void ToggleWindow(WindowType windowType)
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
        
        private void OpenWindow(WindowType windowType)
        {
            currentWindow = windowType;
            isWindowOpen = true;
            
            // Play window open sound/animation here if needed
            Debug.Log($"Opening {windowType} window");
        }
        
        private void CloseCurrentWindow()
        {
            isWindowOpen = false;
            currentWindow = WindowType.None;
            
            // Play window close sound/animation here if needed
            Debug.Log("Closing window");
        }
        
        // Connected to the centralized money system
        private float GetPlayerCash()
        {
            return RestaurantManager.Instance != null ? RestaurantManager.Instance.GetMoney() : 0f;
        }
        
        // Public methods for external access
        public void ShowFinanceWindow() => OpenWindow(WindowType.Finance);
        public void ShowDealerWindow() => OpenWindow(WindowType.Dealer);
        public void ShowCasinoWindow() => OpenWindow(WindowType.Casino);
        public void CloseAllWindows() => CloseCurrentWindow();
        
        // Window state queries
        public bool IsWindowOpen => isWindowOpen;
        public bool IsFinanceWindowOpen => isWindowOpen && currentWindow == WindowType.Finance;
        public bool IsDealerWindowOpen => isWindowOpen && currentWindow == WindowType.Dealer;
        public bool IsCasinoWindowOpen => isWindowOpen && currentWindow == WindowType.Casino;
    }
}