using System;
using MVC.Models;
using MVC.Views;

namespace MVC.Controllers.Pools.Interfaces
{
    public interface IPool<T>
    {
        T Get(Type requester);
        void Release(T item, Type requester);
    }
    
    public interface IMovementModelPool : IPool<MovementModel> { }
    public interface IAsteroidModelPool : IPool<AsteroidModel> { }
    public interface IAsteroidViewPool : IPool<AsteroidView> { }
    public interface IBulletViewPool : IPool<BulletView> { }
    public interface IUFOViewPool : IPool<UFOView> { }
}