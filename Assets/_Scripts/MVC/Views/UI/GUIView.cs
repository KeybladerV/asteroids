using System;
using MVC.Models;
using MVC.Views.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVC.Views.UI
{
    public class GUIView : MonoBehaviour, IView
    {
        public event Action OnPauseButtonClickEvent;
        
        [SerializeField] private TMP_Text _scoreText;
        
        [SerializeField] private TMP_Text _coordinatsText;
        [SerializeField] private TMP_Text _angleText;
        [SerializeField] private TMP_Text _speedText;
        
        [SerializeField] private TMP_Text _laserCountText;
        [SerializeField] private TMP_Text _laserCooldownText;
        
        [SerializeField] private Button _pauseButton;
        
        private void Awake()
        {
            _pauseButton.onClick.AddListener(OnPauseButtonClick);
        }

        private void OnDestroy()
        {
            _pauseButton.onClick.RemoveListener(OnPauseButtonClick);
        }
        
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void OnPauseButtonClick() => OnPauseButtonClickEvent?.Invoke();

        public void SetModels(PlayerModel playerModel, MovementModel movementModel)
        {
            playerModel.LaserCount.OnValueChangedEvent += OnLaserCountChanged;
            playerModel.CurrentLaserCooldown.OnValueChangedEvent += OnLaserCooldownChanged;
            
            movementModel.Angle.OnValueChangedEvent += OnAngleChanged;
            movementModel.Position.OnValueChangedEvent += OnPositionChanged;
            movementModel.Direction.OnValueChangedEvent += OnDirectionChanged;
        }

        private void OnDirectionChanged(Vector2 value)
        {
            _speedText.text = value.magnitude.ToString("F1");
        }

        private void OnPositionChanged(Vector2 value)
        {
            _coordinatsText.text = value.ToString("F1");
        }

        private void OnAngleChanged(float value)
        {
            _angleText.text = value.ToString("F1");
        }

        private void OnLaserCountChanged(int value)
        {
            _laserCountText.text = value.ToString();
        }
        
        private void OnLaserCooldownChanged(float value)
        {
            _laserCooldownText.text = value.ToString("F1");
        }

        public void SetScore(int score)
        {
            _scoreText.text = score.ToString();
        }
    }
}