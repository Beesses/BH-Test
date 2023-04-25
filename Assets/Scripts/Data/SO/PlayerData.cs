using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Models/Player")]
public class PlayerData : SOBase
{
    public GameObject prefab;   
    public Vector2 spawnPoint;
    public Material material;
    public Material invulMaterial;
    public float speed;
    public float rushReloadTime;
    public float afterRushTime;
    public float readyToRushTime;
    public float rushCoef;
    public float invulnerability;
}
