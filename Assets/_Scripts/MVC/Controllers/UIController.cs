using System;
using System.Collections.Generic;
using Asteroid.Utility;
using Asteroid.Utility.Observer;
using Configs;
using MVC.Controllers.Enums;
using MVC.Controllers.Events;
using MVC.Controllers.Interfaces;
using MVC.Models;
using MVC.Views.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MVC.Controllers
{
    public sealed class UIController : IUIController
    {
        [Inject] public IEventDispatcher Dispatcher { get; set; }
        [Inject] public IGameController GameController { get; set; }
        [Inject] public IAssetController AssetController { get; set; }

        private Dictionary<CanvasType, Canvas> _canvasMap = new();

        private MainMenuView _mainMenuViewAsset;
        private MainMenuView _mainMenuView;

        private GUIView _guiViewAsset;
        private GUIView _guiView;

        private GameOverMenuView _gameOverMenuViewAsset;
        private GameOverMenuView _gameOverMenuView;

        private PauseMenuView _pauseMenuViewAsset;
        private PauseMenuView _pauseMenuView;

        [PostCreate]
        public void OnPostCreate()
        {
            _mainMenuViewAsset = AssetController.GetComponentFromAsset<MainMenuView>(Constants.MainMenuViewPath);
            _guiViewAsset = AssetController.GetComponentFromAsset<GUIView>(Constants.GUIViewPath);
            _gameOverMenuViewAsset =
                AssetController.GetComponentFromAsset<GameOverMenuView>(Constants.GameOverMenuViewPath);
            _pauseMenuViewAsset = AssetController.GetComponentFromAsset<PauseMenuView>(Constants.PauseMenuViewPath);

            Dispatcher.AddListener(GameEvents.OnGameStateChanged, OnGameStateChange);
            Dispatcher.AddListener(PlayerEvents.OnPlayerModelsInitialized, OnPlayerModelInitialized);
            Dispatcher.AddListener(ScoreEvents.OnScoreChanged, OnScoreChanged);
        }

        private void OnScoreChanged(int score)
        {
            _guiView.SetScore(score);
            _gameOverMenuView.SetScore(score);
        }

        [OnDestroy]
        public void OnDestroy()
        {
            _mainMenuView.OnStartButtonClickEvent -= OnStartButtonClick;
            _mainMenuView.OnExitButtonClickEvent -= OnExitButtonClick;

            _gameOverMenuView.OnRestartButtonClickEvent -= OnRestartButtonClick;
            _gameOverMenuView.OnExitButtonClickEvent -= OnExitButtonClick;
            _gameOverMenuView.OnToMainMenuButtonClickEvent -= OnToMainMenuButtonClick;

            _pauseMenuView.OnContinueButtonClickEvent -= OnContinueButtonClick;
            _pauseMenuView.OnExitButtonClickEvent -= OnExitButtonClick;
            _pauseMenuView.OnToMainMenuButtonClickEvent -= OnToMainMenuButtonClick;
            _pauseMenuView.OnRestartButtonClickEvent -= OnRestartButtonClick;

            Dispatcher.RemoveListener(GameEvents.OnGameStateChanged, OnGameStateChange);
            Dispatcher.RemoveListener(PlayerEvents.OnPlayerModelsInitialized, OnPlayerModelInitialized);
        }

        private void OnPlayerModelInitialized(PlayerModel playerModel, MovementModel movementModel)
        {
            _guiView.SetModels(playerModel, movementModel);
        }

        public IUIController RegisterCanvas(CanvasType type, Canvas canvas)
        {
            _canvasMap.Add(type, canvas);
            return this;
        }

        private void OnStartButtonClick() => GameController.ChangeGameState(GameState.Game);
        private void OnExitButtonClick() => GameController.ChangeGameState(GameState.Exit);
        private void OnRestartButtonClick() => GameController.ChangeGameState(GameState.Restart);
        private void OnToMainMenuButtonClick() => GameController.ChangeGameState(GameState.MainMenu);
        private void OnContinueButtonClick() => GameController.ChangeGameState(GameState.Game);
        private void OnPauseButtonClick() => GameController.ChangeGameState(GameState.Pause);

        private void OnGameStateChange(GameState state)
        {
            SetUIState(state);
        }

        private void SetUIState(GameState currentState)
        {
            switch (currentState)
            {
                case GameState.MainMenu:
                    _mainMenuView.Show();
                    _guiView.Hide();
                    _gameOverMenuView.Hide();
                    _pauseMenuView.Hide();
                    break;

                case GameState.Game:
                    _mainMenuView.Hide();
                    _guiView.Show();
                    _gameOverMenuView.Hide();
                    _pauseMenuView.Hide();
                    break;

                case GameState.GameOver:
                    _mainMenuView.Hide();
                    _guiView.Hide();
                    _gameOverMenuView.Show();
                    _pauseMenuView.Hide();
                    break;

                case GameState.Pause:
                    _mainMenuView.Hide();
                    _guiView.Hide();
                    _gameOverMenuView.Hide();
                    _pauseMenuView.Show();
                    break;

                case GameState.None:
                case GameState.Restart:
                case GameState.Exit:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null);
            }
        }

        public void PrepareAllUI()
        {
            _mainMenuView = Object.Instantiate(_mainMenuViewAsset, _canvasMap[CanvasType.Menu].transform);
            _mainMenuView.OnStartButtonClickEvent += OnStartButtonClick;
            _mainMenuView.OnExitButtonClickEvent += OnExitButtonClick;
            _mainMenuView.Hide();
            
            _guiView = Object.Instantiate(_guiViewAsset, _canvasMap[CanvasType.GUI].transform);
            _guiView.OnPauseButtonClickEvent += OnPauseButtonClick;
            _guiView.Hide();
            
            _gameOverMenuView = Object.Instantiate(_gameOverMenuViewAsset, _canvasMap[CanvasType.Menu].transform);
            _gameOverMenuView.OnRestartButtonClickEvent += OnRestartButtonClick;
            _gameOverMenuView.OnExitButtonClickEvent += OnExitButtonClick;
            _gameOverMenuView.OnToMainMenuButtonClickEvent += OnToMainMenuButtonClick;

            _pauseMenuView = Object.Instantiate(_pauseMenuViewAsset, _canvasMap[CanvasType.Menu].transform);
            _pauseMenuView.OnContinueButtonClickEvent += OnContinueButtonClick;
            _pauseMenuView.OnExitButtonClickEvent += OnExitButtonClick;
            _pauseMenuView.OnToMainMenuButtonClickEvent += OnToMainMenuButtonClick;
            _pauseMenuView.OnRestartButtonClickEvent += OnRestartButtonClick;
        }
    }
}