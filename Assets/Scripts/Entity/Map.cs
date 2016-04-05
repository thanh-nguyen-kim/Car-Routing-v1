using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts.Entity
{
    class Item
    {
        public int value;
        public int priority;

        public Item(int value, int priority)
        {
            this.value = value;
            this.priority = priority;
        }
    }
    class PriorityQueue
    {

        private List<Item> elements = new List<Item>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(int val, int priority)
        {
            elements.Add(new Item(val, priority));
        }

        public int Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].priority < elements[bestIndex].priority)
                {
                    bestIndex = i;
                }
            }

            int bestItem = elements[bestIndex].value;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }

    public class Map : MonoBehaviour
    {
        public List<GameObject> checkPoints;
        public List<Street> streets;
        private int[,] map;

        private static Map instance;
        private List<Action> mapChangeSubcriber = new List<Action>();

        public static Map Instance
        {
            get
            {
                return instance;
            }
        }

        //make matrix of weight graph from map
        private void MakeGraph()
        {
            map = new int[checkPoints.Count, checkPoints.Count];
            for (int i = 0; i < checkPoints.Count; i++)
                for (int j = 0; j < checkPoints.Count; j++)
                {
                    if (i != j)
                        map[i, j] = int.MaxValue;
                    else
                        map[i, j] = 0;
                }


            foreach (Street street in streets)
            {
                GameObject a = street.points[0];
                GameObject b = street.points[1];
                int i = -1;
                int j = -1;
                i = FindCheckPointIndex(a);
                j = FindCheckPointIndex(b);
                if (i == -1 || j == -1) continue;
                //check red bulb.
                if (!a.GetComponent<CheckPoint>().state ||
                    !b.GetComponent<CheckPoint>().state) continue;
                if (street.IsTwoWay) map[j, i] = 1;
                map[i, j] = 1;
            }
        }

        private int FindCheckPointIndex(GameObject chkpoint)
        {
            foreach (GameObject cp in checkPoints)
            {
                if (cp.Equals(chkpoint))
                    return checkPoints.IndexOf(cp);
            }
            return -1;
        }
        //implement Dijkstra Algorithms
        public List<GameObject> Routing(GameObject currentGameobject, GameObject goalCheckPoint)
        {
            int start = FindCheckPointIndex(GetNearestCheckpoint(currentGameobject));
            int goal = FindCheckPointIndex(GetNearestCheckpoint(goalCheckPoint));

            Debug.Log(start + "   " + goal);

            int[] costFromStart = new int[checkPoints.Count];

            int[] previousNode = new int[checkPoints.Count];
            PriorityQueue frontier = new PriorityQueue();

            frontier.Enqueue(start, 0);

            //initiate cost array
            for (int i = 0; i < checkPoints.Count; i++)
            {
                costFromStart[i] = int.MaxValue;
                previousNode[i] = -1;
            }

            previousNode[start] = start;
            costFromStart[start] = 0;

            while (frontier.Count != 0)
            {
                int current = frontier.Dequeue();

                //Debug.Log(costFromStart[current]);

                if (current == goal)
                {
                    PrintPath(previousNode, start, goal);
                    return GetPathFromArray(previousNode, start, goal);
                }

                for (int next = 0; next < checkPoints.Count; next++)
                {
                    if (map[current, next] >= int.MaxValue || current == next) continue;
                    int newCost = costFromStart[current] + 1;
                    if (costFromStart[next] > newCost)
                    {
                        costFromStart[next] = newCost;
                        frontier.Enqueue(next, newCost);//newCost acts as priority
                        previousNode[next] = current;
                    }
                }
            }
            List<GameObject> path = new List<GameObject>();
            path.Add(GetNearestCheckpoint(currentGameobject));
            return path;
            //PrintPath(previousNode, start, goal);
        }

        private void PrintPath(int[] listNode, int start, int goal)
        {
            int current = goal;
            string path = "";
            path += goal.ToString();
            while (current != start)
            {
                path += "-" + listNode[current].ToString();
                current = listNode[current];
            }
            Debug.Log(path);
        }
        public GameObject GetNearestCheckpoint(GameObject target)
        {
            float distance = int.MaxValue;
            GameObject nearestCheckpoint = null;
            foreach (GameObject go in checkPoints)
            {
                if (Vector3.Distance(go.transform.position, target.transform.position) < distance)
                {
                    nearestCheckpoint = go;
                    distance = Vector3.Distance(go.transform.position, target.transform.position);
                }
            }
            return nearestCheckpoint;

        }
        private List<GameObject> GetPathFromArray(int[] listNode, int start, int goal)
        {
            List<GameObject> path = new List<GameObject>();
            int current = goal;
            while (current != start)
            {
                //path += "-" + listNode[current].ToString();
                path.Add(checkPoints[current]);
                current = listNode[current];
            }
            path.Add(checkPoints[start]);
            if (path.Count != 0)
            {
                path.Reverse();
                return path;
            }

            return null;
        }
        private float HeuristicDistanceEstimate(int current, int goal)
        {
            return Vector3.Distance(checkPoints[current].transform.position, checkPoints[goal].transform.position);
        }
        private void PrintMap()
        {
            string mapStr = "";
            for (int i = 0; i < checkPoints.Count; i++)
            {
                for (int j = 0; j < checkPoints.Count; j++)
                {
                    if (j == 0) mapStr += "\n";
                    mapStr += map[i, j].ToString() + " ";
                }
            }
            Debug.Log(mapStr);
        }

        public Street FindStreetFromCheckPoint(GameObject target, GameObject lastOne)
        {
            foreach (Street street in streets)
            {
                if (street.ContentCheckPoint(target) && street.ContentCheckPoint(lastOne))
                    return street;
            }
            return null;

        }

        public GameObject NextRandomCheckpoint(GameObject lastCheckpoint, GameObject nextCheckpoint, float speed)
        {
            if (lastCheckpoint == null) {
                for (int i = 0; i < streets.Count; i++) {
                    if (streets[i].ContentCheckPoint(nextCheckpoint)) return streets[i].OtherCheckPoint(nextCheckpoint);
                }
            }
            List<GameObject> targets = new List<GameObject>();
            Street currentStreet = FindStreetFromCheckPoint(nextCheckpoint, lastCheckpoint);

            bool direction = false;

            if (currentStreet.CheckPointIndex(nextCheckpoint) == 1)
                direction = true;

            int checkPointIntersect = 1;
            List<Street> intersectStreet = new List<Street>();
            if (!direction) checkPointIntersect = 0;
            foreach (Street street in streets)
            {
                if (street == currentStreet) continue;
                if (street.ContentCheckPoint(currentStreet.points[checkPointIntersect]))
                {
                    intersectStreet.Add(street);
                }
            }

            if (intersectStreet.Count == 0)
            {
//                Debug.Log("Not intersect");
                if (currentStreet.IsTwoWay) return currentStreet.OtherCheckPoint(currentStreet.points[checkPointIntersect]);
                return null;
            }
            else
            {
                foreach (Street street in intersectStreet)
                {
                    if (street.CanGoIn(currentStreet.points[checkPointIntersect], (int)speed))
                        targets.Add(street.OtherCheckPoint(currentStreet.points[checkPointIntersect]));
                 }
            }
            if (targets.Count > 0)
                return targets[UnityEngine.Random.Range(0, targets.Count)];
            else
                if (currentStreet.IsTwoWay) return currentStreet.OtherCheckPoint(currentStreet.points[checkPointIntersect]);
            return null;

        }

        public GameObject CheckPointOnTheRight(GameObject target, GameObject lastOne)
        {
            Street currentStreet = FindStreetFromCheckPoint(target, lastOne);

            bool direction = false;

            if (currentStreet.CheckPointIndex(target) == 1)
                direction = true;

            int checkPointIntersect = 1;
            List<Street> intersectStreet = new List<Street>();
            if (!direction) checkPointIntersect = 0;
            foreach (Street street in streets)
            {
                if (street == currentStreet) continue;
                if (street.ContentCheckPoint(currentStreet.points[checkPointIntersect]))
                {
                    intersectStreet.Add(street);
                }
            }

            if (intersectStreet.Count == 0)
            {
                Debug.Log("Not intersect");
                return null;
            }
            else {
                foreach (Street street in intersectStreet)
                {
                    if (currentStreet.IsOnTheRight(street, checkPointIntersect))
                    {
  //                      Debug.Log("Found one in the right");
                        return street.OtherCheckPoint(currentStreet.points[checkPointIntersect]);
                    }
                    else
                    {
   //                     Debug.Log("Cant found one in the right");
                        return null;
                    }
                }
                return null;

            }

        }

        private void OnDrawGizmos()
        {
            foreach (Street street in streets)
            {
                GameObject a = street.points[0];
                GameObject b = street.points[1];

                //check red bulb.
                if (!a.GetComponent<CheckPoint>().state ||
                    !b.GetComponent<CheckPoint>().state)
                {
                    Gizmos.color = Color.red;
                }
                else {
                    if (street.IsTwoWay)
                        Gizmos.color = Color.blue;
                    else
                        Gizmos.color = Color.yellow;
                }
                Gizmos.DrawLine(a.transform.position, b.transform.position);
            }

        }


        void Start()
        {
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
                DestroyImmediate(gameObject);
            //Debug.Log(Quaternion.Angle(Quaternion.Euler(Vector3.zero), Quaternion.Euler(Vector3.forward*90)));
            //Debug.Log(Quaternion.Angle(Quaternion.Euler(Vector3.zero), Quaternion.Euler(-Vector3.forward*90)));
            //Debug.Log(Vector2.Angle(Vector2.right, Vector2.up));
            //Debug.Log(Vector2.Angle(Vector2.right, -Vector2.up));
            MakeGraph();
            //CheckPointOnTheRight(checkPoints[1], checkPoints[0]);
            //StreetOnTheRight(streets[0], true);
            //Routing(0, 2);
        }
    }
}
