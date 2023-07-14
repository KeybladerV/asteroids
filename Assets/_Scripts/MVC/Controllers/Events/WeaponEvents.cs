using Asteroid.Utility.Observer;
using MVC.Views.Interfaces;

namespace MVC.Controllers.Events
{
    public static class WeaponEvents
    {
        public static readonly Event<IEnemyView> OnBulletHit = new();
        public static readonly Event<IEnemyView> OnLaserHit = new();
    }
}