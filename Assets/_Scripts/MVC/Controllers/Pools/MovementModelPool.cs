using System;
using Asteroid.Utility;
using Asteroid.Utility.Observer;
using MVC.Controllers.Pools.Events;
using MVC.Controllers.Pools.Interfaces;
using MVC.Models;
using UnityEngine.Pool;

namespace MVC.Controllers.Pools
{
    public sealed class MovementModelPool : IMovementModelPool
    {
        [Inject] public IEventDispatcher Dispatcher { get; set; }
        
        private ObjectPool<MovementModel> _pool;

        [PostCreate]
        public void OnPostCreate()
        {
            _pool = new ObjectPool<MovementModel>(CreateFunc, null, ActionOnRelease);
        }

        public MovementModel Get(Type requester)
        {
            var model = _pool.Get();
            Dispatcher.Dispatch(PoolEvents.OnMovementModelGet, model, requester);
            return model;
        }

        public void Release(MovementModel item, Type requester)
        {
            Dispatcher.Dispatch(PoolEvents.OnMovementModelRelease, item, requester);
            _pool.Release(item);
        }

        private void ActionOnRelease(MovementModel model)
        {
            model.Angle.Reset();
            model.Position.Reset();
            model.Direction.Reset();
        }

        private MovementModel CreateFunc() => new MovementModel();
    }
}