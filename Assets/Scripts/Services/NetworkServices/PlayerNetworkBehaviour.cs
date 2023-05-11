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
}
