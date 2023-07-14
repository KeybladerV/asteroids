using System;
using Asteroid.Utility;
using Configs;
using MVC.Controllers.Enums;
using MVC.Controllers.Interfaces;
using MVC.Controllers.Pools.Interfaces;
using MVC.Views;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace MVC.Controllers.Pools
{
    public class AsteroidViewViewPool : IAsteroidViewPool
    {
        [Inject] public IAssetController AssetController { get; set; }
        [Inject] public IGameController GameController { get; set; }
        
        private ObjectPool<AsteroidView> _viewPool;
        
        private AsteroidView _asteroidAsset;

        [PostCreate]
        public void OnPostCreate()
        {
            _asteroidAsset = AssetController.GetComponentFromAsset<AsteroidView>(Constants.AsteroidAssetPath);

            _viewPool = new ObjectPool<AsteroidView>(CreateView);
        }

        public AsteroidView Get(Type requester)
        {
            var view = _viewPool.Get();
            view.transform.SetParent(GameController.GetContainer(ContainerType.Game));
            return view;
        }

        public void Release(AsteroidView item, Type requester)
        {
            item.Hide();
            item.transform.SetParent(GameController.GetContainer(ContainerType.Pools));
            _viewPool.Release(item);
        }

        private AsteroidView CreateView()
        {
            var asteroid = Object.Instantiate(_asteroidAsset).GetComponent<AsteroidView>();
            asteroid.Hide();
            
            return asteroid;
        }
    }
}