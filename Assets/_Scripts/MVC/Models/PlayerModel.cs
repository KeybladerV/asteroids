using Utility;

namespace MVC.Models
{
    public class PlayerModel
    {
        public readonly ReactiveField<int> LaserCount = new();
        public readonly ReactiveField<float> CurrentLaserCooldown = new();
        
        public float CurrentBulletCooldown;
    }
}