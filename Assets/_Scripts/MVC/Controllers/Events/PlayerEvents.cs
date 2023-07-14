using Asteroid.Utility.Observer;
using MVC.Models;

namespace MVC.Controllers.Events
{
    public static class PlayerEvents
    {
        public static readonly Event<PlayerModel, MovementModel> OnPlayerModelsInitialized = new();
        public static readonly Event OnPlayerDestroyed = new();
        public static readonly Event OnPlayerSpawned = new();
    }
}