using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System;

/// QPathFinder modified
/// <summary>
/// A collection of nodes, paths and objects link to them
/// </summary>
[System.Serializable]
public class GraphData {
    /// <summary>
    /// List of all streets
    /// </summary>
    [HideInInspector] public List<Street> allStreets = new List<Street>();
    /// <summary>
    /// List of all junctions
    /// </summary>
    [HideInInspector] public List<Junction> allJunctions = new List<Junction>();

    /// <summary>
    /// List of all nodes
    /// </summary>
    [HideInInspector] public List<Node> nodes = new List<Node>();
    /// <summary>
    /// List of center nodes
    /// </summary>
    [HideInInspector] public List<Node> centers = new List<Node>();
    /// <summary>
    /// List of all paths
    /// </summary>
    [HideInInspector] public List<Path> paths = new List<Path>();
    /// <summary>
    /// Dictionary of path ID by pair of Nodes ID
    /// </summary>
    [HideInInspector] public Dictionary<Vector2Int, int> pathsByNodes = new Dictionary<Vector2Int, int>();

    /// <summary>
    /// Structure used to save data between play and edit mode
    /// </summary>
    [Serializable] struct SavingStructure {
        public float[][] Timers;
        public int[][] eqCounter;
    }


    /// <summary>
    /// Method gather data to structure and save it to file
    /// </summary>
    public void SaveTimers() {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) {
            File.Delete(destination);
        }
        BinaryFormatter bf = new BinaryFormatter();
        float[][] Timers = new float[allJunctions.Count][];
        int[][] eqCounter = new int[allJunctions.Count][];
        for (int i = 0; i < allJunctions.Count; i++) {
            Timers[i] = allJunctions[i].timers;
            eqCounter[i] = new int[allJunctions[i].paths.Count];
            for (int j = 0; j < allJunctions[i].paths.Count; j++) {
                eqCounter[i][j] = allJunctions[i].paths[j].entireQueue;
            }
        }
        SavingStructure sav = new SavingStructure {
            Timers = Timers,
            eqCounter = eqCounter
        };
        file = File.Create(destination);
        bf.Serialize(file, sav);
        file.Close();
        Debug.Log("File saved");
    }
    /// <summary>
    /// Method load file and fill certain objects
    /// </summary>
    public void LoadTimers() {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) {
            file = File.OpenRead(destination);
        } else {
            Debug.Log("File not found");
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        SavingStructure sav = (SavingStructure)bf.Deserialize(file);
        float[][] Timers = sav.Timers;
        int[][] eqCounter = sav.eqCounter;

        if (allJunctions.Count != Timers.Length) {
            return;
        }
        for (int i = 0; i < allJunctions.Count; i++) {
            allJunctions[i].timers = Timers[i];
            if (allJunctions[i].paths.Count == eqCounter[i].Length) {
                for (int j = 0; j < allJunctions[i].paths.Count; j++) {
                    allJunctions[i].paths[j].entireQueue = eqCounter[i][j];
                }
            }
        }
        file.Close();
        Debug.Log("File loaded");
    }

    public Path GetPathBetween(int from, int to) {
        return paths[pathsByNodes[new Vector2Int(from, to)]];
    }

    /// <summary>
    /// Method returns path that connect node "from "to"
    /// </summary>
    /// <param name="from">node "from"</param>
    /// <param name="to">node "to"</param>
    /// <returns>Path that connect nodes</returns>
    public Path GetPathBetween(Node from, Node to) {
        return GetPathBetween(from.ID, to.ID);
    }

    /// <summary>
    /// Metod delete all junctions and streets
    /// </summary>
    public void Clear() {
        foreach (Junction s in allJunctions) {
            s.Destroy();
        }
        allJunctions.Clear();
        allStreets.Clear();
        paths.Clear();
        nodes.Clear();
    }

    /// QPathFinder modified
    /// <summary>
    /// The method is responsible for assigning identifiers to nodes and paths
    /// and creating a dictionary mapping a pair of node IDs, ID of path 
    /// </summary>
    public void ReGenerateIDs() {
        nodes.Clear();
        centers.Clear();
        paths.Clear();
        foreach (Street s in allStreets) {
            nodes.AddRange(s.nodes);
            centers.Add(s.center);
            paths.AddRange(s.paths);
        }
        foreach (Junction j in allJunctions) { paths.AddRange(j.paths); }

        for (int i = 0; i < nodes.Count; i++) { nodes[i].ID = i; }
        for (int i = 0; i < paths.Count; i++) {
            if (paths[i].IDOfA == -1 || paths[i].IDOfB == -1) {
                paths.RemoveAt(i);
                i--;
            } else {
                paths[i].autoGeneratedID = i;
            }
        }

        pathsByNodes = new Dictionary<Vector2Int, int>();
        for (int i = 0; i < paths.Count; i++) {
            Vector2Int v = new Vector2Int(paths[i].IDOfA, paths[i].IDOfB);
            if (pathsByNodes.ContainsKey(v)) {
                Debug.Log("Warrning duble key " + v);
                if (paths[pathsByNodes[v]] == paths[i]) {
                    Debug.Log("Error duble path");
                }
            } else {
                pathsByNodes.Add(v, i);
            }
        }
    }

}
