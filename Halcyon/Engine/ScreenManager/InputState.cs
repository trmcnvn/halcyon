//-----------------------------------------------------------------------------
// InputState.cs
//-----------------------------------------------------------------------------

// Namespace Usage
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Halcyon
{
    /// <summary>
    /// Helper class for Input state of Keyboard, Touch and Accelerometer
    /// </summary>
    public class InputState
    {
        // Max Input constant
        private const int maxInput = 1;
        // Current Keyboard state
        private KeyboardState[] currentKeyboardState;
        private KeyboardState[] lastKeyboardState;
        // Current Game pad state
        private GamePadState[] currentGamepadState;
        private GamePadState[] lastGamepadState;
        // Current touch panel state
        private TouchCollection touchState;
        // Gesture list
        public List<GestureSample> gestures = new List<GestureSample>();

        /// <summary>
        /// Constructs a new input state
        /// </summary>
        public InputState()
        {
            currentKeyboardState = new KeyboardState[maxInput];
            lastKeyboardState = new KeyboardState[maxInput];

            currentGamepadState = new GamePadState[maxInput];
            lastGamepadState = new GamePadState[maxInput];
        }

        /// <summary>
        /// Reads the latest state of the keyboard, touch and accelerometer
        /// </summary>
        public void Update()
        {
            lastKeyboardState[0] = currentKeyboardState[0];
            lastGamepadState[0] = currentGamepadState[0];

            currentKeyboardState[0] = Keyboard.GetState();
            currentGamepadState[0] = GamePad.GetState(PlayerIndex.One);

            touchState = TouchPanel.GetState();

            gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                gestures.Add(TouchPanel.ReadGesture());
            }
        }

        /// <summary>
        /// Checks whether a new key was pressed during this update
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            return (currentKeyboardState[(int)PlayerIndex.One].IsKeyDown(key) &&
                lastKeyboardState[(int)PlayerIndex.One].IsKeyUp(key));
        }

        /// <summary>
        /// Checks whether a new button was pressed during this update
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            return (currentGamepadState[(int)PlayerIndex.One].IsButtonDown(button) &&
                lastGamepadState[(int)PlayerIndex.One].IsButtonUp(button));
        }

        /// <summary>
        /// Checks whether a new touch panel press happened during this update
        /// </summary>
        public bool IsNewTouchPress()
        {
            return touchState.AnyTouch();
        }
    }
}
