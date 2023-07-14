using System;
using Asteroid.Utility;
using MVC.Controllers.Pools.Interfaces;
using MVC.Models;
using UnityEngine.Pool;

namespace MVC.Controllers.Pools
{
    public sealed class AsteroidModelPool : IAsteroidModelPool
    {
        private ObjectPool<AsteroidModel> _modelPool;

        [PostCreate]
        public void OnPostCreate()
        {
            _modelPool = new ObjectPool<AsteroidModel>(CreateModel, null, OnReleaseModel);
        }

        public AsteroidModel Get(Type requester) => _modelPool.Get();
        public void Release(AsteroidModel item, Type requester) => _modelPool.Release(item);

        private AsteroidModel CreateModel() => new();

        private void OnReleaseModel(AsteroidModel model)
        {
            model.IsBig = false;
        }
    }
}