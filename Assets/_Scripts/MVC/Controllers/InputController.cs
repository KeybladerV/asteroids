using System;
using Asteroid.Utility;
using Asteroid.Utility.Observer;
using MVC.Controllers.Enums;
using MVC.Controllers.Events;
using MVC.Controllers.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MVC.Controllers
{
    public sealed class InputController : IInputController, IUpdatable
    {
        [Inject] public IEventDispatcher Dispatcher { get; set; }
        [Inject] public IGameController GameController { get; set; }
        
        private MainControlls _mainControls;
        
        public MainControlls MainControls => _mainControls;
        
        [PostCreate]
        public void OnPostCreate()
        {
            _mainControls = new MainControlls();
            
            Dispatcher.AddListener(GameEvents.OnGameStateChanged, OnGameStateChange);
            
            GameController.Subscribe(this, GameState.Game);
            
            _mainControls.Base.laser.performed += OnLaserPerformed;
            _mainControls.Base.movement.canceled += OnMovementCanceled;
            _mainControls.UI.pause.performed += OnPausePerformed;
        }

        [OnDestroy]
        public void OnDestroy()
        {
            Dispatcher.RemoveListener(GameEvents.OnGameStateChanged, OnGameStateChange);
            
            _mainControls.Base.laser.performed -= OnLaserPerformed;
            _mainControls.UI.pause.performed -= OnPausePerformed;
        }

        public void Update(float deltaTime)
        {
            if(_mainControls.Base.movement.inProgress)
                Dispatcher.Dispatch(InputEvents.OnMovement, _mainControls.Base.movement.ReadValue<Vector2>());
            if(_mainControls.Base.shoot.inProgress)
                Dispatcher.Dispatch(InputEvents.OnAttack1);
        }

        private void OnGameStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    _mainControls.Disable();
                    break;
                
                case GameState.Game:
                    _mainControls.Enable();
                    break;
                
                case GameState.GameOver:
                    _mainControls.Disable();
                    break;
                
                case GameState.Pause:
                    _mainControls.Base.Disable();
                    _mainControls.UI.Enable();
                    break;

                case GameState.Exit:
                    _mainControls.Disable();
                    break;
                
                case GameState.Restart:
                    _mainControls.Disable();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        private void OnPausePerformed(InputAction.CallbackContext ctx) => Dispatcher.Dispatch(InputEvents.OnPause);
        private void OnLaserPerformed(InputAction.CallbackContext ctx) => Dispatcher.Dispatch(InputEvents.OnAttack2);
        private void OnMovementCanceled(InputAction.CallbackContext ctx) => Dispatcher.Dispatch(InputEvents.OnMovementCancel);
    }
}