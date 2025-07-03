# Legacy GUI Setup Instructions

## Quick Setup (Easiest Method)

### Option 1: Automatic Setup
1. **Add the QuickGUISetup script to any GameObject in your scene:**
   - Select any existing GameObject in your scene (like Main Camera, Player, etc.)
   - In the Inspector, click "Add Component"
   - Search for "QuickGUISetup" and add it
   - The GUI will automatically activate when you play the scene!

### Option 2: Manual Setup in Unity Editor
1. **Create an empty GameObject:**
   - Right-click in the Hierarchy → Create Empty
   - Name it "LegacyGUIController"
   - Add the `LegacyGUIController` component to it

## How to Use the GUI

Once the GUI is set up, you'll see three buttons at the bottom of the screen:

### 🏦 FINANCE Button
- **Banking**: View and manage bank accounts
- **Stocks**: Buy/sell stocks with real-time price changes
- **Bonds**: Invest in government and corporate bonds
- **Crypto**: Trade cryptocurrencies with high volatility

### 🤝 DEALER Button
- **Dealers**: Manage relationships with various dealers
- **Activity**: View daily business reports and profits
- **Investments**: Make business investments with different risk levels
- **Legal**: Hire legal services and manage risk factors

### 🎰 CASINO Button
- **Games**: Play various casino games (Slots, Roulette, Blackjack, etc.)
- **Betting**: Place bets with different amounts
- **History**: View your gambling history and results

## Controls

- **G Key**: Toggle the GUI on/off (default, can be changed in QuickGUISetup)
- **ESC Key**: Close any open window
- **Click**: Navigate between different windows and options

## Features

- **Mobile-Optimized**: Designed for 9:20 aspect ratio (mobile screens)
- **Adaptive Scaling**: Automatically adjusts to different screen sizes
- **Persistent**: Can persist across scene changes
- **Multiple Windows**: Finance, Dealer, and Casino systems
- **Real-time Data**: Dynamic pricing, statistics, and game results

## Troubleshooting

### GUI Not Showing?
1. Make sure you have either `QuickGUISetup` or `LegacyGUIController` in your scene
2. Check the Console for any error messages
3. Press the **G** key to toggle the GUI
4. Ensure the GameObject with the GUI script is active

### Compilation Errors?
- All BeginScrollView issues have been fixed
- Make sure all scripts are in the correct namespace: `CryingSnow.FastFoodRush`

### Performance Issues?
- The GUI uses Unity's immediate mode GUI (OnGUI)
- For better performance in production, consider converting to UI Toolkit or uGUI

## File Structure

```
Assets/FastFoodRush/Scripts/UI/
├── LegacyGUIController.cs    # Main GUI controller
├── QuickGUISetup.cs         # Easy setup script
├── GUIAutoSetup.cs          # Automatic scene setup
├── FinanceWindow.cs         # Banking and investment window
├── DealerWindow.cs          # Business management window
├── CasinoWindow.cs          # Casino and gambling window
└── README_GUI_Setup.md      # This file
```

## Notes

- This is a **legacy GUI system** using Unity's OnGUI
- Designed for **mobile game development**
- Contains **mature content** (gambling, business simulation)
- All financial data is **simulated** and for entertainment only

Enjoy exploring the Legacy GUI system! 🎮 