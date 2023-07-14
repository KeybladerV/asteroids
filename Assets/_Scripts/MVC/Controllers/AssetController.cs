using System;
using System.Collections.Generic;
using MVC.Controllers.Interfaces;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = System.Object;

namespace MVC.Controllers
{
    public sealed class AssetController : IAssetController
    {
        private Dictionary<string, Object> _keysAssets = new();
        
        public T GetComponentFromAsset<T>(string key) where T : Component
        {
            if(_keysAssets.TryGetValue(key, out var asset))
                return ((GameObject)asset).GetComponent<T>();

            return GetGameObjectAsset(key).GetComponent<T>();
        }

        public T GetScriptableObject<T>(string key) where T : ScriptableObject
        {
            if(_keysAssets.TryGetValue(key, out var asset))
                return (T)asset;

            var assetSync = Addressables.LoadAssetAsync<ScriptableObject>(key).WaitForCompletion();
            _keysAssets.Add(key, assetSync);
            return (T)assetSync;
        }

        public GameObject GetGameObjectAsset(string key)
        {
            var asset = Addressables.LoadAssetAsync<GameObject>(key).WaitForCompletion();
            _keysAssets.Add(key, asset);
            return asset;
        }

        public AsyncOperationHandle GetAssetAsync(string key)
        {
            throw new NotImplementedException();
        }
    }
}