using Asteroid.Utility;
using Asteroid.Utility.Observer;
using MVC.Controllers;
using MVC.Controllers.Enums;
using MVC.Controllers.Interfaces;
using MVC.Controllers.Pools;
using MVC.Controllers.Pools.Interfaces;
using UnityEngine;


public class Root : MonoBehaviour
{
    [SerializeField] private Canvas _menuCanvas;
    [SerializeField] private Canvas _guiCanvas;
    [SerializeField] private Transform _gameContainer;
    [SerializeField] private Transform _poolsContainer;
    
    private IInjectionBinder _di;
    private IGameController _gameController;

    private void Awake()
    {
        _di = new SuperStupidDI();
        CreateBindings();
        Init();
    }

    private void CreateBindings()
    {
        _di.Bind<IGameController, GameController>().CreateOnStart();
        _di.Bind<IUIController, UIController>().CreateOnStart();
        _di.Bind<IPlayerController, PlayerController>().CreateOnStart();
        _di.Bind<IAsteroidsController, AsteroidsController>().CreateOnStart();
        _di.Bind<IGameFieldController, GameFieldController>().CreateOnStart();
        _di.Bind<IInputController, InputController>().CreateOnStart();
        _di.Bind<IScoreController, ScoreController>().CreateOnStart();
        _di.Bind<IUFOController, UFOController>().CreateOnStart();
        
        _di.Bind<IAssetController, AssetController>();
        _di.Bind<IAsteroidViewPool, AsteroidViewViewPool>();
        _di.Bind<IBulletViewPool, BulletViewPool>();
        _di.Bind<IMovementModelPool, MovementModelPool>();
        _di.Bind<IAsteroidModelPool, AsteroidModelPool>();
        _di.Bind<IUFOViewPool, UFOViewPool>();
        _di.Bind<IEventDispatcher, EventDispatcher>();
        _di.Bind<IWeaponController, WeaponController>();
    }

    private void Init()
    {
        var uiController = _di.GetOrCreate<IUIController>();
        uiController.RegisterCanvas(CanvasType.Menu, _menuCanvas)
            .RegisterCanvas(CanvasType.GUI, _guiCanvas);
        
        _di.InitStartBindings();
        
        _gameController = _di.GetOrCreate<IGameController>();
        _gameController.SetContainer(_gameContainer, ContainerType.Game)
            .SetContainer(_poolsContainer, ContainerType.Pools);
    }

    private void Start()
    {
        _gameController.Start();
    }

    private void Update()
    {
        _gameController.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        _di.DestroyInstances();
    }
}