#region File Information
//-----------------------------------------------------------------------------
// Button.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
#endregion Using Statements

namespace Halcyon
{
    /// <summary>
    /// A reusable button component that supports a 'press' effect, dragging (for slider handles).
    /// Raises TouchDown, Click and PositionChanged events
    /// </summary>
    public class Button
    {
        #region Fields and Properties
        private bool isTouched;
        private bool dragStarted;
        private string assetName;
        private Texture2D texture;
        private bool Enabled;
        /// <summary>
        /// Used to utilize the button as a sliding handle, with drag bounds (usually one axis)
        /// </summary>
        public Rectangle DragRestrictions { get; set; }

        protected Vector2 positionOfOrigin = Vector2.Zero;
        /// <summary>
        /// The location on the screen where the origin of the texture should be placed/drawn
        /// </summary>
        public Vector2 PositionOfOrigin
        {
            get { return positionOfOrigin; }
            set
            {
                if (positionOfOrigin != value)
                {
                    positionOfOrigin = value;
                    OnPositionChanged(EventArgs.Empty);
                }
            }
        }

        public Vector2 PositionOrigin { get; set; }
        /// <summary>
        ///  The origin coordinates inside the texture, for the positioning (as oppose to rotating/scaling)
        /// </summary>
        public Vector2 PositionForDraw
        {
            get { return PositionOfOrigin - PositionOrigin; }
        }

        /// <summary>
        /// The location on the screen of the Top/Left corner of the button
        /// </summary>
        public Vector2 TopLeftPosition
        {
            get { return PositionOfOrigin - PositionOrigin; }
            set { PositionOfOrigin = value + PositionOrigin; }
        }

        protected Color tintColor = Color.White;
        /// <summary>
        /// The tint to apply when drawing the button in a normal state
        /// </summary>
        public Color TintColor
        {
            get
            {
                Color tint =
                    Enabled ?
                    (isTouched ? TintWhenTouched ?? tintColor : tintColor)
                    : TintWhenDisabled ?? tintColor;

                return tint;
            }
            set
            {
                tintColor = value;
            }
        }

        /// <summary>
        /// The center of button Texture
        /// </summary>
        public Vector2 TextureCenter
        {
            get { return new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2)); }
        }

        /// <summary>
        /// Screen bounds
        /// </summary>
        public Rectangle ScreenBounds
        {
            get
            {
                return new Rectangle(
                    (int)TopLeftPosition.X,
                    (int)TopLeftPosition.Y,
                    (int)texture.Width,
                    (int)texture.Height);
            }
        }

        /// <summary>
        /// The tint to apply when drawing the button when in pressed state
        /// </summary>
        public Color? TintWhenTouched { get; set; }

        /// <summary>
        /// The tint to apply when drawing the button in a disabled state
        /// </summary>
        public Color? TintWhenDisabled { get; set; }

        /// <summary>
        /// Indicated whether this button is can be dragged (i.e. for slider numbs)
        /// </summary>
        public bool AllowDrag { get; set; }
        #endregion Fields

        #region Events
        public event EventHandler Click;
        /// <summary>
        /// Raises the Click event if any listeners are attached
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClick(EventArgs e)
        {
            if (Click != null)
            {
                Click.Invoke(this, e);
            }
        }

        public event EventHandler TouchDown;
        /// <summary>
        /// Raises the TouchDown event if any listeners are attached
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTouchDown(EventArgs e)
        {
            if (TouchDown != null)
            {
                TouchDown.Invoke(this, e);
            }
        }

        /// <summary>
        /// Raises the PositionChanged event if any listeners are attached
        /// </summary>
        /// <param name="e"></param>
        public event EventHandler PositionChanged;
        protected virtual void OnPositionChanged(EventArgs e)
        {
            if (PositionChanged != null)
            {
                PositionChanged.Invoke(this, e);
            }
        }
        #endregion Events

        #region Initialization
        /// <summary>
        /// Construction
        /// </summary>
        /// <param name="textureName">the texture asset name to use for drawing the button</param>
        /// <param name="game"></param>
        public Button(string textureName)
        {
            this.assetName = textureName;
            this.TintWhenTouched = Color.DarkGray;
            this.TintWhenDisabled = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            this.Enabled = true;
        }

        public void Initialize()
        {
            PositionOrigin = TextureCenter;
        }
        #endregion

        #region Loading
        /// <summary>
        /// Loads the button's assets
        /// </summary>
        public void LoadContent(ContentManager content, GraphicsDevice device)
        {
            texture = content.Load<Texture2D>(assetName);
        }
        #endregion

        #region Update and Render
        /// <summary>
        /// Each frame we check if we are pressed/released/dragged
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            Rectangle? touchRect = GetTouchRect();           
            
            if (CheckIfFirstTouchDown(touchRect))
            {
                DoOnTouchDown();
            }
            else if (isTouched && CheckIfTouchRelease(touchRect))
            {
                DoOnTouchRelease();
            }
            else if (dragStarted)
            {
                DoOnDrag(touchRect.GetValueOrDefault());
            }
            else if (!CheckIfStillTouching(touchRect))
            {
                DoOnNotTouching();
            }
        }

        /// <summary>
        /// Drawing the button
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position )
        {
            spriteBatch.Draw(texture, position, TintColor);
        }

        /// <summary>
        /// Disables the button
        /// </summary>
        public void Disable() {
            this.Enabled = false;
        }

        /// <summary>
        /// Checks for touch state and creates a small rectangle around the touch position if available
        /// </summary>
        /// <returns>A small rectangle around the touch position if available</returns>
        private Rectangle? GetTouchRect()
        {
            Rectangle? touchRect = null;

            TouchCollection touchState = TouchPanel.GetState();

            foreach (TouchLocation? location in touchState)
            {
                if ( null != location && location.Value.State != TouchLocationState.Invalid ) {
                    // creating a small rectangle around the touch position
                    touchRect = new Rectangle( ( int )location.Value.Position.X - 5,
                                              ( int )location.Value.Position.Y - 5,
                                              10, 10 );
                }
            }

            return touchRect;
        }

        /// <summary>
        /// Being called each frame to check if was just pressed
        /// </summary>
        /// <param name="touchRect">A small rectangle around the touch position if available</param>
        /// <returns>True if was pressed in the current game frame. False otherwise</returns>
        private bool CheckIfFirstTouchDown(Rectangle? touchRect)
        {
            bool isFirstTouched = false;
            if (!isTouched && touchRect != null)
            {
                isFirstTouched = ScreenBounds.Intersects(touchRect.Value);
            }
            
            return isFirstTouched;
        }

        /// <summary>
        /// Being called each frame to check if is still touched ('pressed')
        /// </summary>
        /// <param name="touchRect">A small rectangle around the touch position if available</param>
        /// <returns>True if is still pressed. False otherwise</returns>
        private bool CheckIfStillTouching(Rectangle? touchRect)
        {
            bool isStillTouching = false;

            if (touchRect != null)
            {
                isStillTouching = ScreenBounds.Intersects(touchRect.Value);
            }

            return isStillTouching;
        }

        /// <summary>
        /// Being called each frame to check if is still touched ('pressed')
        /// </summary>
        /// <param name="touchRect">A small rectangle around the touch position if available</param>
        /// <returns>True if is still pressed. False otherwise</returns>
        private bool CheckIfTouchRelease(Rectangle? touchRect)
        {
            bool isTouchReleased = false;
            if (isTouched)
            {
                if (touchRect == null)
                {
                    isTouchReleased = true;
                }
            }

            return isTouchReleased;
        }

        /// <summary>
        /// Set valid flags when the button is touched
        /// </summary>
        private void DoOnTouchDown()
        {
            OnTouchDown(EventArgs.Empty);

            isTouched = true;

            if (AllowDrag)
            {
                dragStarted = true;
            }
        }

        /// <summary>
        /// Reset the flags when the button is not being touched
        /// </summary>
        private void DoOnNotTouching()
        {
            if (isTouched)
            {
                dragStarted = false;
                isTouched = false;

                if (dragStarted)
                {
                    dragStarted = false;
                }
            }
        }

        /// <summary>
        /// Perform actions on button's release
        /// </summary>
        private void DoOnTouchRelease()
        {
            if (isTouched)
            {
                DoOnNotTouching();
                OnClick(EventArgs.Empty);
            }
        }

        /// <summary>
        /// If button is draggable (slider), then perform calculations needed to update thumb position
        /// </summary>
        /// <param name="touchRect">The touch rectangle</param>
        private void DoOnDrag(Rectangle touchRect)
        {
            if (isTouched)
            {
                float newX = MathHelper.Clamp(touchRect.Center.X, DragRestrictions.Left, 
                                              DragRestrictions.Right);
                float newY = MathHelper.Clamp(touchRect.Center.Y, DragRestrictions.Top, 
                                              DragRestrictions.Bottom);

                PositionOfOrigin = new Vector2(newX, newY);
            }
            else
            {
                dragStarted = false;
            }
        }
        #endregion
    }
}
