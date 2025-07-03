using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
using Object = UnityEngine.Object;

namespace CryingSnow.FastFoodRush.Editor
{
    /// <summary>
    /// Editor utility to create mobile-optimized tutorial UI
    /// </summary>
    public static class MobileTutorialUICreator
    {
        [MenuItem("Fast Food Rush/Create Mobile Tutorial UI")]
        public static void CreateMobileTutorialUI()
        {
            // Create main container
            GameObject tutorialContainer = new GameObject("MobileTutorialUI");
            
                         // Add Canvas if not in a canvas
             Canvas parentCanvas = Object.FindObjectOfType<Canvas>();
            if (parentCanvas != null)
            {
                tutorialContainer.transform.SetParent(parentCanvas.transform, false);
            }
            else
            {
                // Create canvas if none exists
                GameObject canvasGO = new GameObject("TutorialCanvas");
                Canvas canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100; // High priority for tutorial
                
                CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                
                canvasGO.AddComponent<GraphicRaycaster>();
                
                tutorialContainer.transform.SetParent(canvas.transform, false);
            }

            // Setup RectTransform for container
            RectTransform containerRect = tutorialContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.05f, 0.1f);
            containerRect.anchorMax = new Vector2(0.95f, 0.35f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = Vector2.zero;

            // Add background panel
            Image backgroundPanel = tutorialContainer.AddComponent<Image>();
            backgroundPanel.color = new Color(0f, 0f, 0f, 0.7f);
            backgroundPanel.raycastTarget = false;

            // Add content layout group
            VerticalLayoutGroup layoutGroup = tutorialContainer.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(30, 30, 20, 20);
            layoutGroup.spacing = 10f;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;

            // Add content size fitter
            ContentSizeFitter sizeFitter = tutorialContainer.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Create text component
            GameObject textGO = new GameObject("TutorialText");
            textGO.transform.SetParent(tutorialContainer.transform, false);

            TextMeshProUGUI tutorialText = textGO.AddComponent<TextMeshProUGUI>();
            tutorialText.text = "Tutorial message will appear here";
            tutorialText.fontSize = 36f;
            tutorialText.enableAutoSizing = true;
            tutorialText.fontSizeMin = 24f;
            tutorialText.fontSizeMax = 48f;
            tutorialText.enableWordWrapping = true;
            tutorialText.textWrappingMode = TextWrappingModes.Normal;
            tutorialText.overflowMode = TextOverflowModes.Ellipsis;
            tutorialText.alignment = TextAlignmentOptions.Center;
            tutorialText.color = Color.white;

            // Setup text RectTransform
            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = Vector2.zero;

            // Add mobile tutorial UI component
            MobileTutorialUI mobileTutorialUI = tutorialContainer.AddComponent<MobileTutorialUI>();
            
            // Use reflection to set private fields
            var tutorialTextField = typeof(MobileTutorialUI).GetField("tutorialText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            tutorialTextField?.SetValue(mobileTutorialUI, tutorialText);

            var backgroundField = typeof(MobileTutorialUI).GetField("backgroundPanel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            backgroundField?.SetValue(mobileTutorialUI, backgroundPanel);

            var containerField = typeof(MobileTutorialUI).GetField("containerPanel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            containerField?.SetValue(mobileTutorialUI, containerRect);

            // Select the created object
            Selection.activeGameObject = tutorialContainer;

            Debug.Log("Mobile Tutorial UI created successfully! Configure the MobileTutorialUI component settings as needed.");
            
            // Show helpful message
            EditorUtility.DisplayDialog("Mobile Tutorial UI Created", 
                "Mobile Tutorial UI has been created with mobile-optimized settings.\n\n" +
                "Next steps:\n" +
                "1. Assign this to your Tutorial component's 'mobileTutorialUI' field\n" +
                "2. Test on different screen ratios using the Game view\n" +
                "3. Adjust settings in the MobileTutorialUI component if needed", 
                "OK");
        }

        [MenuItem("Fast Food Rush/Setup Canvas for Mobile")]
                 public static void SetupCanvasForMobile()
         {
             Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in scene!");
                return;
            }

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }

            // Configure for mobile
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920); // Common mobile resolution
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f; // Balance between width and height

            Debug.Log("Canvas configured for mobile devices (1080x1920 reference resolution)");
            
            EditorUtility.DisplayDialog("Canvas Setup Complete", 
                "Canvas has been configured for optimal mobile scaling:\n\n" +
                "• Reference Resolution: 1080x1920\n" +
                "• Match Width or Height: 0.5\n" +
                "• Scale Mode: Scale With Screen Size\n\n" +
                "This will ensure consistent UI scaling across different mobile devices.", 
                "OK");
        }

        [MenuItem("Fast Food Rush/Test Mobile Aspect Ratios")]
        public static void ShowMobileAspectRatios()
        {
            string message = "Common Mobile Aspect Ratios for Testing:\n\n";
            message += "• 9:16 (iPhone 11, standard) - 1125x2436\n";
            message += "• 9:20 (Samsung Galaxy S20+) - 1440x3200\n";
            message += "• 9:21 (OnePlus 8 Pro) - 1440x3168\n";
            message += "• 10:21 (Huawei P40 Pro) - 1200x2640\n";
            message += "• 19.5:9 (iPhone X series) - 1125x2436\n\n";
            message += "Use Unity's Game view to test these ratios.\n";
            message += "Add custom aspect ratios in the Game view dropdown.";

            EditorUtility.DisplayDialog("Mobile Aspect Ratios", message, "OK");
        }
    }
}
#endif 