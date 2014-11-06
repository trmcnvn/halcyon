#region File Description
//-----------------------------------------------------------------------------
// ScreenFactory.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Halcyon;

namespace Halcyon
{
    /// <summary>
    /// Our game's implementation of IScreenFactory which can handle creating the screens
    /// when resuming from being tombstoned.
    /// </summary>
    public class ScreenFactory : IScreenFactory
    {
        public Screen CreateScreen(Type screenType, ScreenManager.gameStatusData gameStatusData)
        {
            Highscores hs = new Highscores();
            if (screenType == typeof(Game))
            {
                Game screen = new Game(hs);
                Game.current.playerScore = gameStatusData.score;
                Game.current.bombCount = gameStatusData.bombs;
                Game.current.player.PlayerLives = gameStatusData.lives;
                Game.current.enemy.spawnTime = gameStatusData.spawnTime;
                Game.current.enemy.enemySpeed = gameStatusData.spawnSpeed;

                return screen;
            }
            if (screenType == typeof(Paused))
            {
                Paused screen = new Paused();
                Game.current.isPaused = true;
                return screen;
            }
            return Activator.CreateInstance(screenType) as Screen;
            //
            // This lets you still take advantage of constructor arguments yet participate in the
            // serialization process of the screen manager. Of course you need to save out those
            // values when deactivating and read them back, but that means either IsolatedStorage or
            // using the PhoneApplicationService.Current.State dictionary.
        }

    }
}
