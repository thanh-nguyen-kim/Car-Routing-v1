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
                this.playerID = value;
                //if(isServer)
                //Debug.Log(isLocalPlayer + " " + isServer + " " + isClient);
                //GameController.Instance.InitCamera(playerID);
            }
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Invoke("Init", 1f);
        }

        private void Init()
        {
            GameController.Instance.Init(playerID, playerCount);
            //Debug.Log(isLocalPlayer + " " + isServer + " " + isClient);
            //Debug.Log(playerID);
            //GameController.Instance.InitCamera(playerID);
        }
    }
}
