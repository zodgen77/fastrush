using UnityEngine;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Utility script to reset tutorial progress for testing
    /// </summary>
    public class TutorialReset : MonoBehaviour
    {
        [SerializeField, Tooltip("Reset tutorial on game start")]
        private bool resetOnStart = false;

        [SerializeField, Tooltip("Key to press to reset tutorial during gameplay")]
        private KeyCode resetKey = KeyCode.T;

        void Start()
        {
            if (resetOnStart)
            {
                ResetTutorial();
            }
        }

        void Update()
        {
            // Allow manual reset during gameplay for testing
            if (Input.GetKeyDown(resetKey))
            {
                ResetTutorial();
            }
        }

        /// <summary>
        /// Resets the tutorial to the beginning
        /// </summary>
        [ContextMenu("Reset Tutorial")]
        public void ResetTutorial()
        {
            PlayerPrefs.SetInt("Tutorial", 1); // Set to TutorialState.Started
            PlayerPrefs.Save();
            Debug.Log("Tutorial has been reset! Restart the scene to begin tutorial.");
        }

        /// <summary>
        /// Marks tutorial as completed
        /// </summary>
        [ContextMenu("Complete Tutorial")]
        public void CompleteTutorial()
        {
            PlayerPrefs.SetInt("Tutorial", 13); // Set to TutorialState.Ended
            PlayerPrefs.Save();
            Debug.Log("Tutorial has been marked as completed!");
        }

        /// <summary>
        /// Gets the current tutorial state
        /// </summary>
        [ContextMenu("Check Tutorial State")]
        public void CheckTutorialState()
        {
            int state = PlayerPrefs.GetInt("Tutorial", 1);
            Debug.Log($"Current tutorial state: {state} ({(TutorialState)state})");
        }
    }
} 