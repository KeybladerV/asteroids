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
    public sealed class PlayerController : IPlayerController
    {
        [Inject] public IGameController GameController { get; set; }
        [Inject] public IAssetController AssetController { get; set; }
        [Inject] public IGameFieldController GameFieldController { get; set; }
        [Inject] public IWeaponController WeaponController { get; set; }
        [Inject] public IMovementModelPool MovementModelPool { get; set; }
        [Inject] public IEventDispatcher Dispatcher { get; set; }
        
        private PlayerView _playerViewAsset;
        private PlayerConfigScriptableObject _playerConfig;
        
        private PlayerModel _playerModel;
        private MovementModel _playerMovementModel;
        private PlayerView _playerView;
        
        private GameState _gameState;
        
        private Vector2 _movementInput;

        [PostCreate]
        public void OnPostCreate()
        {
            _playerViewAsset = AssetController.GetComponentFromAsset<PlayerView>(Constants.PlayerAssetPath);
            _playerConfig = AssetController.GetScriptableObject<PlayerConfigScriptableObject>(Constants.PlayerConfigPath);

            Dispatcher.AddListener(GameEvents.OnGameStateChanged, OnGameStateChange);
            
            Dispatcher.AddListener(InputEvents.OnMovement, OnMovementInput);
            Dispatcher.AddListener(InputEvents.OnMovementCancel, OnMovementCancel);
            Dispatcher.AddListener(InputEvents.OnAttack1, ShootBullet);
            Dispatcher.AddListener(InputEvents.OnAttack2, ShootLaser);
            
            GameController.Subscribe(this, GameState.Game);
        }

        [OnDestroy]
        public void OnDestroy()
        {
            Dispatcher.RemoveListener(GameEvents.OnGameStateChanged, OnGameStateChange);
            
            Dispatcher.RemoveListener(InputEvents.OnMovement, OnMovementInput);
            Dispatcher.RemoveListener(InputEvents.OnMovementCancel, OnMovementCancel);
            Dispatcher.RemoveListener(InputEvents.OnAttack1, ShootBullet);
            Dispatcher.RemoveListener(InputEvents.OnAttack2, ShootLaser);
        }

        private void CreatePlayerView()
        {
            _playerView = Object.Instantiate(_playerViewAsset).GetComponent<PlayerView>();
            _playerView.Hide();
            
            _playerModel = new PlayerModel();
            _playerMovementModel = MovementModelPool.Get(GetType());
            InitPlayerModel();
            Dispatcher.Dispatch(PlayerEvents.OnPlayerModelsInitialized, _playerModel, _playerMovementModel);
            
            _playerView.SetModels(_playerMovementModel, _playerModel);
            
            Dispatcher.Dispatch(PlayerEvents.OnPlayerSpawned);
            
            _playerView.OnTriggeredEnemy += OnPlayerTriggeredEnemy;
        }

        public void Update(float deltaTime)
        {
            ProcessMovement(deltaTime);
            
            _playerMovementModel.Direction.SetValue(Vector3.Lerp(_playerMovementModel.Direction.GetValue(), Vector2.zero, _playerConfig.Inertia * deltaTime));
            
            _playerMovementModel.Position.SetValue(_playerMovementModel.Position.GetValue() + _playerMovementModel.Direction.GetValue() * deltaTime);
            
            ProcessBulletCooldown(deltaTime);
            ProcessLaserReload(deltaTime);
        }

        private void ProcessBulletCooldown(float deltaTime)
        {
            if(_playerModel.CurrentBulletCooldown <= 0)
                return;
            _playerModel.CurrentBulletCooldown -= deltaTime;
        }

        private void ProcessLaserReload(float deltaTime)
        {
            if(_playerModel.LaserCount.GetValue() >= _playerConfig.MaxLaserCount)
                return;
            
            _playerModel.CurrentLaserCooldown.SetValue(_playerModel.CurrentLaserCooldown.GetValue() - deltaTime);

            if (!(_playerModel.CurrentLaserCooldown.GetValue() <= 0)) 
                return;
            
            _playerModel.CurrentLaserCooldown.SetValue(_playerConfig.LaserCooldown);
            _playerModel.LaserCount.SetValue(_playerModel.LaserCount.GetValue() + 1);
        }

        private void ProcessMovement(float deltaTime)
        {
            if(_movementInput == Vector2.zero)
                return;
            
            var movement = _movementInput * deltaTime;

            var angle = _playerMovementModel.Angle.GetValue();
            angle = (angle + movement.x * _playerConfig.RotationSpeed) % 360;
            if (angle > 180)
                angle -= 360;
            else if (angle < -180)
                angle += 360;
            _playerMovementModel.Angle.SetValue(angle);

            var acceleration = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad)) *
                               (movement.y * _playerConfig.Acceleration);
            _playerMovementModel.Direction.SetValue(
                Vector2.ClampMagnitude(_playerMovementModel.Direction.GetValue() + acceleration, _playerConfig.MaxSpeed));
        }
        
        private void InitPlayerModel()
        {
            _playerMovementModel.Position.SetValue(Vector2.zero);
            _playerMovementModel.Angle.SetValue(0);
            _playerMovementModel.Direction.SetValue(Vector2.zero);
            
            _playerModel.LaserCount.SetValue(_playerConfig.StartLaserCount);
            _playerModel.CurrentLaserCooldown.SetValue(_playerConfig.LaserCooldown);
        }

        private void OnGameStateChange(GameState state)
        {
            if (_gameState == GameState.MainMenu && state == GameState.Game || _gameState == GameState.Restart && state == GameState.Game)
            {
                if (_playerView == null)
                {
                    CreatePlayerView();
                    InitPlayerModel();
                    _playerView.Show();
                }
                else
                {
                    InitPlayerModel();
                    _playerView.Show();
                }
            }
            else if (_gameState == GameState.Game && state != GameState.Pause)
            {
                _playerView.Hide();
            }
            
            _gameState = state;
        }
        
        private void OnMovementInput(Vector2 input) => _movementInput = input;
        private void OnMovementCancel() => _movementInput = Vector2.zero;
        
        private void ShootLaser()
        {
            if(_playerModel.LaserCount.GetValue() <= 0)
                return;
            
            WeaponController.ShootLaser(_playerView.ShootingPoint,GameFieldController.DiagonalLength, _playerMovementModel.Angle.GetValue());
            _playerModel.LaserCount.SetValue(_playerModel.LaserCount.GetValue() - 1);
        }

        private void ShootBullet()
        {
            if(_playerModel.CurrentBulletCooldown > 0)
                return;

            _playerModel.CurrentBulletCooldown = _playerConfig.BulletCooldown;

            WeaponController.ShootBullet(_playerView.ShootingPoint, _playerMovementModel.Angle.GetValue());
        }

        private void OnPlayerTriggeredEnemy(IEnemyView enemyView)
        {
            _playerView.Hide();
            GameController.ChangeGameState(GameState.GameOver);
        }
    }
}
