using UnityEngine;
using UnityStandardAssets.Network;
using System.Collections;
using UnityEngine.Networking;
using Assets.Scripts.Network;


public class NetworkLobbyHook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerData player = gamePlayer.GetComponent<PlayerData>();
        player.PlayerID = lobby.clientId;
        player.playerCount = LobbyManager.s_Singleton._playerNumber - 1;
        //Debug.Log(isLocalPlayer + " " + isServer + " " + isClient);
        //Debug.Log(cam.clientId);
        //cam.Enable();
        //LobbyManager.s_Singleton.lobbySlots.
        //NetworkSpaceship spaceship = gamePlayer.GetComponent<NetworkSpaceship>();

        //spaceship.name = lobby.name;
        //spaceship.color = lobby.playerColor;
        //spaceship.score = 0;
        //spaceship.lifeCount = 3;
    }

    public override void OnLobbyClientExit()
    {
        //Debug.Log("Client exit lobby");
        //GameController.Instance.CmdGetClientID(GameController.Instance.netId);
        //LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        //CameraController cam = gamePlayer.GetComponent<CameraController>();
        //cam.clientId = lobby.clientId;
        //Debug.Log(cam.clientId);
        //cam.OnEnable();
    }

    public override void OnLobbyClientSceneChanged()
    {
        Debug.Log("Client changed scene");
    }

}
