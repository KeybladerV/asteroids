using Asteroid.Utility.Observer;

namespace MVC.Controllers.Events
{
    public static class ScoreEvents
    {
        public static readonly Event<int> OnScoreChanged = new();
    }
}