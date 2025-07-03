using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace CryingSnow.FastFoodRush
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private List<StateMessage> stateMessages;
        [SerializeField] public TextMeshProUGUI tutorialMessage;
        [SerializeField] public MobileTutorialUI mobileTutorialUI;
        [SerializeField] public PlayerController player;
        [SerializeField] public Seating firstSeating;
        [SerializeField] public CounterTable counterTable;
        [SerializeField] public FoodMachine foodMachine;
        [SerializeField] public Seating secondSeating;
        [SerializeField] public Unlockable officeHR;
        [SerializeField] public Unlockable officeGM;
        [SerializeField] private GameObject arrowPrefab;

        private TutorialState currentState;
        private TutorialState messageState;

        private Dictionary<TutorialState, string> messageLookup;

        private GameObject arrow;

        private void Start()
        {
            currentState = (TutorialState)PlayerPrefs.GetInt("Tutorial", 1);

            if (currentState == TutorialState.Ended)
            {
                DestroyTutorial();
                return;
            }

            // Validate required references
            if (!ValidateReferences())
            {
                Debug.LogError("Tutorial: Missing required references! Tutorial will be disabled.");
                DestroyTutorial();
                return;
            }

            messageLookup = stateMessages.ToDictionary(m => m.State, m => m.Message);
            StartCoroutine(BeginTutorial());
        }

        /// <summary>
        /// Validates that all required tutorial references are assigned
        /// </summary>
        private bool ValidateReferences()
        {
            bool isValid = true;

            if (tutorialMessage == null && mobileTutorialUI == null)
            {
                Debug.LogError("Tutorial: Neither tutorialMessage nor mobileTutorialUI is assigned!");
                isValid = false;
            }

            if (player == null)
            {
                Debug.LogError("Tutorial: player is not assigned!");
                isValid = false;
            }

            if (firstSeating == null)
            {
                Debug.LogError("Tutorial: firstSeating is not assigned!");
                isValid = false;
            }

            if (counterTable == null)
            {
                Debug.LogError("Tutorial: counterTable is not assigned!");
                isValid = false;
            }

            if (foodMachine == null)
            {
                Debug.LogError("Tutorial: foodMachine is not assigned!");
                isValid = false;
            }

            if (arrowPrefab == null)
            {
                Debug.LogError("Tutorial: arrowPrefab is not assigned!");
                isValid = false;
            }

            return isValid;
        }

        private void DestroyTutorial()
        {
            Destroy(tutorialMessage.transform.parent.gameObject);
            Destroy(gameObject);
        }

        private IEnumerator BeginTutorial()
        {
            while (currentState == TutorialState.Started)
            {
                Vector3 playerPos = player.transform.position;
                Vector3 firstSeatingPos = firstSeating.transform.position;

                if (Vector3.Distance(playerPos, firstSeatingPos) < 8f)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                yield return null;
            }

            while (currentState == TutorialState.FirstSeating)
            {
                if (firstSeating.gameObject.activeSelf)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(firstSeating.BuyingPoint + Vector3.up * 3);
                yield return null;
            }

            while (currentState == TutorialState.CounterTable)
            {
                if (counterTable.gameObject.activeSelf)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(counterTable.BuyingPoint + Vector3.up * 3);
                yield return null;
            }

            while (currentState == TutorialState.FoodMachine)
            {
                if (foodMachine.gameObject.activeSelf)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(foodMachine.BuyingPoint + Vector3.up * 3);
                yield return null;
            }

            while (currentState == TutorialState.DeliverToCounter)
            {
                if (counterTable.FoodCount >= 5)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(counterTable.FoodPoint + Vector3.up * 3);
                yield return null;
            }

            float serveTime = 0f;

            while (currentState == TutorialState.SellFood)
            {
                if (counterTable.HasWorker) serveTime += Time.deltaTime;

                if (serveTime >= 8f)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(counterTable.WorkingPoint + Vector3.up * 3);
                yield return null;
            }

            long initialMoney = RestaurantManager.Instance.GetMoney();

            while (currentState == TutorialState.CollectRevenue)
            {
                if (RestaurantManager.Instance.GetMoney() != initialMoney)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(counterTable.MoneyPoint + Vector3.up * 3);
                yield return null;
            }

            while (currentState == TutorialState.MoreSeating)
            {
                if (secondSeating.gameObject.activeSelf)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(secondSeating.BuyingPoint + Vector3.up * 3);
                yield return null;
            }

            while (currentState == TutorialState.HireHR)
            {
                if (officeHR.gameObject.activeSelf)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(officeHR.BuyingPoint + Vector3.up * 3);
                yield return null;
            }

            while (currentState == TutorialState.HireEmployee)
            {
                if (RestaurantManager.Instance.GetUpgradeLevel(Upgrade.EmployeeAmount) > 0)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(officeHR.BuyingPoint + Vector3.up * 3);
                yield return null;
            }

            while (currentState == TutorialState.HireGM)
            {
                if (officeGM.gameObject.activeSelf)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(officeGM.BuyingPoint + Vector3.up * 3);
                yield return null;
            }

            while (currentState == TutorialState.UpgradeCounter)
            {
                if (counterTable.UnlockLevel > 1)
                {
                    AdvanceCurrentState();
                }

                TryUpdateTutorialMessage();
                UpdateArrowPosition(counterTable.BuyingPoint + Vector3.up * 3);
                yield return null;
            }

            Destroy(arrow.gameObject);

            yield return new WaitForSeconds(5f);

            DestroyTutorial();
        }

        private void TryUpdateTutorialMessage()
        {
            if (messageState != currentState)
            {
                if (messageLookup.TryGetValue(currentState, out string message))
                {
                    // Use adaptive message based on device characteristics
                    string adaptiveMessage = TutorialMessageOptimizer.GetAdaptiveMessage(currentState);
                    
                    // Use mobile UI if available, otherwise fallback to traditional UI
                    if (mobileTutorialUI != null)
                    {
                        mobileTutorialUI.ShowMessage(adaptiveMessage);
                    }
                    else if (tutorialMessage != null)
                    {
                        tutorialMessage.text = adaptiveMessage;
                        tutorialMessage.color = Color.black;
                        tutorialMessage.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f);
                    }
                }
                else
                {
                    string errorMessage = $"[ERROR] Tutorial message not found for state: {currentState}. Please add a message to the 'stateMessages' list in the Inspector.";
                    
                    if (mobileTutorialUI != null)
                    {
                        mobileTutorialUI.ShowMessage(errorMessage);
                    }
                    else if (tutorialMessage != null)
                    {
                        tutorialMessage.text = errorMessage;
                        tutorialMessage.color = Color.red;
                    }
                }

                messageState = currentState;
            }
        }

        private void UpdateArrowPosition(Vector3 position)
        {
            if (arrow == null)
            {
                arrow = Instantiate(arrowPrefab);
            }

            float verticalOffset = Mathf.Sin(Time.time * 5f) * 0.5f;
            Vector3 oscillatingPosition = position;
            oscillatingPosition.y += verticalOffset;

            arrow.transform.position = oscillatingPosition;
        }

        private void AdvanceCurrentState()
        {
            currentState = (TutorialState)((int)currentState + 1);
            PlayerPrefs.SetInt("Tutorial", (int)currentState);
        }
    }

    public enum TutorialState
    {
        None,
        Started,
        FirstSeating,
        CounterTable,
        FoodMachine,
        DeliverToCounter,
        SellFood,
        CollectRevenue,
        MoreSeating,
        HireHR,
        HireEmployee,
        HireGM,
        UpgradeCounter,
        Ended
    }

    [System.Serializable]
    public struct StateMessage
    {
        public TutorialState State;
        [TextArea] public string Message;
    }
}
