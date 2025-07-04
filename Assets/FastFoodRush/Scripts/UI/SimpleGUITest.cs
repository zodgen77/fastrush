using UnityEngine;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Simple test script to verify OnGUI is working
    /// Use this to test if basic GUI functions work before trying the complex SimpleLegacyGUI
    /// </summary>
    public class SimpleGUITest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool showTestGUI = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.Tab;
        
        private int buttonClickCount = 0;
        
        // Enhanced styles for better readability
        private GUIStyle labelStyle;
        private GUIStyle shadowStyle;
        private GUIStyle buttonStyle;
        private bool stylesInitialized = false;
        
        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                showTestGUI = !showTestGUI;
                Debug.Log($"Test GUI toggled: {showTestGUI}");
            }
        }
        
        void OnGUI()
        {
            if (!showTestGUI) return;
            
            InitializeStyles();
            
            // Test label with improved readability
            DrawLabelWithShadow(new Rect(10, 10, 400, 30), "Simple GUI Test - OnGUI Working!");
            
            // Test button with enhanced style
            if (GUI.Button(new Rect(10, 50, 120, 40), "Test Button", buttonStyle))
            {
                buttonClickCount++;
                Debug.Log($"Button clicked {buttonClickCount} times!");
            }
            
            // Counter display with shadow
            DrawLabelWithShadow(new Rect(10, 100, 200, 25), $"Clicks: {buttonClickCount}");
            
            // Instructions with shadow
            DrawLabelWithShadow(new Rect(10, 130, 350, 25), $"Press {toggleKey} to toggle GUI");
            
            // Screen info with shadow
            DrawLabelWithShadow(new Rect(10, 160, 350, 25), $"Screen: {Screen.width}x{Screen.height}");
            
            // Test a few more buttons to confirm interaction with enhanced styling
            if (GUI.Button(new Rect(10, 195, 90, 35), "Finance", buttonStyle))
            {
                Debug.Log("Finance button would open here!");
            }
            
            if (GUI.Button(new Rect(110, 195, 90, 35), "Dealer", buttonStyle))
            {
                Debug.Log("Dealer button would open here!");
            }
            
            if (GUI.Button(new Rect(210, 195, 90, 35), "Casino", buttonStyle))
            {
                Debug.Log("Casino button would open here!");
            }
        }
        
        void InitializeStyles()
        {
            if (stylesInitialized) return;
            
            try
            {
                // Calculate responsive font sizes
                int labelFontSize = Mathf.Max(14, Screen.width / 70);
                int buttonFontSize = Mathf.Max(16, Screen.width / 60);
                
                // Enhanced label style
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontSize = labelFontSize;
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.normal.textColor = Color.white;
                
                // Shadow style for text outline effect
                shadowStyle = new GUIStyle(labelStyle);
                shadowStyle.normal.textColor = Color.black;
                
                // Enhanced button style
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = buttonFontSize;
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.normal.textColor = Color.white;
                buttonStyle.hover.textColor = Color.yellow;
                buttonStyle.active.textColor = Color.cyan;
                
                stylesInitialized = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"SimpleGUITest: Failed to initialize styles - {e.Message}");
                // Create fallback styles
                labelStyle = new GUIStyle();
                shadowStyle = new GUIStyle();
                buttonStyle = new GUIStyle();
                stylesInitialized = true;
            }
        }
        
        void DrawLabelWithShadow(Rect rect, string text)
        {
            // Draw shadow slightly offset
            Rect shadowRect = new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height);
            GUI.Label(shadowRect, text, shadowStyle);
            
            // Draw main text
            GUI.Label(rect, text, labelStyle);
        }
    }
} 