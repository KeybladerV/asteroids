using System;
using MVC.Views.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace MVC.Views.UI
{
    public class MainMenuView : MonoBehaviour, IView
    {
        public event Action OnStartButtonClickEvent;
        public event Action OnExitButtonClickEvent;

        [SerializeField] private Button _startButton;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            _startButton.onClick.AddListener(OnStartButtonClick);
            _exitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void OnDestroy()
        {
            _startButton.onClick.RemoveListener(OnStartButtonClick);
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
        
        private void OnStartButtonClick() => OnStartButtonClickEvent?.Invoke();
        private void OnExitButtonClick() => OnExitButtonClickEvent?.Invoke();
    }
}
