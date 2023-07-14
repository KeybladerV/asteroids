using MVC.Controllers.Enums;
using UnityEngine;

namespace MVC.Controllers.Interfaces
{
    public interface IUIController
    {
        IUIController RegisterCanvas(CanvasType type, Canvas canvas);
        void PrepareAllUI();
    }
}