using Asteroid.Utility.Observer;
using MVC.Controllers.Enums;

namespace MVC.Controllers.Events
{
    public static class GameEvents
    {
        public static readonly Event<GameState> OnGameStateChanged = new();
    }
}