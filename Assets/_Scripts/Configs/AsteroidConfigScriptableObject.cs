using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "AsteroidConfig", menuName = "AsteroidsConfigs/AsteroidConfig", order = 2)]
    public class AsteroidConfigScriptableObject : ScriptableObject
    {
        public float MaxSpeed;
        public int MaxCount;
        public float Cooldown;
        public int Points;
    }
}