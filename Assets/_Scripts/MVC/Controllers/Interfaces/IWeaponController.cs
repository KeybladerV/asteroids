using UnityEngine;

namespace MVC.Controllers.Interfaces
{
    public interface IWeaponController
    {
        void ShootBullet(Vector2 spawnPoint, float angle);
        void ShootLaser(Vector2 startPos, float distance, float angle);
    }
}