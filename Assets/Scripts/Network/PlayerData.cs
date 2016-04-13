using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace Assets.Scripts.Network
{
    public class PlayerData : NetworkBehaviour
    {
        [SyncVar]
        public int playerID = -1;
        [SyncVar]
        public int playerCount = 0;

        public int PlayerID
        {
            get
            {
                return playerID;
            }

            set
            {
                //int tmp = playerID;
                this.playerID = value;
                //if(tmp!=-1) 
            }
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Invoke("Init", 1f);
        }

        public void Init()
        {
            GameController.Instance.playerData = this;
            if (isLocalPlayer)
                GameController.Instance.Init(playerID, playerCount);
        }

        [ClientRpc]
        public void RpcInit(int _playerID,int _playerCount)
        {
            //Debug.Log(_playerID != playerID+playerID+_playerID);
            //if (_playerID != playerID) return;
            GameController.Instance.Init(_playerID, _playerCount);
            GameController.Instance.GameState = GameStates.Run;
        }
    }
}
