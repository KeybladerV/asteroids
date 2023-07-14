using System;
using Asteroid.Utility.Observer;
using MVC.Models;

namespace MVC.Controllers.Pools.Events
{
    public static class PoolEvents
    {
        public static readonly Event<MovementModel, Type> OnMovementModelGet = new();
        public static readonly Event<MovementModel, Type> OnMovementModelRelease = new();
    }
}