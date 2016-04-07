using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts;
using Assets.Scripts.WaypointsProcessing;
using UnityStandardAssets.Vehicles.Car;
namespace Assets.Scripts.ShortScript
{
    public class StartBtn:MonoBehaviour
    {
        public GameObject CarPrefabs;
        public void StartTheCar() {
            //GameController.Instance.GameState = GameStates.Run;
        }

        public void CamSetUp()
        {
            GameController.Instance.gameObject.GetComponent<CameraController>().CameraViewportControl();
        }


        public void SpawnCar()
        {
            //Debug.Log("Spawn");
            //GameObject go=Instantiate(CarPrefabs, new Vector3(Random.Range(-50, 50), 10, Random.Range(-50, 50)), Quaternion.identity) as GameObject;
            //NetworkServer.Spawn(go);
            //GameController.Instance.SpawnCar();
        }
    }
}
