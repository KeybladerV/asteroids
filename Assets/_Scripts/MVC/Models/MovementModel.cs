using UnityEngine;
using Utility;

namespace MVC.Models
{
    public class MovementModel
    {
        public readonly ReactiveField<Vector2> Position = new();
        public readonly ReactiveField<float> Angle = new();

        public readonly ReactiveField<Vector2> Direction = new();
    }
}