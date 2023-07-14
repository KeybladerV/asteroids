using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "AsteroidsConfigs/PlayerConfig", order = 1)]
    public class PlayerConfigScriptableObject : ScriptableObject
    {
        public float MaxSpeed;
        public float Acceleration;
        public float RotationSpeed;
        public float Inertia;
        public float BulletCooldown;
        public int StartLaserCount;
        public int MaxLaserCount;
        public float LaserCooldown;
    }
}