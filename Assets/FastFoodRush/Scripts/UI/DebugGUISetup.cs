using UnityEngine;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Debug version to help identify GUI visibility issues
    /// This will show basic GUI elements to confirm OnGUI is working
    /// </summary>
    public class DebugGUISetup : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool useSimpleGUI = true;
        [SerializeField] private bool enableLegacyGUI = true;
        
        private LegacyGUIController guiController;
        private Rect debugRect;
        
        void Start()
        {
            Debug.Log("=== Debug GUI Setup Starting ===");
            
            if (enableLegacyGUI)
            {
                SetupLegacyGUI();
            }
            
            // Calculate debug rect
            debugRect = new Rect(10, 10, 300, 100);
            
            Debug.Log($"Screen resolution: {Screen.width}x{Screen.height}");
            Debug.Log($"Debug rect: {debugRect}");
        }
        
        void SetupLegacyGUI()
        {
            guiController = FindObjectOfType<LegacyGUIController>();
            
            if (guiController == null)
            {
                GameObject guiObject = new GameObject("LegacyGUIController");
                guiController = guiObject.AddComponent<LegacyGUIController>();
                Debug.Log("✅ Created LegacyGUIController");
            }
            else
            {
                Debug.Log("✅ Found existing LegacyGUIController");
            }
        }
        
        void OnGUI()
        {
            if (showDebugInfo)
            {
                DrawDebugInfo();
            }
            
            if (useSimpleGUI)
            {
                DrawSimpleTestGUI();
            }
        }
        
        void DrawDebugInfo()
        {
            // Simple debug background
            GUI.Box(debugRect, "");
            
            // Debug text
            GUILayout.BeginArea(new Rect(debugRect.x + 5, debugRect.y + 5, debugRect.width - 10, debugRect.height - 10));
            
            GUILayout.Label("=== DEBUG GUI ===");
            GUILayout.Label($"Screen: {Screen.width}x{Screen.height}");
            GUILayout.Label($"OnGUI Working: ✅");
            GUILayout.Label($"Controller: {(guiController != null ? "✅" : "❌")}");
            
            GUILayout.EndArea();
        }
        
        void DrawSimpleTestGUI()
        {
            // Test buttons at different positions
            
            // Top-left corner
            if (GUI.Button(new Rect(10, 120, 100, 30), "TOP LEFT"))
            {
                Debug.Log("Top-left button clicked!");
            }
            
            // Center screen
            float centerX = Screen.width / 2f - 50;
            float centerY = Screen.height / 2f - 15;
            if (GUI.Button(new Rect(centerX, centerY, 100, 30), "CENTER"))
            {
                Debug.Log("Center button clicked!");
            }
            
            // Bottom-right corner
            if (GUI.Button(new Rect(Screen.width - 110, Screen.height - 40, 100, 30), "BOTTOM RIGHT"))
            {
                Debug.Log("Bottom-right button clicked!");
            }
            
            // Bottom center (where Legacy GUI buttons should be)
            float bottomY = Screen.height - 70;
            float buttonWidth = (Screen.width - 40) / 3f;
            
            if (GUI.Button(new Rect(10, bottomY, buttonWidth, 30), "FINANCE TEST"))
            {
                Debug.Log("Finance test button clicked!");
                if (guiController != null)
                {
                    guiController.ShowFinanceWindow();
                }
            }
            
            if (GUI.Button(new Rect(10 + buttonWidth + 5, bottomY, buttonWidth, 30), "DEALER TEST"))
            {
                Debug.Log("Dealer test button clicked!");
                if (guiController != null)
                {
                    guiController.ShowDealerWindow();
                }
            }
            
            if (GUI.Button(new Rect(10 + (buttonWidth + 5) * 2, bottomY, buttonWidth, 30), "CASINO TEST"))
            {
                Debug.Log("Casino test button clicked!");
                if (guiController != null)
                {
                    guiController.ShowCasinoWindow();
                }
            }
            
            // Instructions
            GUI.Label(new Rect(10, Screen.height - 120, 400, 20), 
                      "If you can see these buttons, OnGUI is working!");
            GUI.Label(new Rect(10, Screen.height - 100, 400, 20), 
                      "Try clicking the test buttons to check functionality.");
        }
        
        void Update()
        {
            // Debug key controls
            if (Input.GetKeyDown(KeyCode.F1))
            {
                showDebugInfo = !showDebugInfo;
                Debug.Log($"Debug info: {showDebugInfo}");
            }
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                useSimpleGUI = !useSimpleGUI;
                Debug.Log($"Simple GUI: {useSimpleGUI}");
            }
            
            if (Input.GetKeyDown(KeyCode.F3))
            {
                if (guiController != null)
                {
                    Debug.Log("Forcing GUI controller recalculation...");
                    // Force recalculation by disabling/enabling
                    guiController.enabled = false;
                    guiController.enabled = true;
                }
            }
        }
    }
} 