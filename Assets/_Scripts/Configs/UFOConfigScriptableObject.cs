using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "UFOConfig", menuName = "AsteroidsConfigs/UFOConfig", order = 4)]
    public class UFOConfigScriptableObject : ScriptableObject
    {
        public float MaxSpeed;
        public int MaxCount;
        public float Cooldown;
        public int Points;
    }
}