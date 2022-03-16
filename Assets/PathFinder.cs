using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public enum Execution {
    Synchronous,
    Asynchronously
}
/// QPathFinder modified
/// <summary>
/// The class is responsible for creating vehicles and finding the shortest route
/// </summary>
public class PathFinder : MonoBehaviour
{
    public List<GameObject> carPrefab;
    public Slider speedSlider;
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
    public float spawnFrequency = 2f;
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
    public int TimeScale
    {
        get => timeScale;
        set
        {
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
    void Awake()
    {
        _instance = this;
        foreach (Junction j in graphData.allJunctions)
        {
            j.globalTimersCalc = calculateTimers;
            foreach (Path p in j.paths)
            {
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
    void OnDestroy()
    {
        _instance = null;
    }
    /// <summary>
    /// The method creates an array of places from which vehicles will leave and arrive
    /// </summary>
    /// <returns>A array storing 5 lists of places of creation: visitors, residents, commercial places, workplaces, people leaving</returns>
    List<int>[] MakeSpawnList()
    {
        List<int>[] spawns = new List<int>[5];//arrival / home / shop / work / departure
        for (int j = 0; j < 5; j++)
        {
            spawns[j] = new List<int>();
        }
        foreach (Street s in graphData.allStreets)
        {
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < s.spawns[j]; i++)
                {
                    if (s.center.ID != -1)
                    {
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
    void SpawnPredictably(List<int>[] spawns)
    {
        List<Node> nodePath = null, returnNodePath = null;
        int spawnType = -1, targetType = -1, spawn = -1, target = -1;
        while (nodePath == null || nodePath.Count == 0)
        {
            spawnType = Random.Range(0, 2);
            spawnType = spawns[spawnType].Count == 0 ? 1 - spawnType : spawnType;
            if (spawnType == 0)
            {//incoming
                targetType = 4;
                if (spawns[targetType].Count == 0) break;
            }
            else
            {//house
                targetType = Random.Range(0, 2);
                targetType = spawns[targetType + 2].Count == 0 ? 1 - targetType : targetType;
                targetType += 2;
                if (spawns[targetType].Count == 0) break;
            }
            spawn = Random.Range(0, spawns[spawnType].Count);//spawn of type sp
            int a = spawns[spawnType][spawn];
            int b = a;
            if (!spawns[targetType].Exists(item => item != a)) break;
            while (b == a)
            {
                target = Random.Range(0, spawns[targetType].Count);//target of type tar
                b = spawns[targetType][target];
            }
            nodePath = FindShortedPathSynchronousInternal(a, b);
            if (spawnType == 1)
            {//from house to work or shop come back
                returnNodePath = FindShortedPathSynchronousInternal(b, a);
            }
        }
        spawns[spawnType].RemoveAt(spawn);
        spawns[targetType].RemoveAt(target);
        List<Path> path = NodesToPath(nodePath);
        path[0].street.spawns[spawnType]--;
        if (targetType == 4)
        {
            path.Last().street.spawns[4]--;
        }
        PathFollower follower = SpawnCar();
        if (returnNodePath != null && returnNodePath.Count != 0)
        {
            List<Path> returnPath = NodesToPath(returnNodePath);
            follower.Follow(path, targetType, targetType == 2 ? shopingDelay : workDelay, returnPath);
        }
        else
        {
            follower.Follow(path);
        }
    }
    /// <summary>
    /// The method is responsible for creating a vehicle going to a random destination
    /// </summary>
    void SpawnRandom()
    {
        PathFollower follower = SpawnCar();
        List<Path> paths = RandomPath();
        follower.Follow(paths);
    }
    /// <summary>
    /// The method is responsible for creating the vehicle object
    /// </summary>
    /// <returns>Vehicle</returns>
    PathFollower SpawnCar()
    {
        if (!carsBox)
        {
            carsBox = (new GameObject("Cars")).transform;
        }
        int random = Random.Range(0, 3);
        GameObject go = GameObject.Instantiate(carPrefab[random]);

        if (random == 0)
        {
            go.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //go.transform.localScale = new Vector3(0.4f, 0.4f, 0.8f);
        go.transform.parent = carsBox;
        cars.Add(go.transform);
        PathFollower follower = go.AddComponent<PathFollower>();
        return follower;
    }
    /// <summary>
    /// The method is responsible for finding a route to a random node
    /// </summary>
    /// <returns>List of paths</returns>
    List<Path> RandomPath()
    {
        List<Node> nodes = null;
        while (nodes == null || nodes.Count == 0)
        {
            int a = Random.Range(0, graphData.centers.Count);
            int b = a;
            while (b == a)
            {
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
    IEnumerator Spawn()
    {
        List<int>[] spawns = MakeSpawnList();
        while (true)
        {
            if (spawning && maxCars > amount)
            {
                if (randomSpawn)
                {
                    SpawnRandom();
                    amount++;
                }
                else
                {
                    if ((spawns[0].Count != 0 || spawns[1].Count != 0) && (spawns[2].Count != 0 || spawns[3].Count != 0 || spawns[4].Count != 0))
                    {
                        SpawnPredictably(spawns);
                        amount++;
                    }
                }
            }
            yield return new WaitForSeconds(spawnFrequency);
        }
    }
   public IEnumerator Spawn(int numberCar, string street1, string street2, int idCar)
    {
        int i = 0;
        float delay = 0f;
        while (i<numberCar)
        {
            spawCarInStreet(street1, street2, idCar);
            amount++;
            i++;
            delay = 60f / (float)numberCar;
            /*if (idCar == 0)
            {
                delay = 4f;
            }else if(idCar == 1)
            {
                delay = 5f;
            }else if (idCar == 2)
            {
                delay = 8f;
            }
            else
            {
                delay = 15f;
            }*/
            yield return new WaitForSeconds(delay);
        }
    }
    public  IEnumerator delaySpawn(float time)
    {
        yield return new WaitForSeconds(time);
    }
    private int getIDFromStreet(string streetName)
    {
        int a = 0;
        foreach (Street str in graphData.allStreets)
        {
            if (streetName == str.Name)
            {
                a = str.center.ID;
                break;
            }
        }
        return a;
    }
    private string getStreetFromID(int id)
    {
        string tmp="";
        foreach(Street str in graphData.allStreets)
        {
            if(id == str.center.ID)
            {
                tmp = str.Name;
            }
        }
        return tmp;
    }

    private int getNumStreetFromID(int id)
    {
        int i = 0;
        foreach (Street str in graphData.allStreets)
        {
            if (id == str.center.ID)
            {
                return i;
            }
            i++;
        }
        return -1;
    }

    private Node getNodeFromID(int id)
    {
        Node tmp = null;
        foreach (Street str in graphData.allStreets)
        {
            if (id == str.center.ID)
            {
                tmp = str.center;
            }
        }
        return tmp;
    }

    public IEnumerator Spawn(int numberCar, string street1, int idCar)
    {
        int i = 0;
        float delay = 0f;
        int maxRangeNeighbor = 50;
        int idStreet1 = getIDFromStreet(street1);
        int numStreet1 = getNumStreetFromID(idStreet1);
        int numStreet2 = numStreet1;
        
        if (numStreet1 >= 0)
        {
            int RangeNeighbor = Random.Range(maxRangeNeighbor - 5, maxRangeNeighbor);
            if (numStreet1 <= RangeNeighbor) numStreet2 = numStreet1 + RangeNeighbor;
            else
            {
                if (numStreet1 >= graphData.allStreets.Count - RangeNeighbor) numStreet2 = numStreet1 - RangeNeighbor;
                else numStreet2 = numStreet1 + RangeNeighbor;
            }
        }
        /*while (idStreet2 == idStreet1)
        {
            int t = Random.Range(0, graphData.centers.Count);
            idStreet2 = graphData.centers[t].ID;
            
        }*/
        // Tất cả các xe lẻ đi theo tuyến đường 1
        // Tất cả các xe chẵn đi theo tuyến đường 2
        int a1 = graphData.allStreets[numStreet1].center.ID;
        int b1 = graphData.allStreets[numStreet2].center.ID;
        List<Path> currentPaths1 = AddRealtimePath(a1, b1);
        //Debug.Log("Select street 1: " + a1 + " to " + b1);
        numStreet1 = getNumStreetFromID(idStreet1);
        numStreet2 = numStreet1;
        if (numStreet1 >= 0)
        {
            int RangeNeighbor = Random.Range(maxRangeNeighbor - 8, maxRangeNeighbor - 3);
            if (numStreet1 <= RangeNeighbor) numStreet2 = numStreet1 + RangeNeighbor;
            else
            {
                if (numStreet1 >= graphData.allStreets.Count - RangeNeighbor) numStreet2 = numStreet1 - RangeNeighbor;
                else numStreet2 = numStreet1 + RangeNeighbor;
            }
        }
        int a2 = graphData.allStreets[numStreet1].center.ID;
        int b2 = graphData.allStreets[numStreet2].center.ID;
        List<Path> currentPaths2 = AddRealtimePath(a2, b2);
        //Debug.Log("Select street 2: " + a2 + " to " + b2);
        while (i < numberCar)
        {
            //spawCarInStreet(numStreet1, numStreet2, idCar);
            
            //Debug.Log(a + "," + b);
            PathFollower follower = SpawnCarRealtime(idCar);
            //List<Path> paths = AddRealtimePath(a, b);
            if (i % 2 == 0)
                follower.Follow(currentPaths1);
            else
                follower.Follow(currentPaths2);
            amount++;
            i++;

            delay = 60f / (float)numberCar; //+ 0.2f;

            //if (idCar == 0)
            //{
            //    delay = 1f;
            //}
            //else if (idCar == 1)
            //{
            //    delay = 1f;
            //}
            //else if (idCar == 2)
            //{
            //    delay = 1f;
            //}
            //else
            //{
            //    delay = 1f;
            //}
            yield return new WaitForSeconds(delay);// + 0.5f);
            //yield return new WaitForSeconds(1f);
        }
    }
    /// <summary>
    /// Routine is responsible for removing vehicles that have reached their destination
    /// </summary>
    IEnumerator RemoveCars()
    {
        while (true)
        {
            cars.RemoveAll(item => item == null);
            amount = cars.Count;
            yield return new WaitForSeconds(1);
        }
    }
    /// <summary>
    /// The method is responsible for converting the list of vertices into a list of paths
    /// </summary>
    /// <param name="nodes">List of nodes</param>
    /// <returns>List of paths</returns>
    List<Path> NodesToPath(List<Node> nodes)
    {
        List<Path> paths = new List<Path>();
        for (int i = 0; i < nodes.Count - 1; i++)
        {
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
    private List<Node> FindShortedPathSynchronousInternal(int fromNodeID, int toNodeID)
    {
        int startPointID = fromNodeID;
        int endPointID = toNodeID;

        graphData.ReGenerateIDs();

        Node startPoint = graphData.nodes[startPointID];
        Node endPoint = graphData.nodes[endPointID];

        foreach (var point in graphData.nodes)
        {
            point.heuristicDistance = -1;
            point.previousNode = null;
        }

        List<Node> completedPoints = new List<Node>();
        List<Node> nextPoints = new List<Node>();
        List<Node> finalPath = new List<Node>();

        startPoint.pathDistance = 0;
        startPoint.heuristicDistance = Vector3.Distance(startPoint.position, endPoint.position);
        nextPoints.Add(startPoint);

        while (true)
        {
            Node leastCostPoint = null;

            float minCost = 99999;
            foreach (var point in nextPoints)
            {
                if (point.heuristicDistance <= 0)
                    point.heuristicDistance = Vector3.Distance(point.position, endPoint.position);

                if (minCost > point.CombinedHeuristic)
                {
                    leastCostPoint = point;
                    minCost = point.CombinedHeuristic;
                }
            }

            if (leastCostPoint == null)
                break;

            if (leastCostPoint == endPoint)
            {
                Node prevPoint = leastCostPoint;
                while (prevPoint != null)
                {
                    finalPath.Insert(0, prevPoint);
                    prevPoint = prevPoint.previousNode;
                }
                return finalPath;
            }

            foreach (var path in graphData.paths)
            {
                if (path.IDOfA == leastCostPoint.ID)
                {

                    Node otherPoint = graphData.nodes[path.IDOfB];

                    if (otherPoint.heuristicDistance <= 0)
                        otherPoint.heuristicDistance = Vector3.Distance(otherPoint.position, endPoint.position);

                    if (completedPoints.Contains(otherPoint))
                        continue;

                    float leastCostDistance = leastCostPoint.pathDistance + path.Cost;
                    if (nextPoints.Contains(otherPoint))
                    {
                        if (otherPoint.pathDistance > leastCostDistance)
                        {
                            otherPoint.pathDistance = leastCostDistance;
                            otherPoint.previousNode = leastCostPoint;
                        }
                    }
                    else
                    {
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
    IEnumerator FindShortestPathAsynchonousInternal(int fromNodeID, int toNodeID, System.Action<List<Node>> callback)
    {
        if (callback == null)
            yield break;

        int startPointID = fromNodeID;
        int endPointID = toNodeID;

        //graphData.ReGenerateIDs();

        Node startPoint = graphData.nodes[startPointID];
        Node endPoint = graphData.nodes[endPointID];

        foreach (var point in graphData.nodes)
        {
            point.heuristicDistance = -1;
            point.previousNode = null;
        }

        List<Node> completedPoints = new List<Node>();
        List<Node> nextPoints = new List<Node>();
        List<Node> finalPath = new List<Node>();

        startPoint.pathDistance = 0;
        startPoint.heuristicDistance = Vector3.Distance(startPoint.position, endPoint.position);
        nextPoints.Add(startPoint);

        while (true)
        {
            Node leastCostPoint = null;

            float minCost = 99999;
            foreach (var point in nextPoints)
            {
                if (point.heuristicDistance <= 0)
                    point.heuristicDistance = Vector3.Distance(point.position, endPoint.position) * 2;

                if (minCost > point.CombinedHeuristic)
                {
                    leastCostPoint = point;
                    minCost = point.CombinedHeuristic;
                }
            }

            if (leastCostPoint == null)
                break;

            if (leastCostPoint == endPoint)
            {
                Node prevPoint = leastCostPoint;
                while (prevPoint != null)
                {
                    finalPath.Insert(0, prevPoint);
                    prevPoint = prevPoint.previousNode;
                }
                callback(finalPath);
                yield break;
            }

            foreach (var path in graphData.paths)
            {
                if (path.IDOfA == leastCostPoint.ID
                || path.IDOfB == leastCostPoint.ID)
                {
                    if (leastCostPoint.ID == path.IDOfB)
                    {
                        continue;
                    }

                    Node otherPoint = path.IDOfA == leastCostPoint.ID ? graphData.nodes[path.IDOfB] : graphData.nodes[path.IDOfA];

                    if (otherPoint.heuristicDistance <= 0)
                        otherPoint.heuristicDistance = Vector3.Distance(otherPoint.position, endPoint.position) + Vector3.Distance(otherPoint.position, startPoint.position);

                    if (completedPoints.Contains(otherPoint))
                        continue;

                    if (nextPoints.Contains(otherPoint))
                    {
                        if (otherPoint.pathDistance >
                            (leastCostPoint.pathDistance + path.Cost))
                        {
                            otherPoint.pathDistance = leastCostPoint.pathDistance + path.Cost;
                            otherPoint.previousNode = leastCostPoint;
                        }
                    }
                    else
                    {
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
    public void FindShortestPathOfNodes(int fromNodeID, int toNodeID, Execution executionType, System.Action<List<Node>> callback)
    {
        if (executionType == Execution.Asynchronously)
        {
            StartCoroutine(FindShortestPathAsynchonousInternal(fromNodeID, toNodeID, callback));
        }
        else
        {
            callback(FindShortedPathSynchronousInternal(fromNodeID, toNodeID));
        }
    }

    // QPathFinder not used
    // <summary>
    // </summary>
    // <param name="point"></param>
    // <returns></returns>
    public int FindNearestNode(Vector3 point)
    {
        float minDistance = float.MaxValue;
        Node nearestNode = null;

        foreach (var node in graphData.nodes)
        {
            if (Vector3.Distance(node.position, point) < minDistance)
            {
                minDistance = Vector3.Distance(node.position, point);
                nearestNode = node;
            }
        }
        return nearestNode != null ? nearestNode.ID : -1;
    }

    public Node FindNearestNode2(Vector3 point, List<Node> currentNode)
    {
        float minDistance = float.MaxValue;
        Node nearestNode = null;
        bool stop = false;
        foreach (var node in graphData.nodes)
        {
            stop = false;
            if (node.position.Equals(point)) continue;
            for(int i = 0; i < currentNode.Count; i++)
            {
                if (node.ID == currentNode[i].ID)
                {
                    stop = true;
                    break;
                }
            }
            if (stop) continue;
            if (Vector3.Distance(node.position, point) < minDistance)
            {
                minDistance = Vector3.Distance(node.position, point);
                nearestNode = node;
            }
        }
        return nearestNode;
    }

    //Custom
    List<Path> AddRealtimePath(int a)
    {
        List<Node> nodes = null;
        while (nodes == null || nodes.Count == 0)
        {
            int b = a;
            while (b == a)
            {
                b = Random.Range(0, graphData.centers.Count);
            }
            Node spawn = graphData.centers[a];
            Node target = graphData.centers[b];

            nodes = FindShortedPathSynchronousInternal(a, target.ID);
        }
        List<Path> path = NodesToPath(nodes);
        return path;
    }
    List<Path> AddRealtimePath(int a, int b)
    {
        List<Node> nodes = null;
        /*int maxtarget = 4;
        if (nodes == null || nodes.Count == 0)
        {
            Node spawn = graphData.centers[a];
            Node start = spawn;
            for (int i = 0; i < maxtarget; i++)
            {
                Node target = FindNearestNode2(start.position, nodes);
                start = target;
                nodes.Add(target);
            }
        }*/
        /*StartCoroutine(FindShortestPathAsynchonousInternal(a, b, (callback) =>
        {
            nodes = callback;
        }));
        while (nodes == null || nodes.Count == 0)
        {
            continue;
        }*/
        while (nodes == null || nodes.Count == 0)
        {
        //Node spawn = graphData.centers[a];
        //Node target = graphData.centers[b];
        //StartCoroutine(FindShortestPathAsynchonousInternal(a, b, (callback) =>
        //{
        //    nodes = callback;
        //}
        //));
            nodes = FindShortedPathSynchronousInternal(a, b);
        //Debug.Log(nodes.Count);
        }
        Debug.Log(nodes.Count);
        List<Path> path = NodesToPath(nodes);
        return path;
    }
    List<Path> AddRealtimePath(Node spawn)
    {
        List<Node> nodes = new List<Node>();
        
        int maxtarget = 4;
        if (nodes == null || nodes.Count == 0)
        {
            nodes.Add(spawn);
            Node start = spawn;
            for (int i = 0; i < maxtarget; i++)
            {
                Node target = FindNearestNode2(start.position, nodes);
                start = target;
                nodes.Add(target);
            }
        }
        nodes.RemoveAt(0);
        Debug.Log(nodes.Count);
        List<Path> path = NodesToPath(nodes);
        return path;
    }
    void SpawnRealtime(int a)
    {
        PathFollower follower = SpawnCarRealtime();
        List<Path> paths = AddRealtimePath(a);
        follower.Follow(paths);
    }
    PathFollower SpawnCarRealtime()
    {
        if (!carsBox)
        {
            carsBox = (new GameObject("Cars")).transform;
        }
        int random = Random.Range(0, 8);
        //GameObject go = GameObject.Instantiate(carPrefab[random]);
        GameObject go = CarsPooling.sharedInstance.GetPooledObject(random);
        if (go != null)
        {
            go.SetActive(true);
           // go.GetComponent<PathFollower>().BeginCountToDisable();
        }
        if (random == 0)
        {
            go.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //go.transform.localScale = new Vector3(0.4f, 0.4f, 0.8f);
        go.transform.parent = carsBox;
        //cars.Add(go.transform);
        PathFollower follower;
        if (go.GetComponent<PathFollower>() != null)
        {
            follower = go.GetComponent<PathFollower>();
        }
        else follower = go.AddComponent<PathFollower>();
        return follower;
    }
    PathFollower SpawnCarRealtime(int idCar)
    {
        if (!carsBox)
        {
            carsBox = (new GameObject("Cars")).transform;
        }
        //GameObject go = GameObject.Instantiate(carPrefab[idCar]);
        GameObject go = CarsPooling.sharedInstance.GetPooledObject(idCar);
        if (go != null)
        {
            go.SetActive(true);
            //go.GetComponent<PathFollower>().BeginCountToDisable();
        }
        if (idCar == 0)
        {
            go.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        go.transform.parent = carsBox;
        //cars.Add(go.transform);
        PathFollower follower;
        if (go.GetComponent<PathFollower>() != null)
        {
            follower = go.GetComponent<PathFollower>();
        }
        else follower = go.AddComponent<PathFollower>();
        return follower;
    }
   
    public void spawCarInStreet(string street1, string street2, int idCar)
    {
        int a = getIDFromStreet(street1);
        int b = getIDFromStreet(street2);
        Debug.Log(a + "," + b);
        PathFollower follower = SpawnCarRealtime(idCar);
        List<Path> paths = AddRealtimePath(a, b);
        follower.Follow(paths);
    }
    public void spawCarInStreet(int numstreet1, int numstreet2, int idCar)
    {
        int a = graphData.allStreets[numstreet1].center.ID;
        int b = graphData.allStreets[numstreet2].center.ID;
        Debug.Log(a + "," + b);
        PathFollower follower = SpawnCarRealtime(idCar);
        List<Path> paths = AddRealtimePath(a, b);
        follower.Follow(paths);
    }
    public void spawCarInStreet(string street1, int idStreet2, int idCar)
    {
        int a = getIDFromStreet(street1);
        Node start = getNodeFromID(a);
        PathFollower follower = SpawnCarRealtime(idCar);
        //List<Path> paths = AddRealtimePath(a, idStreet2);
        //List<Path> paths = AddRealtimePath(start);
        List<Path> paths = AddRealtimePath(a);
        follower.Follow(paths);
    }
    void getInListPath()
    {
       
    }
    public void speedChangeValue()
    {
        TimeScale = (int)speedSlider.value;
    }

    private void Update()
    {
        /*
        if (Input.GetKey(KeyCode.A))
        {
            //foreach (Street b in graphData.allStreets)
            //{
            //    if (b.Name == "hvt1")
            //    {
            //        Debug.Log(b.center.ID);
            //    }
            //}
            foreach (Node a in graphData.centers)
            {
                Debug.Log(a.pathDistance + "   :    " + a.ID);

            }
            
            // SpawnRealtime(13);
        }
        if (Input.GetKey(KeyCode.B))
        {
            Debug.Log(graphData.centers[2624].ID);
            Debug.Log(graphData.centers[896].ID);
        }*/
    }
}
