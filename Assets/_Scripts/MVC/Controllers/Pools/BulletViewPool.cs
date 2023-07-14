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
    public sealed class BulletViewPool : IBulletViewPool
    {
        [Inject] public IAssetController AssetController { get; set; }
        [Inject] public IGameController GameController { get; set; }
        
        private ObjectPool<BulletView> _viewPool;
        
        private BulletView _bulletAsset;

        [PostCreate]
        public void OnPostCreate()
        {
            _bulletAsset = AssetController.GetComponentFromAsset<BulletView>(Constants.BulletAssetPath);

            _viewPool = new ObjectPool<BulletView>(CreateView);
        }

        public BulletView Get(Type requester)
        {
            var view = _viewPool.Get();
            view.transform.SetParent(GameController.GetContainer(ContainerType.Game));
            return view;
        }

        public void Release(BulletView item, Type requester)
        {
            item.Hide();
            item.transform.SetParent(GameController.GetContainer(ContainerType.Pools));
            _viewPool.Release(item);
        }

        private BulletView CreateView()
        {
            var bullet = UnityEngine.Object.Instantiate(_bulletAsset).GetComponent<BulletView>();
            bullet.Hide();
            
            return bullet;
        }
    }
}