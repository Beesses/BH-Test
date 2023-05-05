using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum States
{
    IDLE,
    MOVING,
    READY_TO_RUSH,
    RUSH,
    AFTER_RUSH
}
public sealed class PlayerController : IFixedUpdate, IUpdate, IAwake
{
    private PlayerNetworkBehaviour _playerNetwork;
    private GameNetworkManager _manager;
    private PlayerModel _model;
    private bool invul;
    private float collisionTimer = 0;
    private float timer;
    private float invulTimer = 0;
    private float horizontal;
    private float vertical;
    private float step;
    private GameObject cam;
    private bool isRush = false;

    States state = States.MOVING;

    States prevState;
    public States State => prevState;

    public PlayerController(GameNetworkManager manager, GameObject player, PlayerNetworkBehaviour playerNetworkBehaviour)
    {
        var data = Data.GetModelData<PlayerData>();
        _model = new PlayerModel(data, player);
        _playerNetwork = playerNetworkBehaviour;
        _manager = manager;
    }

    public void OnAwake()
    {
        if(_manager.isClient)
        {
            _manager.sync();
        }
    }

    public void OnUpdate()
    {
        if (_playerNetwork.isLocalPlayer)
        {
            if (cam == null)
            {
                cam = GameObject.FindGameObjectWithTag("MainCamera") as GameObject;
                cam.transform.SetParent(_model.transform);
                Debug.Log("camera");
            }
            _manager.localPlayer.text = "My Points: " + _model.points;
            CheckColission();
        }
    }

    public void OnFixedUpdate()
    {
        if(!_playerNetwork.isLocalPlayer)
        {
            return;
        }
        UpdateStates();
    }


    void CheckColission()
    {
        DrawRays();

        foreach (var ray in _model.rays)
        {
            Debug.DrawRay(ray.origin, ray.direction * 0.108f, Color.green);
            RaycastHit hitData;
            Physics.Raycast(ray, out hitData);
            if (hitData.transform && hitData.distance < 0.14f && hitData.transform.tag == "Player")
            {
                if (invul || hitData.transform == _model.transform) return;
                if (isRush && collisionTimer <= 0 && hitData.transform.GetComponent<Renderer>().material.color == Color.white)
                {
                    collisionTimer = 2;
                    _model.points += 1;
                    if(_playerNetwork.isServer)
                    {
                        _manager.AddPlayer(_model.points, _model.player.GetInstanceID());
                    }
                    else
                    {
                        if (_model.points >= 3)
                        {
                            _playerNetwork.ReloadLevel();
                        }
                    }
                }
                else if (!isRush)
                {
                    invul = true;
                }
                
            }
        }

        if(collisionTimer > 0)
        {
            collisionTimer -= Time.deltaTime;
        }
    }

    public void DrawRays()
    {
        _model.rays = new List<Ray>();

        _model.rays.Add(new Ray(_model.transform.position, _model.transform.forward));
        _model.rays.Add(new Ray(_model.transform.position, _model.transform.right));
        _model.rays.Add(new Ray(_model.transform.position, -_model.transform.right));
        _model.rays.Add(new Ray(_model.transform.position, -_model.transform.forward));
        _model.rays.Add(new Ray(_model.transform.position, _model.transform.forward + _model.transform.right));
        _model.rays.Add(new Ray(_model.transform.position, _model.transform.right - _model.transform.forward));
        _model.rays.Add(new Ray(_model.transform.position, -_model.transform.right - _model.transform.forward));
        _model.rays.Add(new Ray(_model.transform.position, -_model.transform.right + _model.transform.forward));
        _model.rays.Add(new Ray(_model.transform.position, _model.transform.forward * 2 + _model.transform.right));
        _model.rays.Add(new Ray(_model.transform.position, _model.transform.right * 2 - _model.transform.forward));
        _model.rays.Add(new Ray(_model.transform.position, -_model.transform.right * 2 - _model.transform.forward));
        _model.rays.Add(new Ray(_model.transform.position, -_model.transform.right * 2 + _model.transform.forward));
        _model.rays.Add(new Ray(_model.transform.position, _model.transform.forward + _model.transform.right * 2));
        _model.rays.Add(new Ray(_model.transform.position, _model.transform.right - _model.transform.forward * 2));
        _model.rays.Add(new Ray(_model.transform.position, -_model.transform.right - _model.transform.forward * 2));
        _model.rays.Add(new Ray(_model.transform.position, -_model.transform.right + _model.transform.forward * 2));
    }

    public void Moving(float hor, float vert)
    {
        horizontal = hor;
        vertical = vert;
    }

    public void Dashing()
    {
        if (state == States.MOVING || !invul)
        {
            state = States.READY_TO_RUSH;
        }
    }

    public void SideStep(float side)
    {
        step = side;
    }

    public void UpdateStates()
    {
        if(invul)
        {
            _playerNetwork.CmdChangeColor(UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            invulTimer += Time.fixedDeltaTime;
            //_model.player.GetComponent<Renderer>().material = _model.invulMaterial;
            //_model.invulMaterial.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            if (invulTimer >= _model.invul)
            {
                invul = false;
                invulTimer = 0;
                _playerNetwork.CmdChangeColor(Color.white);
            }
        }

        switch (state)
        {
            case States.MOVING:
                if (horizontal != 0 || vertical != 0 || step != 0)
                {
                    _model.transform.rotation *= Quaternion.Euler(0f, horizontal * 1.5f, 0f);
                    _model.rigidbody.MovePosition(_model.transform.position + _model.transform.forward * vertical * _model.speed * Time.deltaTime + (_model.transform.right * step * _model.speed * Time.deltaTime));
                }
                break;
            case States.READY_TO_RUSH:
                _model.rigidbody.velocity = Vector3.zero;
                timer += Time.fixedDeltaTime;
                _model.transform.rotation = Quaternion.Slerp(_model.transform.rotation, Quaternion.Euler(0, _model.transform.rotation.eulerAngles.y, 0), Time.deltaTime * 5);
                if (timer >= _model.rushTimer)
                {
                    timer = 0;
                    isRush = true;
                    state = States.RUSH;
                }
                Debug.Log(state);
                break;
            case States.RUSH:
                _model.rigidbody.AddForce(_model.transform.forward * _model.coef, ForceMode.Impulse);
                state = States.AFTER_RUSH;
                Debug.Log(state);
                break;
            case States.AFTER_RUSH:
                timer += Time.fixedDeltaTime;
                if (timer >= _model.afterRushTime)
                {
                    timer = 0;
                    state = States.MOVING;
                    Debug.Log(state);
                    isRush = false;
                    break;
                }
                break;
        }
        prevState = state;
    }
}
