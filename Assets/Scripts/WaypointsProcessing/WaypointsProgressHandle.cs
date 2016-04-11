using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Entity;
using UnityEngine.Networking;
using UnityStandardAssets.Vehicles.Car;
namespace Assets.Scripts.WaypointsProcessing
{
    public struct RoutePoint
    {
        public Vector3 position;
        public Vector3 direction;


        public RoutePoint(Vector3 position, Vector3 direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }
    public class WaypointsProgressHandle : NetworkBehaviour
    {
        // This script can be used with any object that is supposed to follow a
        // route marked out by waypoints.

        // This script manages the amount to look ahead along the route,
        // and keeps track of progress and laps.

        [SerializeField]
        public List<GameObject> waypoints; // A reference to the waypoint-based route we should follow
        [SyncVar]
        public int carId = -1;
        public GameObject goalCheckPoint;

        public GameObject lastCheckpoint, nextCheckpoint;

        public Street currentStreet = null;


        [SerializeField]
        private float lookAheadForTargetOffset = 5;
        // The offset ahead along the route that the we will aim for

        [SerializeField]
        private float lookAheadForTargetFactor = .1f;
        // A multiplier adding distance ahead along the route to aim for, based on current speed

        [SerializeField]
        private float lookAheadForSpeedOffset = 10;
        // The offset ahead only the route for speed adjustments (applied as the rotation of the waypoint target transform)

        [SerializeField]
        private float lookAheadForSpeedFactor = .2f;
        // A multiplier adding distance ahead along the route for speed adjustments

        [SerializeField]
        private ProgressStyle progressStyle = ProgressStyle.SmoothAlongRoute;
        // whether to update the position smoothly along the route (good for curved paths) or just when we reach each waypoint.

        [SerializeField]
        private float pointToPointThreshold = 4;
        // proximity to waypoint which must be reached to switch target to next waypoint : only used in PointToPoint mode.

        public enum ProgressStyle
        {
            SmoothAlongRoute,
            PointToPoint,
            RandomMove
        }

        //// these are public, readable by other objects - i.e. for an AI to know where to head!
        //public WaypointCircuit.RoutePoint targetPoint { get; private set; }
        //public WaypointCircuit.RoutePoint speedPoint { get; private set; }
        public /*WaypointCircuit.RoutePoint*/ RoutePoint progressPoint { get; private set; }

        public List<GameObject> Waypoints
        {
            get
            {
                return waypoints;
            }

            set
            {
                this.waypoints = value;
                Reset();
            }
        }

        public Transform target;

        private float progressDistance; // The progress round the route, used in smooth mode.
        public int progressNum; // the current waypoint number, used in point-to-point mode.
        private Vector3 lastPosition; // Used to calculate current speed (since we may not have a rigidbody component)
        private float speed; // current speed of this object (calculated from delta since last frame)
        private bool canRun = false;

        void OnDestroy()
        {
            if (currentStreet != null)
                currentStreet.cars.Remove(gameObject);
        }

        // setup script properties


        private void OnEnable()
        {
            // we use a transform to represent the point to aim for, and the point which
            // is considered for upcoming changes-of-speed. This allows this component
            // to communicate this information to the AI without requiring further dependencies.

            // You can manually create a transform and assign it to this component *and* the AI,
            // then this component will update it, and the AI can read it.
            if (target == null)
            {
                target = new GameObject(name + " Waypoint Target").transform;
            }
            //GameController.Instance.GameStateChangeSubcribe(StartCar);

            Invoke("Init", 0.1f);
            //Reset();
        }

        private void OnDisable()
        {
            GameController.Instance.GameStateChangeUnSubcribe(StartCar);
        }

        public void StartCar()
        {
            //Init();
            //Debug.Log("Local player: " + isLocalPlayer);
            if (!isLocalPlayer&&NetworkServer.active)
            {
                if (GameController.Instance.GameState == GameStates.Run)
                {
                    canRun = true;
                    GetComponent<CarAIControl>().enabled = true;
                }
                else {
                    canRun = false;
                    GetComponent<CarAIControl>().enabled = false;
                }
            }
        }

        private void Init()
        {
            GameController.Instance.GameStateChangeSubcribe(StartCar);
            if (progressStyle == ProgressStyle.PointToPoint)
                UpdateWaypoints(Map.Instance.Routing(gameObject, goalCheckPoint));
            else
                if (progressStyle == ProgressStyle.RandomMove)
            {
                lastCheckpoint = null;
                nextCheckpoint = Map.Instance.GetNearestCheckpoint(gameObject);
                target.position = nextCheckpoint.transform.position;
            }
            StartCar();
        }

        // reset the object to sensible values
        public void Reset()
        {
            progressDistance = 0;
            progressNum = 0;
            if (progressStyle == ProgressStyle.PointToPoint)
            {
                target.position = Waypoints[progressNum].transform.position;
                //target.rotation = circuit.Waypoints[progressNum].rotation;
            }
        }

        private void UpdateWaypoints(List<GameObject> _waypoints)
        {
            this.waypoints = _waypoints;
            Reset();
        }

        private void Update()
        {
            if (canRun)
            {
                if (progressStyle == ProgressStyle.SmoothAlongRoute)
                {
                    return;
                }
                else
                if (progressStyle == ProgressStyle.PointToPoint)
                {
                    // point to point mode. Just increase the waypoint if we're close enough:
                    Vector3 targetDelta = target.position - transform.position;
                    if (targetDelta.magnitude < pointToPointThreshold)
                    {
                        if (waypoints[progressNum].GetComponent<CheckPoint>().state)
                        {
                            //green light => passable
                            progressNum = (progressNum + 1) % Waypoints.Count;
                            target.position = Waypoints[progressNum].transform.position;
                        }
                        else
                        {
                            //turn right and recalculate new route
                            GameObject newTarget = Map.Instance.CheckPointOnTheRight(waypoints[progressNum], waypoints[progressNum - 1]);
                            if (newTarget != null)
                            {
                                target.position = newTarget.transform.position;
                                UpdateWaypoints(Map.Instance.Routing(newTarget, goalCheckPoint));
                            }
                            else
                            {
                                //turn around
                                target.position = Waypoints[progressNum - 1].transform.position;
                                UpdateWaypoints(Map.Instance.Routing(Waypoints[progressNum - 1], goalCheckPoint));
                            }
                        }
                    }
                    else
                    {
                        target.position = Waypoints[progressNum].transform.position;
                    }
                }
                else {
                    if (progressStyle == ProgressStyle.RandomMove)
                    {
                        Vector3 targetDelta = target.position - transform.position;
                        if (targetDelta.magnitude < pointToPointThreshold)
                        {
                            //need to check for red light in here.
                            Debug.Log("close to checkpoint");
                            GameObject tmp;
                            if (nextCheckpoint.GetComponent<CheckPoint>().state)
                            {
                                //green light => move random.
                                tmp = Map.Instance.NextRandomCheckpoint(lastCheckpoint, nextCheckpoint, GetComponent<Rigidbody>().velocity.magnitude);
                            }
                            else
                            {
                                //red light=>turn right
                                tmp = Map.Instance.CheckPointOnTheRight(nextCheckpoint, lastCheckpoint);
                            }
                            if (tmp == null)
                            {
                                GetComponent<CarAIControl>().enabled = false;
                                GetComponent<Rigidbody>().velocity = Vector3.zero;
                                return;
                            }
                            else
                            {
                                if (!GetComponent<CarAIControl>().enabled)
                                    GetComponent<CarAIControl>().enabled = true;
                            }
                            //tmp == null need to brake and wait for green light

                            lastCheckpoint = nextCheckpoint;
                            nextCheckpoint = tmp;
                            target.position = nextCheckpoint.transform.position;

                            // register in new street and unregister in old street
                            if (currentStreet != null)
                                currentStreet.cars.Remove(gameObject);
                            currentStreet = Map.Instance.FindStreetFromCheckPoint(nextCheckpoint, lastCheckpoint);
                            currentStreet.cars.Add(gameObject);
                        }
                        else
                        {
                            target.position = nextCheckpoint.transform.position;
                        }
                    }
                }
            }
        }

        public float TimeToCompleteStreet()
        {
            return (target.position - transform.position).magnitude / GetComponent<Rigidbody>().velocity.magnitude;
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, target.position);
                //Gizmos.DrawWireSphere(circuit.GetRoutePosition(progressDistance), 1);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(target.position, target.position + target.forward);
            }
        }

    }
}
