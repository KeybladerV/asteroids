using UnityEngine;

namespace MVC.Controllers.Interfaces
{
    public interface IGameFieldController
    {
        float DiagonalLength { get; }
        Vector3 MinimumCorner { get; }
        Vector3 MaximumCorner { get; }
        bool IsInGameField(Vector3 position);
    }
}