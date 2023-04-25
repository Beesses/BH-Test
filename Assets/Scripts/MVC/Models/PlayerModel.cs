using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{

    public Transform transform;
    public Rigidbody rigidbody;
    public List<Ray> rays;
    public float speed;
    public float coef;
    public float rushTimer;
    public float afterRushTime;
    public Material material;
    public Material invulMaterial;
    public float invul;
    public int points;
    public GameObject player;

    public PlayerModel(PlayerData data, GameObject _player)
    {
        invulMaterial = data.invulMaterial;
        player = _player;
        invul = data.invulnerability;
        material = data.material;
        afterRushTime = data.afterRushTime;
        rushTimer = data.readyToRushTime;
        coef = data.rushCoef;
        speed = data.speed;
        transform = _player.transform;
        rigidbody = transform.GetComponent<Rigidbody>();
    }
}
