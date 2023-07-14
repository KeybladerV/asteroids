using System;
using System.Collections.Generic;
using Asteroid.Utility;
using Asteroid.Utility.Observer;
using MVC.Controllers.Enums;
using MVC.Controllers.Events;
using MVC.Controllers.Interfaces;
using MVC.Controllers.Pools.Events;
using MVC.Models;
using UnityEngine;

namespace MVC.Controllers
{
    public sealed class GameFieldController : IGameFieldController, IUpdatable
    {
        [Inject] public IGameController GameController { get; set; }
        [Inject] public IEventDispatcher Dispatcher { get; set; }

        private Camera _gameCamera;
        private Vector3 _minimumCorner;
        private Vector3 _maximumCorner;
        
        private HashSet<MovementModel> _movements;

        private HashSet<Type> _controlledRequesters;
        
        public float DiagonalLength { get; private set; }
        public Vector3 MinimumCorner => _minimumCorner;
        public Vector3 MaximumCorner => _maximumCorner;
        
        public bool IsInGameField(Vector3 position)
        {
            return position.x >= _minimumCorner.x && position.x <= _maximumCorner.x &&
                   position.y >= _minimumCorner.y && position.y <= _maximumCorner.y;
        }

        [PostCreate]
        public void OnPostCreate()
        {
            _gameCamera = Camera.main;
            CalculateCorners(_gameCamera.pixelRect);
            DiagonalLength = Vector3.Distance(_minimumCorner, _maximumCorner);

            _movements = new HashSet<MovementModel>();
            _controlledRequesters = new HashSet<Type>();

            _controlledRequesters.Add(typeof(PlayerController));
            _controlledRequesters.Add(typeof(AsteroidsController));

            Dispatcher.AddListener(PoolEvents.OnMovementModelGet, OnMovementModelGet);
            Dispatcher.AddListener(PoolEvents.OnMovementModelRelease, OnMovementModelRelease);
            Dispatcher.AddListener(GameEvents.OnGameStateChanged, OnGameStateChange);

            GameController.Subscribe(this, GameState.Game);
        }

        private void OnGameStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.None:
                case GameState.Game:
                case GameState.GameOver:
                case GameState.Pause:
                    break;
                
                case GameState.MainMenu:
                case GameState.Exit:
                case GameState.Restart:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        [OnDestroy]
        public void OnDestroy()
        {
            Dispatcher.RemoveListener(PoolEvents.OnMovementModelGet, OnMovementModelGet);
            Dispatcher.RemoveListener(PoolEvents.OnMovementModelRelease, OnMovementModelRelease);
        }

        private void OnMovementModelGet(MovementModel model, Type requester)
        {
            if (_controlledRequesters.Contains(requester))
                _movements.Add(model);
        }

        private void OnMovementModelRelease(MovementModel model, Type requester)
        {
            if (_controlledRequesters.Contains(requester))
                _movements.Remove(model);
        }

        private void CalculateCorners(Rect rect)
        {
            _minimumCorner = _gameCamera.ScreenToWorldPoint(new Vector3(rect.xMin, rect.yMin, 0));
            _maximumCorner = _gameCamera.ScreenToWorldPoint(new Vector3(rect.xMax, rect.yMax, 0));
        }

        public void Update(float deltaTime)
        {
            foreach (var model in _movements)
            {
                var position = model.Position.GetValue();
                position.x = Loop(position.x, _minimumCorner.x, _maximumCorner.x);
                position.y = Loop(position.y, _minimumCorner.y, _maximumCorner.y);
                model.Position.SetValue(position);
            }
        }

        private float Loop(float value, float min, float max)
        {
            float range = max - min;
            if (value < min)
                value += range;
            else if (value > max)
                value -= range;

            return value;
        }
    }
}