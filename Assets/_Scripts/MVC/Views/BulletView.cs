using System;
using MVC.Models;
using MVC.Views.Interfaces;
using UnityEngine;

namespace MVC.Views
{
    public class BulletView : MonoBehaviour, IWeaponView
    {
        private Transform _transform;

        public event Action<BulletView, IEnemyView> OnTriggeredEnemy;

        private void Awake()
        {
            _transform = transform;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponentInParent<IEnemyView>();
            if(enemy != null)
                OnTriggeredEnemy?.Invoke(this, enemy);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetModel(MovementModel movementModel)
        {
            movementModel.Position.OnValueChangedEvent += OnPositionChanged;
            movementModel.Angle.OnValueChangedEvent += OnAngleChanged;
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