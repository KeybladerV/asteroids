using MVC.Models;
using MVC.Views.Interfaces;
using UnityEngine;

namespace MVC.Views
{
    public class UFOView : MonoBehaviour, IEnemyView
    {
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
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
        }

        private void OnPositionChanged(Vector2 value)
        {
            _transform.position = value;
        }
    }
}