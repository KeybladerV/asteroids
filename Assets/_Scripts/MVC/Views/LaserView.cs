using System.Collections;
using MVC.Views.Interfaces;
using UnityEngine;

namespace MVC.Views
{
    public class LaserView : MonoBehaviour, IWeaponView
    {
        [SerializeField] private LineRenderer _lineRenderer;
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(Vector2 start, Vector2 direction, float time)
        {
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, start + direction);
            Show();
            StartCoroutine(HideAfterTime(time));
        }
        
        private IEnumerator HideAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            Hide();
        }
    }
}