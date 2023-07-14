using System;
using MVC.Models;
using MVC.Views.Interfaces;
using UnityEngine;

namespace MVC.Views
{
    public class PlayerView : MonoBehaviour, IView
    {
        [SerializeField] private Transform _bulletSpawnPoint;
        
        private Transform _transform;

        public event Action<IEnemyView> OnTriggeredEnemy;
        
        public Vector2 ShootingPoint => _bulletSpawnPoint.position;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponentInParent<IEnemyView>();
            if(enemy != null)
                OnTriggeredEnemy?.Invoke(enemy);
        }

        public void SetModels(MovementModel movementModel, PlayerModel playerModel)
        {
            movementModel.Position.OnValueChangedEvent += OnPositionChanged;
            movementModel.Angle.OnValueChangedEvent += OnAngleChanged;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnAngleChanged(float value)
        {
            _transform.rotation = Quaternion.Euler(0, 0, -value);
        }

        private void OnPositionChanged(Vector2 value)
        {
            _transform.position = value;
        }
    }
}