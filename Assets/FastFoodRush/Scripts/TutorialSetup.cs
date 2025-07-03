using UnityEngine;
using TMPro;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Helper script to automatically setup Tutorial references at runtime
    /// This ensures the tutorial system works even when prefab references are missing
    /// </summary>
    public class TutorialSetup : MonoBehaviour
    {
        [SerializeField, Tooltip("The Tutorial component to setup")]
        private Tutorial tutorial;

        [SerializeField, Tooltip("UI Text component for tutorial messages")]
        private TextMeshProUGUI tutorialMessageUI;

        void Start()
        {
            SetupTutorialReferences();
        }

        /// <summary>
        /// Automatically finds and assigns all required tutorial references
        /// </summary>
        private void SetupTutorialReferences()
        {
            if (tutorial == null)
            {
                tutorial = FindObjectOfType<Tutorial>();
                if (tutorial == null)
                {
                    Debug.LogWarning("TutorialSetup: No Tutorial component found in scene!");
                    return;
                }
            }

            // Find and assign Player Controller
            if (tutorial.player == null)
            {
                tutorial.player = FindObjectOfType<PlayerController>();
                if (tutorial.player != null)
                    Debug.Log($"TutorialSetup: Assigned player = {tutorial.player.name}");
            }

            // Find and assign Mobile Tutorial UI (preferred)
            if (tutorial.mobileTutorialUI == null)
            {
                tutorial.mobileTutorialUI = FindObjectOfType<MobileTutorialUI>();
                if (tutorial.mobileTutorialUI != null)
                    Debug.Log($"TutorialSetup: Assigned mobileTutorialUI = {tutorial.mobileTutorialUI.name}");
            }

            // Find and assign UI Text Message (fallback)
            if (tutorial.tutorialMessage == null && tutorial.mobileTutorialUI == null)
            {
                if (tutorialMessageUI != null)
                {
                    tutorial.tutorialMessage = tutorialMessageUI;
                }
                else
                {
                    // Try to find tutorial message UI automatically
                    var tutorialUI = GameObject.Find("TutorialMessage");
                    if (tutorialUI != null)
                    {
                        tutorial.tutorialMessage = tutorialUI.GetComponent<TextMeshProUGUI>();
                    }
                    
                    // Alternative: Look for any TextMeshProUGUI with "tutorial" in the name
                    if (tutorial.tutorialMessage == null)
                    {
                        var allTexts = FindObjectsOfType<TextMeshProUGUI>();
                        foreach (var text in allTexts)
                        {
                            if (text.name.ToLower().Contains("tutorial"))
                            {
                                tutorial.tutorialMessage = text;
                                break;
                            }
                        }
                    }
                }
                
                if (tutorial.tutorialMessage != null)
                    Debug.Log($"TutorialSetup: Assigned tutorialMessage = {tutorial.tutorialMessage.name}");
            }

            // Find and assign Seating areas
            var seatings = FindObjectsOfType<Seating>();
            if (tutorial.firstSeating == null && seatings.Length >= 1)
            {
                tutorial.firstSeating = seatings[0];
                Debug.Log($"TutorialSetup: Assigned firstSeating = {tutorial.firstSeating.name}");
            }
            if (tutorial.secondSeating == null && seatings.Length >= 2)
            {
                tutorial.secondSeating = seatings[1];
                Debug.Log($"TutorialSetup: Assigned secondSeating = {tutorial.secondSeating.name}");
            }

            // Find and assign Counter Table
            if (tutorial.counterTable == null)
            {
                tutorial.counterTable = FindObjectOfType<CounterTable>();
                if (tutorial.counterTable != null)
                    Debug.Log($"TutorialSetup: Assigned counterTable = {tutorial.counterTable.name}");
            }

            // Find and assign Food Machine
            if (tutorial.foodMachine == null)
            {
                tutorial.foodMachine = FindObjectOfType<FoodMachine>();
                if (tutorial.foodMachine != null)
                    Debug.Log($"TutorialSetup: Assigned foodMachine = {tutorial.foodMachine.name}");
            }

            // Find and assign Office unlockables
            var unlockables = FindObjectsOfType<Unlockable>();
            foreach (var unlockable in unlockables)
            {
                // Check if this is an HR office
                if (tutorial.officeHR == null && 
                    (unlockable.name.ToLower().Contains("hr") || 
                     unlockable.name.ToLower().Contains("human") ||
                     unlockable.name.ToLower().Contains("office")))
                {
                    tutorial.officeHR = unlockable;
                    Debug.Log($"TutorialSetup: Assigned officeHR = {tutorial.officeHR.name}");
                }
                // Check if this is a GM office
                else if (tutorial.officeGM == null && 
                         (unlockable.name.ToLower().Contains("gm") || 
                          unlockable.name.ToLower().Contains("general") ||
                          unlockable.name.ToLower().Contains("manager")))
                {
                    tutorial.officeGM = unlockable;
                    Debug.Log($"TutorialSetup: Assigned officeGM = {tutorial.officeGM.name}");
                }
            }

            Debug.Log("TutorialSetup: Tutorial references setup completed!");
        }



        /// <summary>
        /// Manual setup method for editor use
        /// </summary>
        [ContextMenu("Setup Tutorial References")]
        public void ManualSetup()
        {
            SetupTutorialReferences();
        }
    }
} 