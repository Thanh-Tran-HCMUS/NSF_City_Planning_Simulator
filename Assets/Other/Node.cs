using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// QPathFinder
/// <summary>
/// Single Node. From which will be created Paths
/// </summary>
[System.Serializable]
public class Node {
    /// <summary>
    /// Construktor
    /// </summary>
    /// <param name="Position">Position of node</param>
    public Node(Vector3 Position) { position = Position; }
    /// <summary>
    /// Position of node
    /// </summary>
    [SerializeField] public Vector3 position;
    /// <summary>
    /// ID of node
    /// </summary>
    [SerializeField] public int ID = -1;
    /// QPathFinder
    /// <summary>
    /// Distance calculated by heuristic
    /// </summary>
    [HideInInspector] public float heuristicDistance;
    /// QPathFinder
    /// <summary>
    /// Distance from first node
    /// </summary>
    [HideInInspector] public float pathDistance;
    /// QPathFinder
    /// <summary>
    /// Distance from previous node
    /// </summary>
    [HideInInspector] public Node previousNode;
    /// QPathFinder
    /// <summary>
    /// Return sum of distance
    /// </summary>
    [HideInInspector]
    public float CombinedHeuristic {
        get { return pathDistance + heuristicDistance; }
    }
}
