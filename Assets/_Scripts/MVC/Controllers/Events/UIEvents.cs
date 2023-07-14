using Asteroid.Utility.Observer;

namespace MVC.Controllers.Events
{
    public static class UIEvents
    {
        public static readonly Event OnStartButtonClicked = new();
        public static readonly Event OnRestartButtonClicked = new();
        public static readonly Event OnExitButtonClicked = new();
        public static readonly Event OnToMainMenuButtonClicked = new();
        public static readonly Event OnContinueButtonClicked = new();
        public static readonly Event OnPauseButtonClicked = new();
    }
}