using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Assets.Scripts.Network;
using UnityStandardAssets.Vehicles.Car;
using Assets.Scripts.WaypointsProcessing;
namespace Assets.Scripts
{
    public enum GameStates
    {
        Stanby,
        Run
    }

    public class GameController : NetworkBehaviour
    {

        private static GameController instance = null;
        private GameStates gameState;
        private List<Action> gameStateChangeSubcriber = new List<Action>();
        public int id = -1;
        public int clientCount = 0;
        private int carCount = 0;
        public GameObject[] CarPrefabs;
        public GameObject[] spawnPoints;
        //[SyncVar]
        public List<GameObject> cars = new List<GameObject>();
        public GameObject currentCam = null;
        public PlayerData playerData = null;

        public static GameController Instance
        {
            get
            {
                return instance;
            }

            private set { }
        }

        public GameStates GameState
        {
            get
            {
                return gameState;
            }

            set
            {
                this.gameState = value;
                GameStateChangeBroadcast();
            }
        }

        #region GameStateChange Publish-Subcriber
        public void GameStateChangeSubcribe(Action subcriber)
        {
            gameStateChangeSubcriber.Add(subcriber);
        }

        public void GameStateChangeUnSubcribe(Action subcribre)
        {
            gameStateChangeSubcriber.Remove(subcribre);
        }

        public void CleanSubcriber()
        {
            gameStateChangeSubcriber.Clear();
        }

        public void GameStateChangeBroadcast()
        {
            for (int i = 0; i < gameStateChangeSubcriber.Count; i++)
            {
                if (gameStateChangeSubcriber[i] != null)
                    gameStateChangeSubcriber[i]();
            }

        }
        #endregion
        [Client]
        public void Init(int id,int playerCount)
        {
            //Debug.Log(playerData.playerID + " " + id);
            if (playerData.playerID != id)
            {
                return;
            }
            if (currentCam != null)
            {
                Destroy(currentCam);
                currentCam = null;
            }
            GameState = GameStates.Stanby;
            Instance.id = id;
            clientCount = playerCount;
            if (Instance.id != 0)
            {
                GetComponent<CameraController>().SetClientCamera();
            }
            else
            {
                GetComponent<CameraController>().CameraViewportControl();
            }
        }

        void OnApplicationQuit()
        {
            CleanSubcriber();
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
                DestroyImmediate(gameObject);
        }
        #region Methods
        public GameObject SpawnCar(int speed,int color)
        {
            carCount += 1;
            GameObject go = Instantiate(CarPrefabs[color], spawnPoints[carCount-1].transform.position, spawnPoints[carCount - 1].transform.rotation) as GameObject;
            cars.Add(go);
            NetworkServer.Spawn(go);
            go.GetComponent<WaypointsProgressHandle>().carId = carCount-1;
            go.GetComponent<CarController>().MaxSpeed = speed;
            return go;
        }

        public void DespawnCar(GameObject car)
        {
            carCount--;
            cars.Remove(car);
            NetworkServer.Destroy(car);
        }

        #endregion

    }
}
