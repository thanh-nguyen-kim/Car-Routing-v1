using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using Assets.Scripts.WaypointsProcessing;
using UnityStandardAssets.Cameras;
public class CameraSwitch : MonoBehaviour
{
    //public GameObject[] objects;
    public GameObject followCamera;
    public Text text;

    private int m_CurrentActiveObject;


    private void OnEnable()
    {
        //text.text = objects[m_CurrentActiveObject].name;
    }

    public void OrthorCam()
    {
        GameController.Instance.currentCam.SetActive(true);
    }

    public void PerpestiveCam()
    {
        GameController.Instance.currentCam.SetActive(false);
    }

    public void FollowCam(int i) {
        followCamera.GetComponent<AutoCam>().Target = FindCar(i);
        Debug.Log(i);
        if (followCamera.GetComponent<AutoCam>().Target == null) return;
        GameController.Instance.currentCam.SetActive(false);
        followCamera.SetActive(true);
    }

    private Transform FindCar(int i)
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject go in cars)
        {
            if (go.GetComponent<WaypointsProgressHandle>().carId == i) return go.transform;
        }
        return null;
    }
    //public void NextCamera()
    //{
    //    objects[0] = GameController.Instance.currentCam;
    //    int nextactiveobject = m_CurrentActiveObject + 1 >= objects.Length ? 0 : m_CurrentActiveObject + 1;

    //    for (int i = 0; i < objects.Length; i++)
    //    {
    //        objects[i].SetActive(i == nextactiveobject);
    //    }

    //    m_CurrentActiveObject = nextactiveobject;
    //    text.text = objects[m_CurrentActiveObject].name;
    //}
}
