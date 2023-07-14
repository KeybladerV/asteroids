using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "BulletConfig", menuName = "AsteroidsConfigs/BulletConfig", order = 3)]
    public class BulletConfigScriptableObject : ScriptableObject
    {
        public float Speed;
    }
}