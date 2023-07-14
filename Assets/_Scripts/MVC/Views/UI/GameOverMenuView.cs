using System;
using MVC.Views.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVC.Views.UI
{
    public class GameOverMenuView : MonoBehaviour, IView
    {
        public event Action OnRestartButtonClickEvent;
        public event Action OnToMainMenuButtonClickEvent;
        public event Action OnExitButtonClickEvent;
        
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _toMainMenuButton;
        [SerializeField] private Button _exitButton;
        
        [SerializeField] private TMP_Text _scoreText;

        private void Awake()
        {
            _restartButton.onClick.AddListener(OnRestartButtonClick);
            _toMainMenuButton.onClick.AddListener(OnToMainMenuButtonClick);
            _exitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void OnDestroy()
        {
            _restartButton.onClick.RemoveListener(OnRestartButtonClick);
            _toMainMenuButton.onClick.RemoveListener(OnToMainMenuButtonClick);
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void OnRestartButtonClick()
        {
            OnRestartButtonClickEvent?.Invoke();
        }
        
        private void OnToMainMenuButtonClick()
        {
            OnToMainMenuButtonClickEvent?.Invoke();
        }
        
        private void OnExitButtonClick()
        {
            OnExitButtonClickEvent?.Invoke();
        }

        public void SetScore(int score)
        {
            _scoreText.text = score.ToString();
        }
    }
}