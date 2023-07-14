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
    public sealed class AsteroidsController : IAsteroidsController
    {
        [Inject] public IGameController GameController { get; set; }
        [Inject] public IAssetController AssetController { get; set; }
        [Inject] public IAsteroidViewPool AsteroidViewPool { get; set; }
        [Inject] public IMovementModelPool MovementModelPool { get; set; }
        [Inject] public IAsteroidModelPool AsteroidModelPool { get; set; }
        [Inject] public IScoreController ScoreController { get; set; }
        [Inject] public IGameFieldController GameFieldController { get; set; }
        [Inject] public IEventDispatcher Dispatcher { get; set; }
        
        private AsteroidConfigScriptableObject _smallAsteroidConfig;
        private AsteroidConfigScriptableObject _bigAsteroidConfig;
        
        private Dictionary<AsteroidView, AsteroidModelGroup> _asteroids;
        private HashSet<MovementModel> _asteroidsMovements;

        private int _bigAsteroidsCount;
        private int _smallAsteroidsCount;

        private float _bigAsteroidTimer;
        private float _smallAsteroidTimer;

        [PostCreate]
        public void OnPostCreate()
        {
            _smallAsteroidConfig = AssetController.GetScriptableObject<AsteroidConfigScriptableObject>(Constants.SmallAsteroidConfigPath);
            _bigAsteroidConfig = AssetController.GetScriptableObject<AsteroidConfigScriptableObject>(Constants.BigAsteroidConfigPath);
            
            _asteroids = new Dictionary<AsteroidView, AsteroidModelGroup>(_smallAsteroidConfig.MaxCount + _bigAsteroidConfig.MaxCount);
            _asteroidsMovements = new HashSet<MovementModel>(_smallAsteroidConfig.MaxCount + _bigAsteroidConfig.MaxCount);
            
            Dispatcher.AddListener(GameEvents.OnGameStateChanged, OnGameStateChanged);
            Dispatcher.AddListener(WeaponEvents.OnBulletHit, OnHit);
            Dispatcher.AddListener(WeaponEvents.OnLaserHit, OnHit);
            
            GameController.Subscribe(this, GameState.Game);
        }

        [OnDestroy]
        public void OnDestroy()
        {
            RemoveAllAsteroids();
            Dispatcher.RemoveListener(GameEvents.OnGameStateChanged, OnGameStateChanged);
            Dispatcher.RemoveListener(WeaponEvents.OnBulletHit, OnHit);
            Dispatcher.RemoveListener(WeaponEvents.OnLaserHit, OnHit);
            GameController.Unsubscribe(this, GameState.Game);
        }
        
        private void OnHit(IEnemyView enemy)
        {
            if(enemy is AsteroidView asteroidView)
                DestroyAsteroid(asteroidView);
        }

        public void Update(float deltaTime)
        {
            if (_bigAsteroidsCount < _bigAsteroidConfig.MaxCount)
            {
                if (_bigAsteroidTimer <= 0)
                {
                    var asteroid = CreateAsteroid(true);
                    asteroid.Show();
                    _bigAsteroidsCount++;
                    _bigAsteroidTimer = _bigAsteroidConfig.Cooldown;
                }
                
                _bigAsteroidTimer -= deltaTime;
            }
            
            if(_smallAsteroidsCount < _smallAsteroidConfig.MaxCount)
            {
                if (_smallAsteroidTimer <= 0)
                {
                    var asteroid = CreateAsteroid(false);
                    asteroid.Show();
                    _smallAsteroidsCount++;
                    _smallAsteroidTimer = _smallAsteroidConfig.Cooldown;
                }
                
                _smallAsteroidTimer -= deltaTime;
            }
            
            foreach (var model in _asteroidsMovements)
            {
                var position = model.Position.GetValue();
                model.Position.SetValue(position + model.Direction.GetValue() * deltaTime);
            }
        }

        private AsteroidView CreateAsteroid(bool isBig, bool random = true, Vector2 position = default, float speed = default)
        {
            var config = isBig ? _bigAsteroidConfig : _smallAsteroidConfig;
            
            var asteroidModel = AsteroidModelPool.Get(GetType());
            asteroidModel.IsBig = isBig;

            var movementModel = MovementModelPool.Get(GetType());

            var minCorner = GameFieldController.MinimumCorner;
            var maxCorner = GameFieldController.MaximumCorner;
            if (random)
                position = new Vector2(Random.Range(minCorner.x, maxCorner.x), Random.Range(minCorner.y, maxCorner.y));
            speed = Random.Range(random ? 0 : speed, config.MaxSpeed);

            movementModel.Position.SetValue(position);
            movementModel.Direction.SetValue(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * speed);
            
            var asteroidView = AsteroidViewPool.Get(GetType());
            asteroidView.SetModels(movementModel, asteroidModel);
            
            _asteroids.Add(asteroidView, new AsteroidModelGroup(){Model = asteroidModel, MovementModel = movementModel});
            _asteroidsMovements.Add(movementModel);

            return asteroidView;
        }

        private void DestroyAsteroid(AsteroidView view)
        {
            view.Hide();
            var modelGroup = _asteroids[view];
            var isBig = modelGroup.Model.IsBig;
            var pos = modelGroup.MovementModel.Position.GetValue();
            var speed = modelGroup.MovementModel.Direction.GetValue().magnitude;

            _asteroids.Remove(view);
            
            AsteroidModelPool.Release(modelGroup.Model, GetType());
            
            _asteroidsMovements.Remove(modelGroup.MovementModel);
            MovementModelPool.Release(modelGroup.MovementModel, GetType());
            
            AsteroidViewPool.Release(view, GetType());

            if (isBig)
            {
                _bigAsteroidsCount--;
                var smallCount = Random.Range(1, 4);
                for(var i = 0; i < smallCount; i++)
                {
                    var asteroid = CreateAsteroid(false, false, pos, speed);
                    asteroid.Show();
                    _smallAsteroidsCount++;
                }
            }
            else
            {
                _smallAsteroidsCount--;
            }

            ScoreController.AddScore(isBig ? _bigAsteroidConfig.Points : _smallAsteroidConfig.Points);
        }
        
        private void RemoveAllAsteroids()
        {
            foreach (var view in _asteroids.Keys)
            {
                if(view == null)
                    continue;
                
                view.Hide();
                var modelGroup = _asteroids[view];
                
                AsteroidModelPool.Release(modelGroup.Model, GetType());
                
                _asteroidsMovements.Remove(modelGroup.MovementModel);
                MovementModelPool.Release(modelGroup.MovementModel, GetType());
                
                AsteroidViewPool.Release(view, GetType());
            }
            
            _asteroids.Clear();
            _bigAsteroidsCount = 0;
            _smallAsteroidsCount = 0;
            _bigAsteroidTimer = _bigAsteroidConfig.Cooldown;
            _smallAsteroidTimer = _smallAsteroidConfig.Cooldown;
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.None:
                    break;
                
                case GameState.Game:
                    break;
                
                case GameState.GameOver:
                    break;
                
                case GameState.Pause:
                    break;
                
                case GameState.MainMenu:
                case GameState.Exit:
                case GameState.Restart:
                    RemoveAllAsteroids();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}