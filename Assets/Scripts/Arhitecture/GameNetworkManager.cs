using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Linq;

public class GameNetworkManager : NetworkBehaviour
{
    [SerializeField]
    private Canvas canvas;

    public Text localPlayer;
    public List<Text> othersPlayers;
    public Text GameOver;
    public bool isOver = false;
    private int N = 0;

    [SyncVar]
    private List<int> Global = new List<int>();
    [SyncVar]
    private List<string> Names = new List<string>();
    [SyncVar]
    private List<int> ids = new List<int>();


    [ClientRpc]
    public void RpcGlobalText(int p, int playerId)
    {
        if (isOver) return;
        localPlayer.text = "My Points: " + p;

        if(ids.Capacity == 0)
        {
            ids.Add(playerId);
            Global.Add(p);
            Names.Add("Player " + N);
            N++;
            othersPlayers[0].text = Names[0] + ": " + Global[0];
        }
        else
        {
            for (int i = 0; i < ids.Count; i++)
            {
                if (playerId == ids[i])
                {
                    Global[i] = p;
                    othersPlayers[i].text = Names[i] + ": " + Global[i];
                    break;
                }
                else if (ids[i] == ids.LastOrDefault())
                {
                    ids.Add(playerId);
                    Global.Add(p);
                    Names.Add("Player " + N);
                    N++;
                    othersPlayers[i].text = Names[i] + ": " + Global[i];
                    break;
                }
            }
        }


        if (p >= 3)
        {
            string Winner = "";
            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] == playerId)
                {
                    Winner = Names[i];
                }
            }
            GameOver.text = "" + Winner + " is the winner";
            isOver = true;
            return;
        }

    }
}
