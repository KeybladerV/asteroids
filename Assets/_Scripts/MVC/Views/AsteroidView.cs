using MVC.Models;
using MVC.Views.Interfaces;
using UnityEngine;

namespace MVC.Views
{
    public class AsteroidView : MonoBehaviour, IEnemyView
    {
        [SerializeField] private GameObject _bigAsteroid;
        [SerializeField] private GameObject _smallAsteroid;
        
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void SetModels(MovementModel movementModel, AsteroidModel model)
        {
            movementModel.Angle.OnValueChangedEvent += OnAngleChanged;
            movementModel.Position.OnValueChangedEvent += OnPositionChanged;
            
            _transform.position = movementModel.Position.GetValue();
            _transform.rotation = Quaternion.Euler(0, 0, -movementModel.Angle.GetValue());
            
            _bigAsteroid.SetActive(model.IsBig);
            _smallAsteroid.SetActive(!model.IsBig);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnPositionChanged(Vector2 value)
        {
            _transform.position = value;
        }

        private void OnAngleChanged(float value)
        {
            _transform.rotation = Quaternion.Euler(0, 0, -value);
        }
    }
}