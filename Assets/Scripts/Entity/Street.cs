using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.WaypointsProcessing;
namespace Assets.Scripts.Entity
{
    [System.Serializable]
    public class Street
    {
        public List<GameObject> points;
        public List<GameObject> cars = new List<GameObject>();
        public bool IsTwoWay = false;

        public bool ContentCheckPoint(GameObject checkPoint)
        {
            foreach (GameObject go in points)
            {
                if (go == checkPoint) return true;
            }
            return false;
        }

        public int CheckPointIndex(GameObject checkPoint)
        {
            return points.IndexOf(checkPoint);
        }

        public GameObject OtherCheckPoint(GameObject checkPoint)
        {
            if (CheckPointIndex(checkPoint) == 0) return points[1];
            else
                return points[0];
        }

        public Vector3 StreetInVector3()
        {
            return (points[1].transform.position - points[0].transform.position);
        }

        public bool IsOnTheRight(Street other, int intersectCheckPoint)
        {
            Vector3 curStreetVector = this.StreetInVector3().normalized;
            Vector3 otherStreetVector = other.StreetInVector3().normalized;
            if (!other.IsTwoWay && other.CheckPointIndex(this.points[intersectCheckPoint]) == 1) return false;
            if (intersectCheckPoint == 0)
            {
                curStreetVector = -curStreetVector;
            }

            float sinDelta = curStreetVector.x * otherStreetVector.z - curStreetVector.z * otherStreetVector.x;
            float cosDelta = curStreetVector.x * otherStreetVector.x + curStreetVector.z * otherStreetVector.z;
            
            if (!other.IsTwoWay||(other.IsTwoWay&& other.CheckPointIndex(this.points[intersectCheckPoint]) == 0))
            {
                if (Constants.sinBoundary > sinDelta &&
                    -Constants.cosBoundary < cosDelta && cosDelta < Constants.cosBoundary)
                    return true;
            }
            else
            {
                if (Constants.sinBoundary > -sinDelta &&
                -Constants.cosBoundary < -cosDelta && -cosDelta < Constants.cosBoundary)
                    return true;
            }
            return false;
        }

        public bool Equals(Street obj)
        {
            if (this.points == obj.points) return true;
            return false;
        }

        private float CalculateTimeToFinishStreet(int speed)
        {
            return StreetInVector3().magnitude / speed;
        }

        public bool CanGoIn(GameObject checkpoint, int speed)
        {
            if (!IsTwoWay)
            {
                if (checkpoint != points[0]) return false;
                else
                {
                    if (cars.Count < 1) return true;
                    else
                    {
                        float timeOnStreet = CalculateTimeToFinishStreet(speed);
                        for (int j = 0; j < cars.Count; j++)
                        {
                            if (timeOnStreet > cars[j].GetComponent<WaypointsProgressHandle>().TimeToCompleteStreet())
                                return false;
                        }
                        return true;
                    }
                }
            }
            else {
                if (cars.Count < 1) return true;
                else
                {
                    float timeOnStreet = CalculateTimeToFinishStreet(speed);
                    for (int j = 0; j < cars.Count; j++)
                    {
                        if (timeOnStreet > cars[j].GetComponent<WaypointsProgressHandle>().TimeToCompleteStreet())
                            return false;
                    }
                    return true;
                }
            }
        }
    }
}
