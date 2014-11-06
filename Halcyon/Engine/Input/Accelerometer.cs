//-----------------------------------------------------------------------------
// Accelerometer.cs
//-----------------------------------------------------------------------------

// Namespace Usage
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

// rawr Namespace
namespace Halcyon
{
    /// <summary>
    /// A static encapsulation of accelerometer input to provide games with a polling-based
    /// accelerometer system.
    /// </summary>
    public static class Accelerometer
    {
        // Accelerometer sensor on the wp7 device
        private static Microsoft.Devices.Sensors.Accelerometer accelerometer =
            new Microsoft.Devices.Sensors.Accelerometer();

        // Object for locking when ReadingChanged event is fired on a different thread
        private static object threadLock = new object();

        // Used to store the value from the last accelerometer callback
        private static Vector3 nextValue = new Vector3();

        // Used to check if the accelerometer has been initialized before
        private static bool isInitialized = false;

        // Used to check if the accelerometer is active or not
        private static bool isActive = false;

        /// <summary>
        /// Initializes the Accelerometer for the current game. This method can only be called once per game.
        /// </summary>
        public static void Initialize()
        {
            // Make sure we have not initialized before
            if (isInitialized)
                throw new InvalidOperationException("Initialize can only be called once");

            if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Device)
            {
                try
                {
                    accelerometer.ReadingChanged +=
                        new EventHandler<Microsoft.Devices.Sensors.AccelerometerReadingEventArgs>(ReadingChanged);
                    accelerometer.Start();
                    isActive = true;
                }
                catch (Microsoft.Devices.Sensors.AccelerometerFailedException)
                {
                    isActive = false;
                }
            }
            else // Always return true here because of emulator using hardware keyboard simulation
                isActive = true;

            // Accelerometer is now initialized :)!
            isInitialized = true;
        }

        /// <summary>
        /// Called when the ReadingChanged event is fired
        /// </summary>
        private static void ReadingChanged(object sender, Microsoft.Devices.Sensors.AccelerometerReadingEventArgs e)
        {
            // Store the accelerometer value in our variable to be used on the next update
            lock (threadLock)
                nextValue = new Vector3((float)e.X, (float)e.Y, (float)e.Z);
        }

        /// <summary>
        /// Gets the current state of the accelerometer.
        /// </summary>
        /// <returns>A new AccelerometerState with the current state of the accelerometer.</returns>
        public static AccelerometerState GetState()
        {
            // make sure we've initialized the Accelerometer before we try to get the state
            if (!isInitialized)
                throw new InvalidOperationException("You must Initialize before you can call GetState");

            // create a new value for our state
            Vector3 stateValue = new Vector3();

            // if the accelerometer is active
            if (isActive)
            {
                if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Device)
                    // if we're on device, we'll just grab our latest reading from the accelerometer
                    lock (threadLock)
                        stateValue = nextValue;
                else
                {
                    // if we're in the emulator, we'll generate a fake acceleration value using the arrow keys
                    // press the pause/break key to toggle keyboard input for the emulator
                    KeyboardState keyboardState = Keyboard.GetState();

                    stateValue.Z = -1;

                    if (keyboardState.IsKeyDown(Keys.Left))
                        stateValue.X--;
                    if (keyboardState.IsKeyDown(Keys.Right))
                        stateValue.X++;
                    if (keyboardState.IsKeyDown(Keys.Up))
                        stateValue.Y++;
                    if (keyboardState.IsKeyDown(Keys.Down))
                        stateValue.Y--;

                    stateValue.Normalize();
                }
            }

            return new AccelerometerState(stateValue, isActive);
        }
    }

    /// <summary>
    /// An encapsulation of the accelerometer's current state.
    /// </summary>
    public struct AccelerometerState
    {
        /// <summary>
        /// Gets the accelerometer's current value in G-force.
        /// </summary>
        public Vector3 Acceleration { get; private set; }

        /// <summary>
        /// Gets whether or not the accelerometer is active and running.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Initializes a new AccelerometerState.
        /// </summary>
        /// <param name="acceleration">The current acceleration (in G-force) of the accelerometer.</param>
        /// <param name="isActive">Whether or not the accelerometer is active.</param>
        public AccelerometerState(Vector3 acceleration, bool isActive)
            : this()
        {
            Acceleration = acceleration;
            IsActive = isActive;
        }

        /// <summary>
        /// Returns a string containing the values of the Acceleration and IsActive properties.
        /// </summary>
        /// <returns>A new string describing the state.</returns>
        public override string ToString()
        {
            return string.Format("Acceleration: {0}, IsActive: {1}", Acceleration, IsActive);
        }
    }
}