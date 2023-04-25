using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerNetworkBehaviour : NetworkBehaviour
{
    private MainController main;
    private void Awake()
    {
        Debug.Log("new player");
        main = GameObject.Find("MainController").GetComponent<MainController>();
        main.newPlayer(this.gameObject, this);
        main._inputController.IsActive = true;
    }
}
