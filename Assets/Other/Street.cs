using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part of the street connected to the intersection
/// </summary>
[System.Serializable]
public class Joint {
    /// <summary>
    /// Stores references to the street of which it is part
    /// </summary>
    public Street street;
    /// <summary>
    /// Stores a list of nodes of paths entering an intersection
    /// </summary>
    public List<Node> input = new List<Node>();
    /// <summary>
    /// Stores a list of vertices of paths exiting from an junction
    /// </summary>
    public List<Node> output = new List<Node>();
    /// <summary>
    /// Stores the normalized vector from the center to the path junction
    /// </summary>
    public Vector3 outsideVec;
    /// <summary>
    /// Returns the approximate position of the joint
    /// </summary>
    public Vector3 Position {
        get {
            if (input.Count != 0 && output.Count != 0) {
                return (input[0].position + output[0].position) / 2;
            }
            if (input.Count != 0) {
                return input[0].position;
            }
            if (output.Count != 0) {
                return output[0].position;
            } else {
                return Vector3.zero;
            }
        }
    }
    /// <summary>
    /// Construktor
    /// </summary>
    /// <param name="Street">Street to which it belongs</param>
    /// <param name="Type">Connection type. True if junction "to", False if junction "from"</param>
    public Joint(Street Street, bool Type) {
        street = Street;
        outsideVec = (Street.to.transform.position - Street.from.transform.position).normalized;
        outsideVec = Type == false ? -outsideVec : outsideVec;
    }
}

/// <summary>
/// Street
/// </summary>
[System.Serializable]
public class Street : MonoBehaviour {
    /// <summary>
    /// Store references to the "from" intersection
    /// </summary>
    [HideInInspector] public Junction from;
    /// <summary>
    /// Store references to the "to" intersection
    /// </summary>
    [HideInInspector] public Junction to;
    /// <summary>
    /// Stores position
    /// </summary>
    Vector3 fromBorder;
    Vector3 toBorder;
    /// <summary>
    /// Stores the street name
    /// </summary>
    public string Name = "";
    /// <summary>
    /// Store list of paths
    /// </summary>
    public List<Path> paths = new List<Path>();
    /// <summary>
    /// Store list of nodes
    /// </summary>
    public List<Node> nodes = new List<Node>();
    /// <summary>
    /// Holds the middle node
    /// </summary>
    public Node center;
    /// <summary>
    /// Stores the number of paths "from-to"
    /// </summary>
    [HideInInspector] public int iFrom = 2;
    /// <summary>
    /// Stores the number of paths "to-from"
    /// </summary>
    [HideInInspector] public int iTo = 2;
    /// <summary>
    /// It stores how many vehicles of a given type enter or leave this street
    /// It is an array that stores 5 types: visitors, residents, commercial places, workplaces, and people leaving
    /// </summary>
    [HideInInspector] public int[] spawns = new int[5] { 0, 0, 0, 0, 0 };//arrival / home / shop / work / departure

    /// <summary>
    /// The method is responsible for initiating the street
    /// </summary>
    /// <param name="From">Junction "from"</param>
    /// <param name="To">Junction "to"</param>
    /// <param name="fromCount">Number of paths "from-to"</param>
    /// <param name="toCount">Number of paths "to-from"</param>
    public void Init(Junction From, Junction To, int fromCount = 1, int toCount = 1) {
        iFrom = fromCount; iTo = toCount;
        this.from = From; this.to = To;
        From.AddJoint(new Joint(this, false));
        To.AddJoint(new Joint(this, true));
        Vector3 centerPos = (To.transform.position + From.transform.position) / 2;
        this.center = new Node(centerPos);
        nodes.Add(this.center);
        Calculate();
    }
    /// <summary>
    /// The method is responsible for destroying the object
    /// </summary>
    /// <param name="spare">Junction that keep references to the street</param>
    public void Destroy(Junction spare = null) {
        if (from != spare) {
            from.RemoveJoint(this);
        }
        if (to != spare) {
            to.RemoveJoint(this);
        }
        Clear();
        DestroyImmediate(gameObject);
    }
    /// <summary>
    /// The method is responsible for removing owned paths and node
    /// </summary>
    void Clear() {
        foreach (Path p in paths) {
            if (p.transform != null) {
                DestroyImmediate(p.transform.gameObject);
            }
        }
        paths.Clear();
        nodes.Clear();
        nodes.Add(center);
    }
    /// <summary>
    /// The method is responsible for changing the length of the paths
    /// </summary>
    public void Resize() {
        Vector3 norm = (to.transform.position - from.transform.position).normalized;
        Vector2 p2 = Vector2.Perpendicular(new Vector2(norm.x, norm.z)).normalized;
        Vector3 perpendic = new Vector3(p2.x, 0, p2.y).normalized;
        Vector3 jPerpendic = Vector3.Project(Perpendic(from), norm) + perpendic;
        Joint jointFrom = from.joints.Find(item => item.street == this);
        Joint jointTo = to.joints.Find(item => item.street == this);
        for (int i = 0; i < jointFrom.input.Count; i++) {
            jointFrom.input[i].position = (from.transform.position + norm * from.margin + jPerpendic * (0.4f + i * 0.6f));
        }
        for (int i = 0; i < jointFrom.output.Count; i++) {
            jointFrom.output[i].position = (from.transform.position + norm * from.margin - jPerpendic * (0.4f + i * 0.6f));
        }
        Vector3 jPerpendic2 = Vector3.Project(Perpendic(to), -norm) + perpendic;
        for (int i = 0; i < jointTo.input.Count; i++) {
            jointTo.input[i].position = (to.transform.position - norm * to.margin - jPerpendic2 * (0.4f + i * 0.6f));
        }
        for (int i = 0; i < jointTo.output.Count; i++) {
            jointTo.output[i].position = (to.transform.position - norm * to.margin + jPerpendic2 * (0.4f + i * 0.6f));
        }
        foreach (Path p in paths) {
            p.Visualize();
        }
    }
    /// <summary>
    /// The method calculates the vector by which the paths leading to the junction will end
    /// </summary>
    /// <param name="j">The junction the vector will apply to</param>
    /// <returns>Scaled vector perpendicular to other junction streets</returns>
    Vector3 Perpendic(Junction j) {
        Vector3 perpendic = new Vector3(-(toBorder.z - fromBorder.z), 0, toBorder.x - fromBorder.x).normalized;
        int index = j.joints.FindIndex(item => item.street == this);
        if (index == -1) {
            return perpendic;
        }
        int prev = index > 0 ? index - 1 : j.joints.Count - 1;
        prev = prev < 0 ? 0 : prev;
        int next = index < j.joints.Count - 1 ? index + 1 : 0;
        next = next == prev ? index : next;
        if (j.joints.Count > 1) {
            Vector3 a = j.joints[prev].outsideVec;
            Vector3 b = j.joints[next].outsideVec;
            if (j == to) {
                a = -a;
                b = -b;
            }
            Vector3 v = (a - b).normalized;
            float scale = 1f;
            if (j.joints.Count == 2) {
                scale = (1f - Vector3.Angle(a, b) / 180f) * 3f;
            }
            perpendic = scale * v;
        }
        if (index == next && index != 0) {
            perpendic = -perpendic;
        }
        return perpendic;
    }

    /// <summary>
    /// The method is responsible for calculating the connected junction
    /// </summary>
    public void RecalcJunction() {
        from.Calculate();
        to.Calculate();
    }
    /// <summary>
    /// The method is responsible for creating a predetermined number of paths
    /// </summary>
    public void Calculate() {
        Vector3 norm = (to.transform.position - from.transform.position).normalized;
        fromBorder = from.transform.position + norm * from.margin;
        toBorder = to.transform.position - norm * to.margin;

        Vector3 perpedic = new Vector3(-(toBorder.z - fromBorder.z), 0, toBorder.x - fromBorder.x).normalized;

        List<Node> nod1 = new List<Node>();
        List<Node> nod2 = new List<Node>();
        List<Node> nod3 = new List<Node>();
        List<Node> nod4 = new List<Node>();
        Clear();

        center.position = ((toBorder + fromBorder) / 2 + norm * (from.margin - to.margin));

        Node prev = center;
        for (int i = 0; i < iFrom; i++) {
            Node n1 = new Node(fromBorder - perpedic * (0.4f + i * 0.6f));
            Node n3 = new Node(toBorder - perpedic * (0.4f + i * 0.6f));
            Node n2 = new Node(center.position - perpedic * (0.4f + i * 0.6f));
            nod1.Add(n1);
            nod2.Add(n3);
            nodes.Add(n1);
            nodes.Add(n2);
            nodes.Add(n3);
            Path[] p = new Path[4];
            p[0] = new Path(n1, n2, transform, HidePath.Shown, BlockType.Open);
            p[1] = new Path(n2, n3, transform, HidePath.Shown, BlockType.Open);
            p[2] = new Path(prev, n2, transform, HidePath.Hiden, BlockType.Open);
            p[3] = new Path(n2, prev, transform, HidePath.Hiden, BlockType.Open);
            paths.AddRange(p);
            prev = n2;
        }
        prev = center;
        for (int i = 0; i < iTo; i++) {
            Node n1 = new Node(toBorder + perpedic * (0.4f + i * 0.6f));
            Node n3 = new Node(fromBorder + perpedic * (0.4f + i * 0.6f));
            Node n2 = new Node(center.position + perpedic * (0.4f + i * 0.6f));
            nod3.Add(n1);
            nod4.Add(n3);
            nodes.Add(n1);
            nodes.Add(n2);
            nodes.Add(n3);
            Path[] p = new Path[4];
            p[0] = new Path(n1, n2, transform, HidePath.Shown, BlockType.Open);
            p[1] = new Path(n2, n3, transform, HidePath.Shown, BlockType.Open);
            p[2] = new Path(prev, n2, transform, HidePath.Hiden, BlockType.Open);
            p[3] = new Path(n2, prev, transform, HidePath.Hiden, BlockType.Open);
            paths.AddRange(p);
            prev = n2;
        }
        Joint jointFrom = from.joints.Find(item => item.street == this);
        Joint jointTo = to.joints.Find(item => item.street == this);
        jointFrom.input = nod4;
        jointFrom.output = nod1;
        jointTo.input = nod2;
        jointTo.output = nod3;
    }
    /// <summary>
    /// The method is responsible for highlighting the street as marked
    /// </summary>
    /// <param name="light">True if highlight</param>
    public void Select(bool light = true) {
        List<MeshRenderer> mats = new List<MeshRenderer>();
        Material mat = new Material(source: paths.Find(p => p.transform != null).transform.GetComponent<MeshRenderer>().sharedMaterial);
        foreach (Path p in paths) {
            if (p.transform) {
                mats.Add(p.transform.GetComponent<MeshRenderer>());
            }
        }
        if (light) {
            mat.color = new Color(1f, 1f, 1f);
        } else {
            mat.color = new Color(0.5f, 0.5f, 0.5f);
        }
        foreach (MeshRenderer m in mats) {
            m.material = mat;
        }
    }
}
