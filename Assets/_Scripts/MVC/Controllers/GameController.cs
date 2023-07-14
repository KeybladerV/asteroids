using System;
using System.Collections.Generic;
using Asteroid.Utility;
using Asteroid.Utility.Observer;
using MVC.Controllers.Enums;
using MVC.Controllers.Events;
using MVC.Controllers.Interfaces;
using UnityEngine;

namespace MVC.Controllers
{
    public sealed class GameController : IGameController
    {
        [Inject] public IEventDispatcher Dispatcher { get; set; }
        [Inject] public IUIController UIController { get; set; }
        
        private Dictionary<GameState, HashSet<IUpdatable>> _gamestateUpdatables;
        private HashSet<IUpdatable> _currentUpdatables;
        
        private Dictionary<ContainerType, Transform> _containers = new();

        private GameState _gameState;
        
        public void Start()
        {
            UIController.PrepareAllUI();
            Dispatcher.Dispatch(GameEvents.OnGameStateChanged, GameState.MainMenu);
        }
        
        public void Update(float deltaTime)
        {
            if(_currentUpdatables != null)
                foreach (var updatable in _currentUpdatables)
                    updatable.Update(deltaTime);
        }
        
        public void Subscribe(IUpdatable updatable, GameState state)
        {
            if(_gamestateUpdatables == null)
                InitUpdateables();
            
            _gamestateUpdatables[state].Add(updatable);
        }
        
        public void Unsubscribe(IUpdatable updatable, GameState state)
        {
            _gamestateUpdatables[state].Remove(updatable);
        }
        
        public void ChangeGameState(GameState gameState)
        {
            _gameState = gameState;
            Dispatcher.Dispatch(GameEvents.OnGameStateChanged, _gameState);

            _currentUpdatables = _gamestateUpdatables[_gameState];
            
            if(gameState == GameState.Restart)
                ChangeGameState(GameState.Game);
            if(gameState == GameState.Exit)
                Application.Quit();
        }

        public IGameController SetContainer(Transform container, ContainerType type)
        {
            _containers.Add(type, container);
            return this;
        }
        
        public Transform GetContainer(ContainerType type)
        {
            return _containers[type];
        }
        
        private void InitUpdateables()
        {
            _gamestateUpdatables = new();
            foreach (GameState state in Enum.GetValues(typeof(GameState)))
                _gamestateUpdatables.Add(state, new HashSet<IUpdatable>());
        }
    }
}