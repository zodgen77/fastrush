using UnityEngine;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Automatic setup script for SimpleLegacyGUI
    /// Add this to any GameObject in your scene to automatically enable SimpleLegacyGUI
    /// </summary>
    public class SimpleLegacyGUISetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        [SerializeField] private bool setupOnStart = true;
        [SerializeField] private bool createPersistentGUI = true;
        
        private SimpleLegacyGUI simpleLegacyGUI;
        
        void Start()
        {
            if (setupOnStart)
            {
                SetupSimpleLegacyGUI();
            }
        }
        
        [ContextMenu("Setup Simple Legacy GUI")]
        public void SetupSimpleLegacyGUI()
        {
            // Check if SimpleLegacyGUI already exists
            simpleLegacyGUI = FindObjectOfType<SimpleLegacyGUI>();
            
            if (simpleLegacyGUI == null)
            {
                // Create new GameObject with SimpleLegacyGUI
                GameObject guiObject = new GameObject("SimpleLegacyGUI");
                simpleLegacyGUI = guiObject.AddComponent<SimpleLegacyGUI>();
                
                if (createPersistentGUI)
                {
                    DontDestroyOnLoad(guiObject);
                }
                
                Debug.Log("✅ SimpleLegacyGUI has been automatically created!");
                Debug.Log("💰 Press TAB to toggle GUI on/off");
                Debug.Log("🎮 GUI buttons will appear at the bottom of the screen");
            }
            else
            {
                Debug.Log("✅ SimpleLegacyGUI already exists in scene!");
            }
        }
        
        [ContextMenu("Remove Simple Legacy GUI")]
        public void RemoveSimpleLegacyGUI()
        {
            SimpleLegacyGUI[] existingGUIs = FindObjectsOfType<SimpleLegacyGUI>();
            
            foreach (SimpleLegacyGUI gui in existingGUIs)
            {
                if (Application.isPlaying)
                {
                    Destroy(gui.gameObject);
                }
                else
                {
                    DestroyImmediate(gui.gameObject);
                }
            }
            
            Debug.Log("SimpleLegacyGUI has been removed from scene");
        }
    }
} 