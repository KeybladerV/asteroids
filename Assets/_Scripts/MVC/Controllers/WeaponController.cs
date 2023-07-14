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
using Object = UnityEngine.Object;

namespace MVC.Controllers
{
    public struct BulletSpawnGroup
    {
        public BulletView BulletView;
        public MovementModel MovementModel;
    }

    public sealed class WeaponController : IWeaponController, IUpdatable
    {
        [Inject] public IGameController GameController { get; set; }
        [Inject] public IBulletViewPool BulletViewPool { get; set; }
        [Inject] public IMovementModelPool MovementModelPool { get; set; }
        [Inject] public IGameFieldController GameFieldController { get; set; }
        [Inject] public IAssetController AssetController { get; set; }
        [Inject] public IEventDispatcher Dispatcher { get; set; }

        private BulletConfigScriptableObject _bulletConfig;

        private Dictionary<BulletView, MovementModel> _bullets = new();
        private Queue<BulletSpawnGroup> _bulletsToSpawn = new();
        private Queue<BulletView> _bulletsToRemove = new();

        private LaserView _laserView;

        [PostCreate]
        public void OnPostCreate()
        {
            _bulletConfig =
                AssetController.GetScriptableObject<BulletConfigScriptableObject>(Constants.BulletConfigPath);

            var laserAsset = AssetController.GetComponentFromAsset<LaserView>(Constants.LaserAssetPath);
            _laserView = Object.Instantiate(laserAsset);
            _laserView.Hide();

            Dispatcher.AddListener(GameEvents.OnGameStateChanged, OnGameStateChanged);
            GameController.Subscribe(this, GameState.Game);
        }

        [OnDestroy]
        public void OnDestroy()
        {
            Dispatcher.RemoveListener(GameEvents.OnGameStateChanged, OnGameStateChanged);
            GameController.Unsubscribe(this, GameState.Game);
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
                case GameState.Restart:
                case GameState.Exit:
                    ResetAll();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void Update(float deltaTime)
        {
            ProcessViewsToSpawn();

            foreach (var bulletPair in _bullets)
            {
                if (!GameFieldController.IsInGameField(bulletPair.Value.Position.GetValue()))
                    RemoveBullet(bulletPair.Key);

                bulletPair.Value.Position.SetValue(bulletPair.Value.Position.GetValue() +
                                                   bulletPair.Value.Direction.GetValue() * deltaTime);
            }

            ProcessViewsToRemove();
        }

        public void ShootBullet(Vector2 spawnPoint, float angle)
        {
            var bulletView = BulletViewPool.Get(GetType());
            var movementModel = MovementModelPool.Get(GetType());
            movementModel.Position.SetValue(spawnPoint);
            movementModel.Angle.SetValue(angle);
            movementModel.Direction.SetValue(
                new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) * _bulletConfig.Speed);

            bulletView.SetModel(movementModel);
            bulletView.OnTriggeredEnemy += OnBulletTriggeredEnemy;

            _bulletsToSpawn.Enqueue(new BulletSpawnGroup
            {
                BulletView = bulletView,
                MovementModel = movementModel
            });
        }

        public void ShootLaser(Vector2 startPos, float distance, float angle)
        {
            var direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
            var hits = Physics2D.RaycastAll(startPos, direction, distance);

            foreach (var hit in hits)
            {
                var enemyView = hit.collider.GetComponentInParent<IEnemyView>();
                if (enemyView != null)
                    Dispatcher.Dispatch(WeaponEvents.OnLaserHit, enemyView);
            }

            _laserView.Show(startPos, direction * distance, 0.1f);
        }

        private void OnBulletTriggeredEnemy(BulletView bulletView, IEnemyView enemyView)
        {
            RemoveBullet(bulletView);
            Dispatcher.Dispatch(WeaponEvents.OnBulletHit, enemyView);
        }

        private void RemoveBullet(BulletView view)
        {
            view.OnTriggeredEnemy -= OnBulletTriggeredEnemy;

            _bulletsToRemove.Enqueue(view);
        }

        private void ProcessViewsToSpawn()
        {
            while (_bulletsToSpawn.Count > 0)
            {
                var bulletSpawnGroup = _bulletsToSpawn.Dequeue();
                _bullets.Add(bulletSpawnGroup.BulletView, bulletSpawnGroup.MovementModel);
                bulletSpawnGroup.BulletView.Show();
            }
        }

        private void ProcessViewsToRemove()
        {
            while (_bulletsToRemove.Count > 0)
            {
                var bulletView = _bulletsToRemove.Dequeue();
                MovementModelPool.Release(_bullets[bulletView], GetType());
                BulletViewPool.Release(bulletView, GetType());
                _bullets.Remove(bulletView);
            }
        }

        private void ResetAll()
        {
            foreach (var bulletPair in _bullets)
            {
                bulletPair.Key.OnTriggeredEnemy -= OnBulletTriggeredEnemy;
                MovementModelPool.Release(bulletPair.Value, GetType());
                BulletViewPool.Release(bulletPair.Key, GetType());
            }

            while (_bulletsToSpawn.Count > 0)
            {
                var bulletSpawnGroup = _bulletsToSpawn.Dequeue();
                MovementModelPool.Release(bulletSpawnGroup.MovementModel, GetType());
                BulletViewPool.Release(bulletSpawnGroup.BulletView, GetType());
            }

            while (_bulletsToRemove.Count > 0)
            {
                var bulletView = _bulletsToRemove.Dequeue();
                MovementModelPool.Release(_bullets[bulletView], GetType());
                BulletViewPool.Release(bulletView, GetType());
            }

            _bullets.Clear();
            _bulletsToSpawn.Clear();
            _bulletsToRemove.Clear();
            
            _laserView.Hide();
        }
    }
}