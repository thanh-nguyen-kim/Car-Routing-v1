using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Network;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        //handle multi camera position and viewport
        //default full screen camera aspect is 16:9.
        //Camera in client is ok.
        
        public GameObject cameraPrefab;
        public List<GameObject> cameras = new List<GameObject>();
        public Vector3[] camOnePos, camTwoPos, cameThreePos, camFourPos, camFivePos, camSixPos;

        public void CameraViewportControl()
        {
            if (GameController.Instance.id == 0)
            {
                //todo: instance camera.
                GameObject go = Instantiate(cameraPrefab) as GameObject;
                go.transform.position = CameraPosition(0);
                go.transform.rotation = Quaternion.Euler(90, 0, 0);
                go.GetComponent<Camera>().orthographicSize = GetCamSize();
                go.GetComponent<Camera>().rect = GetCameraViewPort(1);
                go.GetComponent<Camera>().depth = 1;
                for (int i = 0; i < LobbyManager.s_Singleton._playerNumber-1; i++)
                {
                    if(i!=0)
                    go = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.position = CameraPosition(i + 1);
                    go.transform.rotation = Quaternion.Euler(90, 0, 0);
                    go.GetComponent<Camera>().orthographicSize = GetCamSize();
                    go.GetComponent<Camera>().rect = GetCameraViewPort(i);
                    go.GetComponent<Camera>().depth = 1;
                    cameras.Add(go);
                }

            }
        }


        //todo: handle camera display in client machine.
        public void SetClientCamera()
        {
            Debug.Log("Set");
            GameObject go = Instantiate(cameraPrefab) as GameObject;
            go.transform.rotation = Quaternion.Euler(90, 0, 0);
            Vector3 tmp = CameraPosition(GameController.Instance.id);
            go.transform.position = new Vector3(tmp.x, tmp.y, tmp.z);
            go.GetComponent<Camera>().orthographicSize = GetCamSize();
            go.GetComponent<Camera>().rect = ClientCameraViewPort();
            Camera.main.depth = 0;
            go.GetComponent<Camera>().depth = 1;
            cameras.Add(go);
            //NetworkServer.Spawn(go);
        }

        public int GetCamSize()
        {
            if (GameController.Instance.clientCount != 4 && GameController.Instance.clientCount != 6) return 200;
            else
                return 100;
        }


        public Rect ClientCameraViewPort()
        {
            float targetAspect = 16f / 9f;
            float clientScreenAspect = Screen.width;
            clientScreenAspect = clientScreenAspect / Screen.height;
            switch (GameController.Instance.clientCount)
            {
                case 1:
                    targetAspect = 16f / 9f;
                    break;
                case 2:
                    targetAspect = 16f / 18f;
                    break;
                case 3:
                    targetAspect = 16f / 27f;
                    break;
                case 4:
                    targetAspect = 16f / 9f;
                    break;
                case 5:
                    targetAspect = 16f / 45f;
                    break;
                case 6:
                    targetAspect = 32f / 27f;
                    break;
            }
            float scaleHeight = clientScreenAspect / targetAspect;
            //Debug.Log(clientScreenAspect);
            Rect rect = new Rect();
            if (scaleHeight < 1)
            {
                rect.width = 1;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1 - scaleHeight) / 2f;
            }
            else
            {
                rect.width = 1 / scaleHeight;
                rect.height = 1;
                rect.x = (1 - 1 / scaleHeight) / 2f;
                rect.y = 0;
            }
            //Debug.Log(rect);
            return rect;
        }

        public Rect GetCameraViewPort(int cameraId)
        {
            switch (GameController.Instance.clientCount)
            {
                case 1:
                    return new Rect(0, 0, 1, 1);
                case 2:
                    if (cameraId == 0) return new Rect(0, 0, 0.5f, 1);
                    return new Rect(0.5f, 0, 0.5f, 1);
                case 3:
                    if (cameraId == 0) return new Rect(0, 0, 0.33f, 1);
                    else
                        if (cameraId == 1) return new Rect(0.33f, 0, 0.33f, 1);
                    return new Rect(0.66f, 0, 0.33f, 1);
                case 4:
                    if (cameraId == 0) return new Rect(0, 0, 0.5f, 0.5f);
                    else
                         if (cameraId == 1) return new Rect(0.5f, 0, 0.5f, 0.5f);
                    else
                         if (cameraId == 2) return new Rect(0, 0.5f, 0.5f, 0.5f);
                    else
                        return new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                case 5:
                    if (cameraId == 0) return new Rect(0, 0, 0.2f, 1);
                    else
                         if (cameraId == 1) return new Rect(0.2f, 0, 0.2f, 1);
                    else
                         if (cameraId == 2) return new Rect(0.4f, 0, 0.2f, 1);
                    else
                        if (cameraId == 3) return new Rect(0.6f, 0, 0.2f, 1);
                    else
                        return new Rect(0.8f, 0, 0.2f, 1);
                case 6:
                    if (cameraId == 0) return new Rect(0, 0, 0.33f, 0.5f);
                    else
                         if (cameraId == 1) return new Rect(0.33f, 0, 0.33f, 0.5f);
                    else
                         if (cameraId == 2) return new Rect(0.66f, 0, 0.33f, 0.5f);
                    else
                        if (cameraId == 3) return new Rect(0, 0.5f, 0.33f, 0.5f);
                    else
                        if (cameraId == 4) return new Rect(0.33f, 0.5f, 0.33f, 0.5f);
                    else
                        return new Rect(0.66f, 0.5f, 0.33f, 0.5f);
                default: return new Rect(0, 0, 1, 1);
            }
        }

        public Vector3 CameraPosition(int cameraId)
        {

            switch (cameraId)
            {
                case 1:
                    return camOnePos[GameController.Instance.clientCount - 1];
                case 2:
                    return camTwoPos[GameController.Instance.clientCount - 2];
                case 3:
                    return cameThreePos[GameController.Instance.clientCount - 3];
                case 4:
                    return camFourPos[GameController.Instance.clientCount - 4];
                case 5:
                    return camFivePos[GameController.Instance.clientCount - 5];
                case 6:
                    return camSixPos[GameController.Instance.clientCount - 6];
                default:
                    return camOnePos[0];
            }
        }

    }
}
