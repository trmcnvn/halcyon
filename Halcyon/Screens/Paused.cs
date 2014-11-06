//-----------------------------------------------------------------------------
// Paused.cs
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Halcyon
{
    /// <summary>
    /// Paused Screen!
    /// </summary>
    class Paused : MenuScreen
    {
        /// <summary>
        /// Construct a pause menu
        /// </summary>
        public Paused()
            : base("Paused")
        {
            // Set this to be a pop up screen, so screens below it don't transition off
            IsPopup = true;

            entryPosition = 330.0f;

            // Create Entries
            MenuEntry resumeEntry = new MenuEntry("Resume");
            MenuEntry restartEntry = new MenuEntry("Restart");
            MenuEntry optionsEntry = new MenuEntry("Options");
            MenuEntry exitEntry = new MenuEntry("Main Menu");

            // Create Selected functions
            resumeEntry.Selected += ResumeSelected;
            restartEntry.Selected += RestartSelected;
            optionsEntry.Selected += OptionsSelected;
            exitEntry.Selected += ExitSelected;

            // Add Entries to list!
            MenuEntries.Add(resumeEntry);
            MenuEntries.Add(restartEntry);
            MenuEntries.Add(optionsEntry);
            MenuEntries.Add(exitEntry);
        }

        /// <summary>
        /// Resume was selected on the menu
        /// </summary>
        void ResumeSelected(object sender, EventArgs e)
        {
            // Remove Paused Screen
            this.IsExiting = true;
            // Unset Paused State
            Game.current.isPaused = false;
        }

        /// <summary>
        /// Restart the game
        /// </summary>
        void RestartSelected(object sender, EventArgs e)
        {
            Dummy.Create(ScreenManager, new Background(), new Game(Game.current.playerHS));
        }

        /// <summary>
        /// Options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OptionsSelected( object sender, EventArgs e ) {
            // Remove pause screen
            this.IsExiting = true;
            // Load popup options
            ScreenManager.Add( new PopupOptions() );
        }

        /// <summary>
        /// Main Menu was selected 
        /// </summary>
        void ExitSelected(object sender, EventArgs e)
        {
            Dummy.Create(ScreenManager, new Background(), new MainMenu());
        }

        /// <summary>
        /// When Button.Back is pushed
        /// Same effect as selecting Resume
        /// </summary>
        protected override void OnCancel()
        {
            // Remove Paused Screen
            this.IsExiting = true;
            // Unset Paused State
            Game.current.isPaused = false;
        }
    }
}
