using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public sealed class MainController : MonoBehaviour
{
    public static MainController instance;

    List<IAwake> OnAwakeList = new List<IAwake>();
    List<IUpdate> OnUpdateList = new List<IUpdate>();
    List<IFixedUpdate> OnFixedUpdateList = new List<IFixedUpdate>();
    List<ILateUpdate> OnLateUpdateList = new List<ILateUpdate>();

    [SerializeField]
    private GameNetworkManager _gameNetworkManager;
    private PlayerController _playerController;
    public InputController _inputController;


    void InstanceControllers()
    {

    }

    public void newPlayer(GameObject player, PlayerNetworkBehaviour playerNetworkBehaviour)
    {
        _playerController = new PlayerController(_gameNetworkManager, player, playerNetworkBehaviour);
        OnFixedUpdateList.Add(_playerController);
        _inputController = new InputController(_playerController);
        OnUpdateList.Add(_inputController);
        OnUpdateList.Add(_playerController);
    }

    private void Awake()
    {
        
        if (instance is null)
            instance = this;
        InstanceControllers();

        foreach (var feature in OnAwakeList)
        {
            feature.OnAwake();
        }
    }

    private void Update()
    {
        foreach (var feature in OnUpdateList)
        {
            feature.OnUpdate();
        }
    }

    private void FixedUpdate()
    {
        foreach (var feature in OnFixedUpdateList)
        {
            feature.OnFixedUpdate();
        }
    }

    private void LateUpdate()
    {

    }


}