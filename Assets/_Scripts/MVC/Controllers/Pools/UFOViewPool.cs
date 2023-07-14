using System;
using Asteroid.Utility;
using Configs;
using MVC.Controllers.Enums;
using MVC.Controllers.Interfaces;
using MVC.Controllers.Pools.Interfaces;
using MVC.Views;
using UnityEngine.Pool;

namespace MVC.Controllers.Pools
{
    public sealed class UFOViewPool : IUFOViewPool
    {
        [Inject] public IAssetController AssetController { get; set; }
        [Inject] public IGameController GameController { get; set; }
        
        private ObjectPool<UFOView> _viewPool;
        
        private UFOView _ufoAsset;

        [PostCreate]
        public void OnPostCreate()
        {
            _ufoAsset = AssetController.GetComponentFromAsset<UFOView>(Constants.UFOAssetPath);

            _viewPool = new ObjectPool<UFOView>(CreateView);
        }

        public UFOView Get(Type requester)
        {
            var view = _viewPool.Get();
            view.transform.SetParent(GameController.GetContainer(ContainerType.Game));
            return view;
        }

        public void Release(UFOView item, Type requester)
        {
            item.Hide();
            item.transform.SetParent(GameController.GetContainer(ContainerType.Pools));
            _viewPool.Release(item);
        }

        private UFOView CreateView()
        {
            var ufo = UnityEngine.Object.Instantiate(_ufoAsset).GetComponent<UFOView>();
            ufo.Hide();
            
            return ufo;
        }
    }
}