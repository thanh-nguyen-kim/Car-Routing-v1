using UnityEngine;
using UnityStandardAssets.Network;
using System.Collections;
using UnityEngine.Networking;
using Assets.Scripts.Network;
using Assets.Scripts;


public class NetworkLobbyHook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerData player = gamePlayer.GetComponent<PlayerData>();
        player.transform.parent = LobbyManager.s_Singleton.gameObject.transform.GetChild(6);
        LobbyManager.s_Singleton.playersNetwork.Add(gamePlayer);
        player.PlayerID = lobby.clientId;
        player.playerCount = LobbyManager.s_Singleton._playerNumber - 1;
    }

    public override void OnOtherClientDisconnect()
    {
        base.OnOtherClientDisconnect();
        for (int i = 0; i < LobbyManager.s_Singleton.playersNetwork.Count; i++)
        {
            if (LobbyManager.s_Singleton.playersNetwork[i] == null) continue;
            else
            {
                PlayerData player = LobbyManager.s_Singleton.playersNetwork[i].GetComponent<PlayerData>();
                if (player.PlayerID > LobbyManager.s_Singleton.disconnectedClientId)
                {
                    player.PlayerID--;
                }
                //Debug.Log(player.PlayerID);
                player.playerCount--;
                GameController.Instance.GameState = GameStates.Run;
                //player.RpcInit(player.playerID, player.playerCount);
            }
        }
        Invoke("ReAssignId", 0.2f);
    }

    private void ReAssignId()
    {
        for (int i = 0; i < LobbyManager.s_Singleton.playersNetwork.Count;)
            if (LobbyManager.s_Singleton.playersNetwork[i] == null) LobbyManager.s_Singleton.playersNetwork.RemoveAt(i);
            else
            {
                PlayerData player = LobbyManager.s_Singleton.playersNetwork[i].GetComponent<PlayerData>();
                player.RpcInit(player.playerID, player.playerCount);
                i++;
            }
    }
}

