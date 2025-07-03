using UnityEngine;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Quick setup script - Add this to any GameObject in your scene to enable the Legacy GUI
    /// This is the easiest way to get the Finance, Dealer, and Casino windows working
    /// </summary>
    public class QuickGUISetup : MonoBehaviour
    {
        [Header("Quick Setup")]
        [SerializeField] private bool setupOnStart = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.G; // Press G to toggle GUI
        
        private LegacyGUIController guiController;
        private bool guiEnabled = false;
        
        void Start()
        {
            if (setupOnStart)
            {
                EnableLegacyGUI();
            }
        }
        
        void Update()
        {
            // Allow toggling GUI with a key press
            if (Input.GetKeyDown(toggleKey))
            {
                if (guiEnabled)
                {
                    DisableLegacyGUI();
                }
                else
                {
                    EnableLegacyGUI();
                }
            }
        }
        
        [ContextMenu("Enable Legacy GUI")]
        public void EnableLegacyGUI()
        {
            if (guiController == null)
            {
                // Find existing controller or create new one
                guiController = FindObjectOfType<LegacyGUIController>();
                
                if (guiController == null)
                {
                    // Create new GameObject with the GUI controller
                    GameObject guiObject = new GameObject("LegacyGUIController");
                    guiController = guiObject.AddComponent<LegacyGUIController>();
                    
                    Debug.Log("✅ Legacy GUI has been enabled!");
                    Debug.Log("💰 Click FINANCE button for banking and investments");
                    Debug.Log("🤝 Click DEALER button for business management");
                    Debug.Log("🎰 Click CASINO button for gambling games");
                    Debug.Log($"🎮 Press '{toggleKey}' key to toggle GUI on/off");
                }
            }
            
            if (guiController != null)
            {
                guiController.gameObject.SetActive(true);
                guiEnabled = true;
            }
        }
        
        [ContextMenu("Disable Legacy GUI")]
        public void DisableLegacyGUI()
        {
            if (guiController != null)
            {
                guiController.gameObject.SetActive(false);
                guiEnabled = false;
                Debug.Log("Legacy GUI disabled");
            }
        }
        
        void OnGUI()
        {
            // Show help text in the corner
            if (guiEnabled && guiController != null)
            {
                GUI.Label(new Rect(10, Screen.height - 60, 300, 40), 
                         $"Legacy GUI Active - Press '{toggleKey}' to toggle");
            }
            else
            {
                GUI.Label(new Rect(10, Screen.height - 40, 300, 20), 
                         $"Press '{toggleKey}' to enable Legacy GUI");
            }
        }
    }
} 