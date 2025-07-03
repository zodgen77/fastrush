using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Mobile-optimized tutorial UI system that adapts to different screen ratios
    /// Specifically designed for modern phone aspect ratios like 9:20
    /// </summary>
    public class MobileTutorialUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField, Tooltip("Tutorial message text component")]
        private TextMeshProUGUI tutorialText;

        [SerializeField, Tooltip("Background panel for tutorial")]
        private Image backgroundPanel;

        [SerializeField, Tooltip("Container panel that holds the tutorial content")]
        private RectTransform containerPanel;

        [Header("Mobile Optimization")]
        [SerializeField, Tooltip("Minimum font size for small screens")]
        private float minFontSize = 24f;

        [SerializeField, Tooltip("Maximum font size for large screens")]
        private float maxFontSize = 48f;

        [SerializeField, Tooltip("Base font size for reference resolution")]
        private float baseFontSize = 36f;

        [SerializeField, Tooltip("Reference screen width for font scaling")]
        private float referenceWidth = 1080f;

        [Header("Layout Settings")]
        [SerializeField, Tooltip("Padding from screen edges (percentage of screen)")]
        private Vector2 screenPadding = new Vector2(0.05f, 0.1f); // 5% horizontal, 10% vertical

        [SerializeField, Tooltip("Position from bottom of screen (percentage)")]
        [Range(0f, 1f)]
        private float bottomOffset = 0.15f;

        [SerializeField, Tooltip("Maximum width as percentage of screen width")]
        [Range(0.1f, 1f)]
        private float maxWidthPercent = 0.9f;

        [Header("Animation")]
        [SerializeField, Tooltip("Animation duration for show/hide")]
        private float animationDuration = 0.3f;

        [SerializeField, Tooltip("Scale animation punch effect")]
        private Vector3 punchScale = new Vector3(0.1f, 0.1f, 0.1f);

        private Canvas parentCanvas;
        private CanvasScaler canvasScaler;
        private RectTransform canvasRect;
        private bool isInitialized = false;

        void Awake()
        {
            InitializeComponents();
        }

        void Start()
        {
            OptimizeForMobile();
        }

        /// <summary>
        /// Initialize required components and references
        /// </summary>
        private void InitializeComponents()
        {
            // Find parent canvas
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                canvasScaler = parentCanvas.GetComponent<CanvasScaler>();
                canvasRect = parentCanvas.GetComponent<RectTransform>();
            }

            // Auto-find components if not assigned
            if (tutorialText == null)
                tutorialText = GetComponentInChildren<TextMeshProUGUI>();

            if (backgroundPanel == null)
                backgroundPanel = GetComponent<Image>();

            if (containerPanel == null)
                containerPanel = GetComponent<RectTransform>();

            isInitialized = true;
        }

        /// <summary>
        /// Optimize UI layout and sizing for mobile devices
        /// </summary>
        public void OptimizeForMobile()
        {
            if (!isInitialized) InitializeComponents();

            SetupCanvasScaler();
            SetupResponsiveLayout();
            SetupResponsiveFontSize();
            SetupSafeAreaHandling();
        }

        /// <summary>
        /// Setup canvas scaler for optimal mobile scaling
        /// </summary>
        private void SetupCanvasScaler()
        {
            if (canvasScaler == null) return;

            // Configure for mobile-first scaling
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920); // Common mobile resolution
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f; // Balance between width and height matching

            Debug.Log($"MobileTutorialUI: Canvas scaler configured for mobile (1080x1920 reference)");
        }

        /// <summary>
        /// Setup responsive layout that adapts to different aspect ratios
        /// </summary>
        private void SetupResponsiveLayout()
        {
            if (containerPanel == null) return;

            // Get screen dimensions
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float aspectRatio = screenWidth / screenHeight;

            // Calculate safe positioning for different aspect ratios
            Vector2 anchorMin, anchorMax;
            Vector2 anchoredPosition;
            Vector2 sizeDelta;

            // For tall screens (like 9:20), position tutorial lower
            if (aspectRatio < 0.6f) // Very tall screens (9:20, 9:21, etc.)
            {
                anchorMin = new Vector2(screenPadding.x, bottomOffset);
                anchorMax = new Vector2(1f - screenPadding.x, bottomOffset + 0.2f);
                anchoredPosition = Vector2.zero;
                sizeDelta = Vector2.zero;
            }
            else if (aspectRatio < 0.75f) // Standard tall screens (9:16, etc.)
            {
                anchorMin = new Vector2(screenPadding.x, bottomOffset + 0.05f);
                anchorMax = new Vector2(1f - screenPadding.x, bottomOffset + 0.25f);
                anchoredPosition = Vector2.zero;
                sizeDelta = Vector2.zero;
            }
            else // Wider screens
            {
                anchorMin = new Vector2(0.1f, bottomOffset + 0.1f);
                anchorMax = new Vector2(0.9f, bottomOffset + 0.3f);
                anchoredPosition = Vector2.zero;
                sizeDelta = Vector2.zero;
            }

            // Apply layout settings
            containerPanel.anchorMin = anchorMin;
            containerPanel.anchorMax = anchorMax;
            containerPanel.anchoredPosition = anchoredPosition;
            containerPanel.sizeDelta = sizeDelta;

            Debug.Log($"MobileTutorialUI: Layout optimized for aspect ratio {aspectRatio:F2}");
        }

        /// <summary>
        /// Setup responsive font size based on screen size
        /// </summary>
        private void SetupResponsiveFontSize()
        {
            if (tutorialText == null) return;

            // Calculate responsive font size based on screen width
            float screenWidth = Screen.width;
            float scaleFactor = screenWidth / referenceWidth;
            float responsiveFontSize = baseFontSize * scaleFactor;

            // Clamp to min/max values
            responsiveFontSize = Mathf.Clamp(responsiveFontSize, minFontSize, maxFontSize);

            // Apply font size
            tutorialText.fontSize = responsiveFontSize;

            // Enable auto-sizing as fallback
            tutorialText.enableAutoSizing = true;
            tutorialText.fontSizeMin = minFontSize;
            tutorialText.fontSizeMax = maxFontSize;

            // Optimize text settings for mobile
            tutorialText.enableWordWrapping = true;
            tutorialText.overflowMode = TextOverflowModes.Ellipsis;
            tutorialText.textWrappingMode = TextWrappingModes.Normal;

            Debug.Log($"MobileTutorialUI: Font size set to {responsiveFontSize:F1} (screen width: {screenWidth})");
        }

        /// <summary>
        /// Handle safe area for devices with notches/rounded corners
        /// </summary>
        private void SetupSafeAreaHandling()
        {
            // Get safe area
            Rect safeArea = Screen.safeArea;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            // Calculate safe area ratios
            Vector2 safeAreaMin = new Vector2(safeArea.x / screenSize.x, safeArea.y / screenSize.y);
            Vector2 safeAreaMax = new Vector2((safeArea.x + safeArea.width) / screenSize.x, 
                                             (safeArea.y + safeArea.height) / screenSize.y);

            // Adjust layout if safe area is significantly different from screen
            if (safeAreaMin.y > 0.05f || safeAreaMax.y < 0.95f)
            {
                // Adjust bottom offset to account for safe area
                bottomOffset = Mathf.Max(bottomOffset, safeAreaMin.y + 0.05f);
                SetupResponsiveLayout(); // Reapply layout with safe area adjustments

                Debug.Log($"MobileTutorialUI: Safe area adjustments applied. Safe area: {safeArea}");
            }
        }

        /// <summary>
        /// Show tutorial message with animation
        /// </summary>
        public void ShowMessage(string message)
        {
            if (tutorialText == null) return;

            tutorialText.text = message;
            
            // Animate in
            containerPanel.localScale = Vector3.zero;
            containerPanel.gameObject.SetActive(true);
            
            containerPanel.DOScale(Vector3.one, animationDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    // Add punch effect for emphasis
                    containerPanel.DOPunchScale(punchScale, 0.2f, 1, 0.5f);
                });

            // Fade in background
            if (backgroundPanel != null)
            {
                Color bgColor = backgroundPanel.color;
                bgColor.a = 0f;
                backgroundPanel.color = bgColor;
                backgroundPanel.DOFade(0.8f, animationDuration);
            }
        }

        /// <summary>
        /// Hide tutorial message with animation
        /// </summary>
        public void HideMessage()
        {
            if (containerPanel == null) return;

            containerPanel.DOScale(Vector3.zero, animationDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => {
                    containerPanel.gameObject.SetActive(false);
                });

            // Fade out background
            if (backgroundPanel != null)
            {
                backgroundPanel.DOFade(0f, animationDuration);
            }
        }

        /// <summary>
        /// Force refresh layout (useful when screen orientation changes)
        /// </summary>
        [ContextMenu("Refresh Mobile Layout")]
        public void RefreshLayout()
        {
            OptimizeForMobile();
        }

        /// <summary>
        /// Test different aspect ratios in editor
        /// </summary>
        [ContextMenu("Test Mobile Layout")]
        public void TestMobileLayout()
        {
            OptimizeForMobile();
            ShowMessage("This is a test tutorial message for mobile optimization. It should display properly on all screen ratios including 9:20 aspect ratio phones.");
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            // Auto-refresh in editor when values change
            if (Application.isPlaying && isInitialized)
            {
                OptimizeForMobile();
            }
        }
#endif
    }
} 