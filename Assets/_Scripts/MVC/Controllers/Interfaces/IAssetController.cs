using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MVC.Controllers.Interfaces
{
    public interface IAssetController
    {
        T GetComponentFromAsset<T>(string key) where T : Component;
        T GetScriptableObject<T>(string key) where T : ScriptableObject;
        GameObject GetGameObjectAsset(string key);
        AsyncOperationHandle GetAssetAsync(string key);
    }
}