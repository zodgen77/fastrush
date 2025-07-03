using UnityEngine;
using System.Collections.Generic;

namespace CryingSnow.FastFoodRush
{
    /// <summary>
    /// Utility to optimize tutorial messages for mobile devices
    /// Provides shorter, more concise messages suitable for small screens
    /// </summary>
    public static class TutorialMessageOptimizer
    {
        /// <summary>
        /// Mobile-optimized tutorial messages (shorter and more direct)
        /// </summary>
        public static Dictionary<TutorialState, string> MobileOptimizedMessages = new Dictionary<TutorialState, string>
        {
            { TutorialState.Started, "👆 Swipe to move around your restaurant!" },
            { TutorialState.FirstSeating, "🪑 Buy seating for customers\nTap the highlighted area" },
            { TutorialState.CounterTable, "🍽️ Add a Counter Table\nCustomers need somewhere to order" },
            { TutorialState.FoodMachine, "🍔 Install a Food Machine\nProduce delicious food" },
            { TutorialState.DeliverToCounter, "📦 Deliver food to counter\nAim for 5+ items" },
            { TutorialState.SellFood, "💰 Serve customers!\nGo behind counter and complete orders" },
            { TutorialState.CollectRevenue, "💵 Collect your earnings\nTap the money to expand" },
            { TutorialState.MoreSeating, "🪑+ More customers coming!\nAdd extra seating" },
            { TutorialState.HireHR, "👔 Hire HR Manager\nManage your growing team" },
            { TutorialState.HireEmployee, "👨‍🍳 Hire your first employee\nIncrease Amount in upgrades" },
            { TutorialState.HireGM, "🎯 Hire General Manager\nBoost your restaurant stats" },
            { TutorialState.UpgradeCounter, "⬆️ Upgrade Counter Table\nAdd a cashier for automation" },
            { TutorialState.Ended, "🎉 Congratulations!\nYou're ready to run your restaurant empire!" }
        };

        /// <summary>
        /// Get optimized message for mobile devices
        /// </summary>
        public static string GetMobileMessage(TutorialState state)
        {
            return MobileOptimizedMessages.TryGetValue(state, out string message) 
                ? message 
                : $"Tutorial step: {state}";
        }

        /// <summary>
        /// Get standard message (original longer version)
        /// </summary>
        public static string GetStandardMessage(TutorialState state)
        {
            return StandardMessages.TryGetValue(state, out string message) 
                ? message 
                : $"Tutorial step: {state}";
        }

        /// <summary>
        /// Standard tutorial messages (original longer versions)
        /// </summary>
        public static Dictionary<TutorialState, string> StandardMessages = new Dictionary<TutorialState, string>
        {
            { TutorialState.Started, "Let's get started! Swipe the bottom-half of the screen to move around the restaurant." },
            { TutorialState.FirstSeating, "Purchase your first seating to accommodate customers and keep them happy." },
            { TutorialState.CounterTable, "Add a Counter Table so customers can place their orders and check out." },
            { TutorialState.FoodMachine, "Install a Food Machine to produce delicious food for your customers." },
            { TutorialState.DeliverToCounter, "Deliver the food to the Counter Table. Aim for at least 5 items to start." },
            { TutorialState.SellFood, "Serve the customer! Go behind the counter and complete their order." },
            { TutorialState.CollectRevenue, "Collect the earned revenue to expand and improve your restaurant!" },
            { TutorialState.MoreSeating, "More customers are arriving! Purchase additional seating to prevent overcrowding." },
            { TutorialState.HireHR, "It's getting busy! Hire a Human Resources (HR) manager to help manage your employees." },
            { TutorialState.HireEmployee, "Hire your first employee (increase Amount) to assist with tasks and improve efficiency." },
            { TutorialState.HireGM, "Hire a General Manager (GM) to boost your stats and streamline operations." },
            { TutorialState.UpgradeCounter, "Upgrade the Counter Table to add a cashier. This will help sell food while you focus on other tasks." },
            { TutorialState.Ended, "Congratulations! Now you are ready to manage your restaurant and create a thriving business!" }
        };

        /// <summary>
        /// Apply mobile-optimized messages to a Tutorial component
        /// </summary>
        public static void ApplyMobileMessages(Tutorial tutorial)
        {
            if (tutorial == null) return;

            // Use reflection to access the private stateMessages field
            var field = typeof(Tutorial).GetField("stateMessages", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                var stateMessages = new List<StateMessage>();
                
                foreach (var kvp in MobileOptimizedMessages)
                {
                    stateMessages.Add(new StateMessage 
                    { 
                        State = kvp.Key, 
                        Message = kvp.Value 
                    });
                }

                field.SetValue(tutorial, stateMessages);
                Debug.Log("TutorialMessageOptimizer: Applied mobile-optimized messages to Tutorial component");
            }
        }

        /// <summary>
        /// Check if current device should use mobile-optimized messages
        /// </summary>
        public static bool ShouldUseMobileMessages()
        {
            // Check screen size and aspect ratio
            float aspectRatio = (float)Screen.width / Screen.height;
            bool isMobileAspectRatio = aspectRatio < 0.8f; // Tall screens
            bool isSmallScreen = Screen.width < 1200 || Screen.height < 1600;
            
            // Check platform
            bool isMobilePlatform = Application.platform == RuntimePlatform.Android || 
                                   Application.platform == RuntimePlatform.IPhonePlayer;

            return isMobileAspectRatio || isSmallScreen || isMobilePlatform;
        }

        /// <summary>
        /// Get appropriate message based on device characteristics
        /// </summary>
        public static string GetAdaptiveMessage(TutorialState state)
        {
            return ShouldUseMobileMessages() 
                ? GetMobileMessage(state) 
                : GetStandardMessage(state);
        }
    }
} 