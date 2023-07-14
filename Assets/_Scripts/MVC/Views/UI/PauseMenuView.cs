using System;
using MVC.Views.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace MVC.Views.UI
{
    public class PauseMenuView : MonoBehaviour, IView
    {
        public event Action OnContinueButtonClickEvent;
        public event Action OnRestartButtonClickEvent;
        public event Action OnToMainMenuButtonClickEvent;
        public event Action OnExitButtonClickEvent;
        
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _toMainMenuButton;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            _continueButton.onClick.AddListener(OnContinueButtonClick);
            _restartButton.onClick.AddListener(OnRestartButtonClick);
            _toMainMenuButton.onClick.AddListener(OnToMainMenuButtonClick);
            _exitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void OnDestroy()
        {
            _continueButton.onClick.RemoveListener(OnContinueButtonClick);
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
        
        private void OnContinueButtonClick()
        {
            OnContinueButtonClickEvent?.Invoke();
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
    }
}