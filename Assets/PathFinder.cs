using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Execution {
    Synchronous,
    Asynchronously
}
/// QPathFinder modified
/// <summary>
/// The class is responsible for creating vehicles and finding the shortest route
/// </summary>
public class PathFinder : MonoBehaviour {
    private static PathFinder _instance;////usunąc?
    public static PathFinder Instance => _instance;


    /// <summary>
    /// Stores a list of created vehicles
    /// </summary>
    List<Transform> cars = new List<Transform>();
    Transform carsBox;
    /// <summary>
    /// Stores the current number of vehicles
    /// </summary>
    public int amount = 0;
    /// <summary>
    /// Stores the maximum number of vehicles
    /// </summary>
    public int maxCars = 100;
    /// <summary>
    /// Stores how often vehicles are created
    /// </summary>
    public float spawnFrequency = 0.1f;
    /// <summary>
    /// Stores the amount of time vehicles spend at the workplace
    /// </summary>
    public float workDelay = 5f;
    /// <summary>
    /// Stores the amount of time vehicles spend in a trading place
    /// </summary>
    public float shopingDelay = 1f;
    /// <summary>
    /// Determines whether vehicles will be created from random streets for random purposes or according to a schedule
    /// </summary>
    public bool randomSpawn = false;
    /// <summary>
    /// Specifies whether the object is to create vehicles
    /// </summary>
    public bool spawning = true;
    /// <summary>
    /// Determines whether the object should save data at the end of the simulation
    /// </summary>
    public bool save = true;
    /// <summary>
    /// Specifies whether intersections should calculate the duration of each phase
    /// </summary>
    public bool calculateTimers = true;
    /// <summary>
    /// Specifies whether to display path lines
    /// </summary>
    public bool drawPaths = true;
    /// <summary>
    /// Specifies whether to show information about vehicle creation points
    /// </summary>
    public bool showSpawns = true;
    public bool countPassings = true;
    public bool showNodeId = false;
    public bool showPathId = false;
    public bool showCosts = false;
    int timeScale = 1;
    /// <summary>
    /// Defines the pace of the simulation
    /// </summary>
    public int TimeScale {
        get => timeScale;
        set {
            timeScale = value;
            Time.timeScale = timeScale;
        }
    }
    /// <summary>
    /// Stores the graph data
    /// </summary>
    [HideInInspector] public GraphData graphData = new GraphData();

    /// QPathFinder
    /// <summary>
    /// The method, which is run before the simulation starts, prepares the data for simulation
    /// </summary>
    void Awake() {
        _instance = this;
        foreach (Junction j in graphData.allJunctions) {
            j.globalTimersCalc = calculateTimers;
            foreach (Path p in j.paths) {
                p.entireQueue = 0;
            }
        }
        StartCoroutine(Spawn());
        StartCoroutine(RemoveCars());
    }

    /// QPathFinder
    /// <summary>
    /// Method run when the object is destroyed
    /// </summary>
    void OnDestroy() {
        _instance = null;
    }
    /// <summary>
    /// The method creates an array of places from which vehicles will leave and arrive
    /// </summary>
    /// <returns>A array storing 5 lists of places of creation: visitors, residents, commercial places, workplaces, people leaving</returns>
    List<int>[] MakeSpawnList() {
        List<int>[] spawns = new List<int>[5];//arrival / home / shop / work / departure
        for (int j = 0; j < 5; j++) {
            spawns[j] = new List<int>();
        }
        foreach (Street s in graphData.allStreets) {
            for (int j = 0; j < 5; j++) {
                for (int i = 0; i < s.spawns[j]; i++) {
                    if (s.center.ID != -1) {
                        spawns[j].Add(s.center.ID);
                    }
                }
            }
        }
        return spawns;
    }
    /// <summary>
    /// The method is responsible for creating the vehicle according to the schedule
    /// </summary>
    /// <param name="spawns">List of vehicle creation locations</param>
    void SpawnPredictably(List<int>[] spawns) {
        List<Node> nodePath = null, returnNodePath = null;
        int spawnType = -1, targetType = -1, spawn = -1, target = -1;
        while (nodePath == null || nodePath.Count == 0) {
            spawnType = Random.Range(0, 2);
            spawnType = spawns[spawnType].Count == 0 ? 1 - spawnType : spawnType;
            if (spawnType == 0) {//incoming
                targetType = 4;
                if (spawns[targetType].Count == 0) break;
            } else {//house
                targetType = Random.Range(0, 2);
                targetType = spawns[targetType + 2].Count == 0 ? 1 - targetType : targetType;
                targetType += 2;
                if (spawns[targetType].Count == 0) break;
            }
            spawn = Random.Range(0, spawns[spawnType].Count);//spawn of type sp
            int a = spawns[spawnType][spawn];
            int b = a;
            if (!spawns[targetType].Exists(item => item != a)) break;
            while (b == a) {
                target = Random.Range(0, spawns[targetType].Count);//target of type tar
                b = spawns[targetType][target];
            }
            nodePath = FindShortedPathSynchronousInternal(a, b);
            if (spawnType == 1) {//from house to work or shop come back
                returnNodePath = FindShortedPathSynchronousInternal(b, a);
            }
        }
        spawns[spawnType].RemoveAt(spawn);
        spawns[targetType].RemoveAt(target);
        List<Path> path = NodesToPath(nodePath);
        path[0].street.spawns[spawnType]--;
        if (targetType == 4) {
            path.Last().street.spawns[4]--;
        }
        PathFollower follower = SpawnCar();
        if (returnNodePath != null && returnNodePath.Count != 0) {
            List<Path> returnPath = NodesToPath(returnNodePath);
            follower.Follow(path, targetType, targetType == 2 ? shopingDelay : workDelay, returnPath);
        } else {
            follower.Follow(path);
        }
    }
    /// <summary>
    /// The method is responsible for creating a vehicle going to a random destination
    /// </summary>
    void SpawnRandom() {
        PathFollower follower = SpawnCar();
        List<Path> paths = RandomPath();
        follower.Follow(paths);
    }
    /// <summary>
    /// The method is responsible for creating the vehicle object
    /// </summary>
    /// <returns>Vehicle</returns>
    PathFollower SpawnCar() {
        if (!carsBox) {
            carsBox = (new GameObject("Cars")).transform;
        }
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localScale = new Vector3(0.4f, 0.4f, 0.8f);
        go.transform.parent = carsBox;
        cars.Add(go.transform);
        PathFollower follower = go.AddComponent<PathFollower>();
        return follower;
    }
    /// <summary>
    /// The method is responsible for finding a route to a random node
    /// </summary>
    /// <returns>List of paths</returns>
    List<Path> RandomPath() {
        List<Node> nodes = null;
        while (nodes == null || nodes.Count == 0) {
            int a = Random.Range(0, graphData.centers.Count);
            int b = a;
            while (b == a) {
                b = Random.Range(0, graphData.centers.Count);
            }
            Node spawn = graphData.centers[a];
            Node target = graphData.centers[b];
            nodes = FindShortedPathSynchronousInternal(spawn.ID, target.ID);
        }
        List<Path> path = NodesToPath(nodes);
        return path;
    }
    /// <summary>
    /// Routine is responsible for creating vehicles
    /// </summary>
    /// <returns></returns>
    IEnumerator Spawn() {
        List<int>[] spawns = MakeSpawnList();
        while (true) {
            if (spawning && maxCars > amount) {
                if (randomSpawn) {
                    SpawnRandom();
                    amount++;
                } else {
                    if ((spawns[0].Count != 0 || spawns[1].Count != 0) && (spawns[2].Count != 0 || spawns[3].Count != 0 || spawns[4].Count != 0)) {
                    SpawnPredictably(spawns);
                    amount++;
                    }
                }
            }
            yield return new WaitForSeconds(spawnFrequency);
        }
    }
    /// <summary>
    /// Routine is responsible for removing vehicles that have reached their destination
    /// </summary>
    IEnumerator RemoveCars() {
        while (true) {
            cars.RemoveAll(item => item == null);
            amount = cars.Count;
            yield return new WaitForSeconds(2);
        }
    }
    /// <summary>
    /// The method is responsible for converting the list of vertices into a list of paths
    /// </summary>
    /// <param name="nodes">List of nodes</param>
    /// <returns>List of paths</returns>
    List<Path> NodesToPath(List<Node> nodes) {
        List<Path> paths = new List<Path>();
        for (int i = 0; i < nodes.Count - 1; i++) {
            Path path = graphData.GetPathBetween(nodes[i].ID, nodes[i + 1].ID);
            if (path == null) { return null; }
            paths.Add(path);
        }
        return paths;
    }
    // QPathFinder
    /// <summary>
    /// The method is responsible for finding the path between two vertices
    /// </summary>
    // <param name="fromNodeID">ID of first Node</param>
    // <param name="toNodeID">ID of second Node</param>
    // <returns>List nodes on paths</returns>
    private List<Node> FindShortedPathSynchronousInternal(int fromNodeID, int toNodeID) {
        int startPointID = fromNodeID;
        int endPointID = toNodeID;

        graphData.ReGenerateIDs();

        Node startPoint = graphData.nodes[startPointID];
        Node endPoint = graphData.nodes[endPointID];

        foreach (var point in graphData.nodes) {
            point.heuristicDistance = -1;
            point.previousNode = null;
        }

        List<Node> completedPoints = new List<Node>();
        List<Node> nextPoints = new List<Node>();
        List<Node> finalPath = new List<Node>();

        startPoint.pathDistance = 0;
        startPoint.heuristicDistance = Vector3.Distance(startPoint.position, endPoint.position);
        nextPoints.Add(startPoint);

        while (true) {
            Node leastCostPoint = null;

            float minCost = 99999;
            foreach (var point in nextPoints) {
                if (point.heuristicDistance <= 0)
                    point.heuristicDistance = Vector3.Distance(point.position, endPoint.position);

                if (minCost > point.CombinedHeuristic) {
                    leastCostPoint = point;
                    minCost = point.CombinedHeuristic;
                }
            }

            if (leastCostPoint == null)
                break;

            if (leastCostPoint == endPoint) {
                Node prevPoint = leastCostPoint;
                while (prevPoint != null) {
                    finalPath.Insert(0, prevPoint);
                    prevPoint = prevPoint.previousNode;
                }
                return finalPath;
            }

            foreach (var path in graphData.paths) {
                if (path.IDOfA == leastCostPoint.ID) {

                    Node otherPoint = graphData.nodes[path.IDOfB];

                    if (otherPoint.heuristicDistance <= 0)
                        otherPoint.heuristicDistance = Vector3.Distance(otherPoint.position, endPoint.position);

                    if (completedPoints.Contains(otherPoint))
                        continue;

                    float leastCostDistance = leastCostPoint.pathDistance + path.Cost;
                    if (nextPoints.Contains(otherPoint)) {
                        if (otherPoint.pathDistance > leastCostDistance) {
                            otherPoint.pathDistance = leastCostDistance;
                            otherPoint.previousNode = leastCostPoint;
                        }
                    } else {
                        otherPoint.pathDistance = leastCostDistance;
                        otherPoint.previousNode = leastCostPoint;
                        nextPoints.Add(otherPoint);
                    }
                }
            }
            nextPoints.Remove(leastCostPoint);
            completedPoints.Add(leastCostPoint);
        }
        return null;
    }

    /// QPathFinder not used
    /// <param name="fromNodeID"></param>
    /// <param name="toNodeID"></param>
    /// <param name="callback"></param>
    IEnumerator FindShortestPathAsynchonousInternal(int fromNodeID, int toNodeID, System.Action<List<Node>> callback) {
        if (callback == null)
            yield break;

        int startPointID = fromNodeID;
        int endPointID = toNodeID;

        //graphData.ReGenerateIDs();

        Node startPoint = graphData.nodes[startPointID];
        Node endPoint = graphData.nodes[endPointID];

        foreach (var point in graphData.nodes) {
            point.heuristicDistance = -1;
            point.previousNode = null;
        }

        List<Node> completedPoints = new List<Node>();
        List<Node> nextPoints = new List<Node>();
        List<Node> finalPath = new List<Node>();

        startPoint.pathDistance = 0;
        startPoint.heuristicDistance = Vector3.Distance(startPoint.position, endPoint.position);
        nextPoints.Add(startPoint);

        while (true) {
            Node leastCostPoint = null;

            float minCost = 99999;
            foreach (var point in nextPoints) {
                if (point.heuristicDistance <= 0)
                    point.heuristicDistance = Vector3.Distance(point.position, endPoint.position) * 2;

                if (minCost > point.CombinedHeuristic) {
                    leastCostPoint = point;
                    minCost = point.CombinedHeuristic;
                }
            }

            if (leastCostPoint == null)
                break;

            if (leastCostPoint == endPoint) {
                Node prevPoint = leastCostPoint;
                while (prevPoint != null) {
                    finalPath.Insert(0, prevPoint);
                    prevPoint = prevPoint.previousNode;
                }
                callback(finalPath);
                yield break;
            }

            foreach (var path in graphData.paths) {
                if (path.IDOfA == leastCostPoint.ID
                || path.IDOfB == leastCostPoint.ID) {
                    if (leastCostPoint.ID == path.IDOfB) {
                        continue;
                    }

                    Node otherPoint = path.IDOfA == leastCostPoint.ID ? graphData.nodes[path.IDOfB] : graphData.nodes[path.IDOfA];

                    if (otherPoint.heuristicDistance <= 0)
                        otherPoint.heuristicDistance = Vector3.Distance(otherPoint.position, endPoint.position) + Vector3.Distance(otherPoint.position, startPoint.position);

                    if (completedPoints.Contains(otherPoint))
                        continue;

                    if (nextPoints.Contains(otherPoint)) {
                        if (otherPoint.pathDistance >
                            (leastCostPoint.pathDistance + path.Cost)) {
                            otherPoint.pathDistance = leastCostPoint.pathDistance + path.Cost;
                            otherPoint.previousNode = leastCostPoint;
                        }
                    } else {
                        otherPoint.pathDistance = leastCostPoint.pathDistance + path.Cost;
                        otherPoint.previousNode = leastCostPoint;
                        nextPoints.Add(otherPoint);
                    }
                }
            }
            nextPoints.Remove(leastCostPoint);
            completedPoints.Add(leastCostPoint);

            yield return null;
        }

        callback(null);
        yield break;
    }


    // QPathFinder not used
    // Finds shortest path between Nodes.
    // Once the path if found, it will return the path as List of nodes (not positions, but nodes. If you need positions, use FindShortestPathOfPoints). 
    // <returns> Returns list of **Nodes**</returns>
    // <param name="fromNodeID">Find the path from this node</param>
    // <param name="toNodeID">Find the path to this node</param>
    // <param name="executionType">Synchronous is immediate and locks the control till path is found and returns the path. 
    // Asynchronous type runs in coroutines without locking the control. If you have more than 50 Nodes, Asynchronous is recommended</param>
    // <param name="callback">Callback once the path is found</param>
    public void FindShortestPathOfNodes(int fromNodeID, int toNodeID, Execution executionType, System.Action<List<Node>> callback) {
        if (executionType == Execution.Asynchronously) {
            StartCoroutine(FindShortestPathAsynchonousInternal(fromNodeID, toNodeID, callback));
        } else {
            callback(FindShortedPathSynchronousInternal(fromNodeID, toNodeID));
        }
    }

    // QPathFinder not used
    // <summary>
    // </summary>
    // <param name="point"></param>
    // <returns></returns>
    public int FindNearestNode(Vector3 point) {
        float minDistance = float.MaxValue;
        Node nearestNode = null;

        foreach (var node in graphData.nodes) {
            if (Vector3.Distance(node.position, point) < minDistance) {
                minDistance = Vector3.Distance(node.position, point);
                nearestNode = node;
            }
        }
        return nearestNode != null ? nearestNode.ID : -1;
    }

}
