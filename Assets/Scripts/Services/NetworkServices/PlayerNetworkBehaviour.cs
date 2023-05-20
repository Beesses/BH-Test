using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerNetworkBehaviour : NetworkBehaviour
{
    private MainController main;
    private float timer = 0;
    private bool isOver = false;
    Material material;
    [SyncVar(hook = nameof(SetColor))]
    public Color playerColor = Color.white;

    [SyncVar(hook = nameof(SetBool))]
    public bool invul = false;
    private void Awake()
    {
        material = this.gameObject.GetComponent<Renderer>().material;
        Debug.Log("new player");
        main = GameObject.Find("MainController").GetComponent<MainController>();
        main.newPlayer(this.gameObject, this);
        main._inputController.IsActive = true;
    }

    private void Update()
    {
        if (!isOver) return;

        timer += Time.deltaTime;

        if (timer > 5)
        {
            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        }
    }

    void SetColor(Color oldColor, Color newColor)
    {
        material.color = newColor;
    }

    [Server]
    public void ChangeColor(Color newColor)
    {
        playerColor = newColor;
    }

    [Command]
    public void CmdChangeColor(Color newColor)
    {
        ChangeColor(newColor);
    }

    [Command]
    public void ReloadLevel()
    {
        isOver = true;
    }

    void SetBool(bool oldBool, bool newBool)
    {
        invul = newBool;
    }

    [Server]
    public void ChangeBool(bool newBool)
    {
        invul = newBool;
    }

    [Command]
    public void CmdChangeBool(bool newBool)
    {
        ChangeBool(newBool);
    }

    [Command]
    public void CmdFoundPlayer(Transform id)
    {
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach(var player in players)
        {
            if(player.transform == id)
            {
                player.GetComponent<PlayerNetworkBehaviour>().CmdChangeBool(true);
                break;
            }
        }
    }

    [Command]
    public void CmdFoundLocalPlayer(Transform id)
    {
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            if (player.transform == id)
            {
                player.GetComponent<PlayerNetworkBehaviour>().ChangeBool(true);
                break;
            }
        }
    }
}
