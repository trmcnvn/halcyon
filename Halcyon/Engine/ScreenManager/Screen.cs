//-----------------------------------------------------------------------------
// Screen.cs
//-----------------------------------------------------------------------------

// Namespace Usage
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Halcyon
{
    /// <summary>
    /// Enum of the screen transition state
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    /// <summary>
    /// Everything is a screen, the menu, the game itself
    /// </summary>
    public abstract class Screen
    {
        /// <summary>
        /// Determines if this screen is a pop up or not
        /// </summary>
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }
        private bool isPopup = false;

        /// <summary>
        /// How long each screen takes to transition
        /// </summary>
        public TimeSpan TransitionTimeOn
        {
            get { return transitionTimeOn; }
            protected set { transitionTimeOn = value; }
        }
        private TimeSpan transitionTimeOn = TimeSpan.Zero;

        /// <summary>
        /// How long each screen takes to transition
        /// </summary>
        public TimeSpan TransitionTimeOff
        {
            get { return transitionTimeOff; }
            protected set { transitionTimeOff = value; }
        }
        private TimeSpan transitionTimeOff = TimeSpan.Zero;

        /// <summary>
        /// Gets/Sets the position of the screen transition
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }
        private float transitionPosition = 1.0f;

        /// <summary>
        /// Gets/Sets the current screen transition state
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }
        private ScreenState screenState = ScreenState.TransitionOn;

        /// <summary>
        /// Indicates whether or not the screen is Exiting
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }
        private bool isExiting = false;

        public bool isBlurred = false;

        public virtual void Activate(bool instancePreserved) { }

        public virtual void Deactivate() { }

        //public List<Sprite> BlurredTextures = new List<Sprite>();

        /// <summary>
        /// Checks whether the screen is active
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus && (screenState == ScreenState.TransitionOn ||
                    screenState == ScreenState.Active);
            }
        }
        private bool otherScreenHasFocus;

        // Check's if screen is Serializable

        public bool IsSerializable
        {
            get { return isSerializable; }
            protected set { isSerializable = value; }
        }

        bool isSerializable = true;
        /// <summary>
        /// Gets/Sets the ScreenManager this screen belongs too
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }
        private ScreenManager screenManager;

        /// <summary>
        /// Gets the gestures the screen is interested in
        /// </summary>
        public GestureType EnabledGestures
        {
            get { return enabledGestures; }
            protected set
            {
                enabledGestures = value;

                if (ScreenState == ScreenState.Active)
                    TouchPanel.EnabledGestures = value;
            }
        }
        private GestureType enabledGestures = GestureType.None;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public virtual void UnloadContent() { }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Set internal otherScreenHasFocus
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                // Screen is exiting, Set it's state to transition off
                screenState = ScreenState.TransitionOff;

                // Transition is finished, remove the screen
                if (!UpdateTransition(gameTime, transitionTimeOff, 1))
                    ScreenManager.Remove(this);
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off
                if (UpdateTransition(gameTime, transitionTimeOff, 1))
                    screenState = ScreenState.TransitionOff;
                else
                    screenState = ScreenState.Hidden;
            }
            else
            {
                // Otherwise the screen should transition on and become active
                if (UpdateTransition(gameTime, transitionTimeOn, -1))
                    screenState = ScreenState.TransitionOn;
                else
                    screenState = ScreenState.Active;
            }
        }

        /// <summary>
        /// Update the transition of the screen
        /// </summary>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // Set how much to move by
            float transitionDelta;
            if (time == TimeSpan.Zero)
                transitionDelta = 1.0f;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            // Update position
            transitionPosition += transitionDelta * direction;

            // Are we at the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0.0f, 1.0f);
                return false;
            }

            // otherwise we are still transitioning
            return true;
        }

        /// <summary>
        /// Handles Input, Unlike Update() this is only called when the screen is active
        /// </summary>
        public virtual void HandleInput(InputState input, GameTime gameTime) { }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Draw(GameTime gameTime) { }

        /// <summary>
        /// Exits the screen
        /// </summary>
        public void Exit()
        {
            if (TransitionTimeOff == TimeSpan.Zero)
                ScreenManager.Remove(this);
            else
                isExiting = true;
        }
    }
}
