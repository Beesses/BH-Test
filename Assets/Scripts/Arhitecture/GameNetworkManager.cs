using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameNetworkManager : NetworkBehaviour
{
    [SerializeField]
    private Canvas canvas;

    public Text localPlayer;
    public List<Text> othersPlayers;
    public Text GameOver;
    [SyncVar]
    public bool isOver = false;
    [SyncVar]
    private int N = 0;
    private float timer = 0;

    readonly SyncList<int> Global = new SyncList<int>();
    readonly SyncList<string> Names = new SyncList<string>();
    readonly SyncList<int> ids = new SyncList<int>();

    public List<int> GlobalLocal;
    public List<string> NamesLocal;
    public List<int> IdsLocal;

    [Server]
    void AddGlobal(int points)
    {
        Global.Add(points);
    }

    [Server]
    void AddNames()
    {
        Names.Add("Player " + N);
        N++;
    }

    [Server]
    void AddIds(int id)
    {
        ids.Add(id);
    }

    [Command]
    public void CmdAddPlayer(int points, int id)
    {
        if (points >= 3)
        {
            string Winner = "";
            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] == id)
                {
                    Winner = Names[i];
                    break;
                }
            }
            GameOver.text = "" + Winner + " is the winner";
            isOver = true;
        }
        Debug.Log("AddPlayer");
        for(int i = 0; i < ids.Count; i++)
        {
            if(id == ids[i])
            {
                Global[i] = points;
                updateLeaderboard();
                return;
            }
        }
        AddGlobal(points);
        AddNames();
        AddIds(id);
        updateLeaderboard();
    }

    [Server]
    public void AddPlayer(int points, int id)
    {
        if (points >= 3)
        {
            string Winner = "";
            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] == id)
                {
                    Winner = Names[i];
                    break;
                }
            }
            GameOver.text = "" + Winner + " is the winner";
            isOver = true;
        }
        Debug.Log("AddPlayer");
        for (int i = 0; i < ids.Count; i++)
        {
            if (id == ids[i])
            {
                Global[i] = points;
                updateLeaderboard();
                return;
            }
        }
        AddGlobal(points);
        AddNames();
        AddIds(id);
        updateLeaderboard();
    }


    [ClientRpc]
    void updateLeaderboard()
    {
        for (int i = 0; i < ids.Count; i++)
        {
            othersPlayers[i].text = Names[i] + ": " + Global[i];
        }
    }

    void SyncGlobalLocal(SyncList<int>.Operation op, int index, int oldItem, int newItem)
    {
        switch (op)
        {
            case SyncList<int>.Operation.OP_ADD:
                {
                    GlobalLocal.Add(newItem);
                    break;
                }
            case SyncList<int>.Operation.OP_CLEAR:
                {

                    break;
                }
            case SyncList<int>.Operation.OP_INSERT:
                {

                    break;
                }
            case SyncList<int>.Operation.OP_REMOVEAT:
                {

                    break;
                }
            case SyncList<int>.Operation.OP_SET:
                {

                    break;
                }
        }
    }

    void SyncNamesLocal(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        switch (op)
        {
            case SyncList<string>.Operation.OP_ADD:
                {
                    NamesLocal.Add(newItem);
                    break;
                }
            case SyncList<string>.Operation.OP_CLEAR:
                {

                    break;
                }
            case SyncList<string>.Operation.OP_INSERT:
                {

                    break;
                }
            case SyncList<string>.Operation.OP_REMOVEAT:
                {

                    break;
                }
            case SyncList<string>.Operation.OP_SET:
                {

                    break;
                }
        }
    }

    void SyncIdsLocal(SyncList<int>.Operation op, int index, int oldItem, int newItem)
    {
        switch (op)
        {
            case SyncList<int>.Operation.OP_ADD:
                {
                    IdsLocal.Add(newItem);
                    break;
                }
            case SyncList<int>.Operation.OP_CLEAR:
                {

                    break;
                }
            case SyncList<int>.Operation.OP_INSERT:
                {

                    break;
                }
            case SyncList<int>.Operation.OP_REMOVEAT:
                {

                    break;
                }
            case SyncList<int>.Operation.OP_SET:
                {

                    break;
                }
        }
    }

    public void sync()
    {
        Global.Callback += SyncGlobalLocal;
        Names.Callback += SyncNamesLocal;
        ids.Callback += SyncIdsLocal;


        for (int i = 0; i < Global.Count; i++)
        {
            SyncGlobalLocal(SyncList<int>.Operation.OP_ADD, i, 0, Global[i]);
        }
        for (int i = 0; i < Names.Count; i++)
        {
            SyncNamesLocal(SyncList<string>.Operation.OP_ADD, i, " ", Names[i]);
        }
        for (int i = 0; i < ids.Count; i++)
        {
            SyncIdsLocal(SyncList<int>.Operation.OP_ADD, i, 0, ids[i]);
        }
    }

    private void Update()
    {
        if (!isOver) return;

        timer += Time.deltaTime;

        if(timer > 5)
        {
            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        }
    }
}
