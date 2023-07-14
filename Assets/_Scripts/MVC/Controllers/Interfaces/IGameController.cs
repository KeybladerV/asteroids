using MVC.Controllers.Enums;
using UnityEngine;

namespace MVC.Controllers.Interfaces
{
    public interface IGameController
    {
        void Start();
        void Subscribe(IUpdatable updatable, GameState state);
        void Unsubscribe(IUpdatable updatable, GameState state);
        void Update(float deltaTime);
        void ChangeGameState(GameState state);
        IGameController SetContainer(Transform container, ContainerType game);
        Transform GetContainer(ContainerType type);
    }
}