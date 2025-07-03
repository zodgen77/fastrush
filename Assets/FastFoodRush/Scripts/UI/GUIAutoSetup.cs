using UnityEngine;
using UnityEngine.SceneManagement;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Auto-setup script that ensures the Legacy GUI Controller is available in every scene
    /// Add this script to any persistent GameObject or let it create one automatically
    /// </summary>
    public class GUIAutoSetup : MonoBehaviour
    {
        [Header("Auto Setup Settings")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool persistAcrossScenes = true;
        [SerializeField] private string guiControllerName = "LegacyGUIController";
        
        private static bool hasBeenSetup = false;
        private static GUIAutoSetup instance;
        
        void Awake()
        {
            // Ensure only one instance exists
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            
            if (persistAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        
        void Start()
        {
            if (autoSetupOnStart && !hasBeenSetup)
            {
                SetupLegacyGUI();
                hasBeenSetup = true;
            }
        }
        
        [ContextMenu("Setup Legacy GUI")]
        public void SetupLegacyGUI()
        {
            // Check if LegacyGUIController already exists
            LegacyGUIController existingController = FindObjectOfType<LegacyGUIController>();
            
            if (existingController != null)
            {
                Debug.Log("LegacyGUIController already exists in scene!");
                return;
            }
            
            // Create new GameObject with LegacyGUIController
            GameObject guiControllerObject = new GameObject(guiControllerName);
            LegacyGUIController controller = guiControllerObject.AddComponent<LegacyGUIController>();
            
            // Position it appropriately
            guiControllerObject.transform.position = Vector3.zero;
            
            // Make it persist across scenes if desired
            if (persistAcrossScenes)
            {
                DontDestroyOnLoad(guiControllerObject);
            }
            
            Debug.Log($"LegacyGUIController has been automatically created and added to the scene!");
            Debug.Log("You can now access Finance, Dealer, and Casino windows using the bottom buttons.");
        }
        
        // Optional: Listen for scene changes to re-setup if needed
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Only setup in new scenes, not when additively loading
            if (mode == LoadSceneMode.Single && autoSetupOnStart)
            {
                // Small delay to ensure scene is fully loaded
                Invoke(nameof(CheckAndSetupGUI), 0.1f);
            }
        }
        
        private void CheckAndSetupGUI()
        {
            LegacyGUIController existingController = FindObjectOfType<LegacyGUIController>();
            if (existingController == null && autoSetupOnStart)
            {
                SetupLegacyGUI();
            }
        }
        
        // Public methods for manual control
        public static void ManualSetup()
        {
            GameObject setupObject = new GameObject("GUIAutoSetup");
            GUIAutoSetup setup = setupObject.AddComponent<GUIAutoSetup>();
            setup.SetupLegacyGUI();
        }
        
        public static void ForceSetup()
        {
            hasBeenSetup = false;
            if (instance != null)
            {
                instance.SetupLegacyGUI();
                hasBeenSetup = true;
            }
        }
    }
} 