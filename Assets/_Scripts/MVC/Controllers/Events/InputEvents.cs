using Asteroid.Utility.Observer;
using UnityEngine;
using Event = Asteroid.Utility.Observer.Event;

namespace MVC.Controllers.Events
{
    public static class InputEvents
    {
        public static readonly Event<Vector2> OnMovement = new();
        public static readonly Event OnMovementCancel = new();
        public static readonly Event OnAttack1 = new();
        public static readonly Event OnAttack2 = new();
        public static readonly Event OnPause = new();
    }
}