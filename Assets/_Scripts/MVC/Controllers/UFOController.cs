using System;
using System.Collections.Generic;
using Asteroid.Utility;
using Asteroid.Utility.Observer;
using Configs;
using MVC.Controllers.Enums;
using MVC.Controllers.Events;
using MVC.Controllers.Interfaces;
using MVC.Controllers.Pools.Interfaces;
using MVC.Models;
using MVC.Views;
using MVC.Views.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MVC.Controllers
{
    public sealed class UFOController : IUFOController, IUpdatable
    {
        [Inject] public IGameController GameController { get; set; }
        [Inject] public IAssetController AssetController { get; set; }
        [Inject] public IEventDispatcher Dispatcher { get; set; }
        [Inject] public IMovementModelPool MovementModelPool { get; set; }
        [Inject] public IUFOViewPool UFOViewPool { get; set; }
        [Inject] public IScoreController ScoreController { get; set; }
        [Inject] public IGameFieldController GameFieldController { get; set; }

        private UFOConfigScriptableObject _ufoConfig;

        private Dictionary<UFOView, MovementModel> _ufos;

        private MovementModel _playerMovementModel;

        private float _spawnTimer;

        [PostCreate]
        public void PostCreate()
        {
            _ufoConfig = AssetController.GetScriptableObject<UFOConfigScriptableObject>(Constants.UFOConfigPath);

            _ufos = new Dictionary<UFOView, MovementModel>(_ufoConfig.MaxCount);

            Dispatcher.AddListener(GameEvents.OnGameStateChanged, OnGameStateChanged);
            Dispatcher.AddListener(WeaponEvents.OnBulletHit, OnHit);
            Dispatcher.AddListener(WeaponEvents.OnLaserHit, OnHit);

            Dispatcher.AddListener(PlayerEvents.OnPlayerModelsInitialized, OnPlayerModelsInitialized);

            GameController.Subscribe(this, GameState.Game);
        }

        [OnDestroy]
        public void OnDestroy()
        {
            Dispatcher.RemoveListener(GameEvents.OnGameStateChanged, OnGameStateChanged);
            Dispatcher.RemoveListener(WeaponEvents.OnBulletHit, OnHit);
            Dispatcher.RemoveListener(WeaponEvents.OnLaserHit, OnHit);

            Dispatcher.RemoveListener(PlayerEvents.OnPlayerModelsInitialized, OnPlayerModelsInitialized);

            GameController.Unsubscribe(this, GameState.Game);
        }

        private void OnPlayerModelsInitialized(PlayerModel playerModel, MovementModel movementModel)
        {
            _playerMovementModel = movementModel;
        }

        private void OnHit(IEnemyView enemy)
        {
            if (enemy is UFOView ufoView)
                DestroyUFO(ufoView);
        }

        private void DestroyUFO(UFOView ufoView)
        {
            ufoView.Hide();
            MovementModelPool.Release(_ufos[ufoView], GetType());
            UFOViewPool.Release(ufoView, GetType());
            _ufos.Remove(ufoView);
            
            ScoreController.AddScore(_ufoConfig.Points);
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

                case GameState.MainMenu:
                case GameState.Exit:
                case GameState.Restart:
                    RemoveAllUFOs();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void Update(float deltaTime)
        {
            if (_ufos.Count < _ufoConfig.MaxCount)
            {
                if (_spawnTimer <= 0)
                {
                    TrySpawnUFO();
                    _spawnTimer = _ufoConfig.Cooldown;
                }
                else
                {
                    _spawnTimer -= deltaTime;
                }
            }

            foreach (var ufo in _ufos)
            {
                ufo.Value.Direction.SetValue((_playerMovementModel.Position.GetValue() - ufo.Value.Position.GetValue()).normalized * _ufoConfig.MaxSpeed);
                ufo.Value.Position.SetValue(ufo.Value.Position.GetValue() + ufo.Value.Direction.GetValue() * deltaTime);
            }
        }

        private void TrySpawnUFO()
        {
            var ufoView = UFOViewPool.Get(GetType());
            var movementModel = MovementModelPool.Get(GetType());

            var minCorner = GameFieldController.MinimumCorner;
            var maxCorner = GameFieldController.MaximumCorner;
            movementModel.Position.SetValue(new Vector2(Random.Range(minCorner.x, maxCorner.x), Random.Range(minCorner.y, maxCorner.y)));

            ufoView.SetModel(movementModel);
            _ufos.Add(ufoView, movementModel);

            ufoView.Show();
        }

        private void RemoveAllUFOs()
        {
            foreach (var ufo in _ufos)
            {
                ufo.Key.Hide();
                MovementModelPool.Release(ufo.Value, GetType());
                UFOViewPool.Release(ufo.Key, GetType());
            }

            _ufos.Clear();

            _spawnTimer = _ufoConfig.Cooldown;
        }
    }
}