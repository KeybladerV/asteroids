using System;
using Asteroid.Utility;
using Asteroid.Utility.Observer;
using MVC.Controllers.Enums;
using MVC.Controllers.Events;
using MVC.Controllers.Interfaces;

namespace MVC.Controllers
{
    public sealed class ScoreController : IScoreController
    {
        [Inject] public IEventDispatcher Dispatcher { get; set; }
        
        public int Score { get; private set; }
        
        [PostCreate]
        public void OnPostCreate()
        {
            Dispatcher.AddListener(GameEvents.OnGameStateChanged, OnGameStateChanged);
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.None:
                case GameState.Game:
                case GameState.GameOver:
                case GameState.Pause:
                    break;
                
                case GameState.Exit:
                case GameState.MainMenu:
                case GameState.Restart:
                    ResetScore();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        [OnDestroy]
        public void OnDestroy()
        {
            Dispatcher.RemoveListener(GameEvents.OnGameStateChanged, OnGameStateChanged);
        }
        
        public void AddScore(int score)
        {
            Score += score;
            Dispatcher.Dispatch(ScoreEvents.OnScoreChanged, Score);
        }

        public void ResetScore()
        {
            Score = 0;
            Dispatcher.Dispatch(ScoreEvents.OnScoreChanged, Score);
        }
    }
}