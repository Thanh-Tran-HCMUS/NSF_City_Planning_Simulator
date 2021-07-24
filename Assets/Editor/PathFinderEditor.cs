using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

/// QPathFinder modified
/// <summary>
/// Class is responsible for displaying GUI
/// </summary>
[CustomEditor(typeof(PathFinder))]
public class PathFinderEditor : Editor {
    enum SceneMode {
        AddJunction,
        AddStreet,
        SelectJunction,
        SelectStreet,
        None,
        DeleteJunction,
        DeleteStreet
    }

    Junction previous;
    Junction selectedJunction;
    Street selectedStreet;
    int ifrom = 2, ito = 2;
    /// <summary>
    /// Selected scene mode 
    /// </summary>
    SceneMode sceneMode;
    PathFinder script;

    const string nodeGUITextColor = "#ff00ffff";
    const string pathGUITextColor = "#00ffffff";
    const string costGUITextColor = "#00a2ffff";
    const string junctionGUITextColor = "#00ff00ff";
    const string groundColliderLayerName = "Default";
    GUIStyle RichTextStyle => new GUIStyle {
        richText = true,
        alignment = TextAnchor.MiddleCenter,
        stretchHeight = true,
        stretchWidth = true
    };
    GUIStyle BoxStyle => new GUIStyle {
        overflow = new RectOffset(),
        clipping = TextClipping.Overflow,
        alignment = TextAnchor.MiddleCenter,
        stretchHeight = true,
        stretchWidth = true
    };

    /// QPathFinder
    /// <summary>
    /// Method running when user using scene window
    /// </summary>
    void OnSceneGUI() {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(controlID);
        DrawGUIWindowOnScene();
        UpdateMouseInput();
        DrawPathLine();
        if (GUI.changed) {
            SceneView.RepaintAll();
        }
    }

    /// QPathFinder modified
    /// <summary>
    /// Method is responsible for display window in scene window
    /// </summary>
    void DrawGUIWindowOnScene() {
        GUILayout.Window(1, new Rect(0f, 25f, 70f, 80f), delegate (int windowID) {
            EditorGUILayout.BeginHorizontal();
            sceneMode = (SceneMode)GUILayout.SelectionGrid((int)sceneMode, new string[] { "Add Junction", "Add Street", "Select Junction", "Select Street", "None", "Delete Junction", "Delete Street" }, 1);

            if (sceneMode != SceneMode.SelectStreet && selectedStreet) {
                selectedStreet.Select(false);
                selectedStreet = null;
            }
            if (sceneMode != SceneMode.SelectJunction && selectedJunction) {
                selectedJunction.Select(false);
                selectedJunction = null;
            }

            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }, "Mode");

        GUILayout.Window(2, new Rect(0, 195f, 70f, 100f), delegate (int windowID) {
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Clear All")) {
                ClearAll();
            }
            if (GUILayout.Button("Refresh Data")) {
                RefreshData();
            }
            script.TimeScale = (int)GUILayout.HorizontalSlider(script.TimeScale, 1, 5);
            GUI.Label(new Rect(10, 75, 80, 20), "Time Scale " + script.TimeScale);
            EditorGUILayout.EndVertical();
        }, "");

        if (sceneMode == SceneMode.AddStreet) {
            GUI.Window(4, new Rect(100f, 25f, 140f, 60f), delegate (int windowID) {
                EditorGUILayout.BeginHorizontal();
                GUI.color = Color.white;
                GUI.Label(new Rect(0, 20, 70, 15), "Lines IN");
                GUI.Label(new Rect(0, 40, 70, 15), "Lines OUT");
                GUI.Label(new Rect(90, 20, 20, 15), ifrom.ToString());
                GUI.Label(new Rect(90, 40, 20, 15), ito.ToString());
                if (GUI.Button(new Rect(105, 20, 15, 15), "<")) {
                    ifrom = Mathf.Clamp((ifrom - 1), 0, 10);
                }
                if (GUI.Button(new Rect(120, 20, 15, 15), ">")) {
                    ifrom = Mathf.Clamp((ifrom + 1), 0, 10);
                }
                if (GUI.Button(new Rect(105, 40, 15, 15), "<")) {
                    ito = Mathf.Clamp((ito - 1), 0, 10);
                }
                if (GUI.Button(new Rect(120, 40, 15, 15), ">")) {
                    ito = Mathf.Clamp((ito + 1), 0, 10);
                }
                EditorGUILayout.EndHorizontal();
            }, "Add Street");
        }
        if (sceneMode == SceneMode.SelectJunction && selectedJunction != null) {
            GUI.Window(5, new Rect(100f, 25f, 140f, 160f), delegate (int windowID) {
                GUI.color = Color.white;
                EditorGUILayout.BeginHorizontal();
                GUI.Label(new Rect(0, 20, 100, 25), "Roundabout");
                GUI.Label(new Rect(0, 40, 100, 25), "Calculate Timers");
                GUI.Label(new Rect(0, 60, 100, 25), "Cycle Time");
                selectedJunction.Rondo = GUI.Toggle(new Rect(110, 20, 20, 15), selectedJunction.Rondo, "");
                selectedJunction.timersCalc = GUI.Toggle(new Rect(110, 40, 20, 15), selectedJunction.timersCalc, "");
                string tmp = GUI.TextField(new Rect(110, 60, 25, 15), selectedJunction.cycleTime.ToString(), 3);
                tmp = tmp.Length == 0 ? "1" : tmp;
                try {
                    selectedJunction.cycleTime = float.Parse(tmp);
                }
                catch (FormatException) { }
                if (selectedJunction.Rondo == false) {
                    Color[] col = new Color[4] { Color.red, Color.green, Color.blue, Color.yellow };
                    GUI.Label(new Rect(0, 80, 120, 15), "Timers");
                    for (int i = 0; i < selectedJunction.timers.Length; i++) {
                        GUI.color = col[i];
                        tmp = GUI.TextField(new Rect(110, 80 + i * 20, 25, 15), selectedJunction.timers[i].ToString(), 3);
                        tmp = tmp.Length == 0 ? "0" : tmp;
                        try {
                            selectedJunction.timers[i] = float.Parse(tmp);
                        }
                        catch (FormatException) { }
                    }
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }, "Selected Junction");
        }
        if (sceneMode == SceneMode.SelectStreet && selectedStreet) {
            GUI.Window(3, new Rect(100f, 25f, 140f, 190f), delegate (int windowID) {
                GUI.color = Color.white;
                EditorGUILayout.BeginHorizontal();
                GUI.Label(new Rect(0, 20, 70, 15), "Name");
                GUI.Label(new Rect(0, 40, 70, 15), "Lines IN");
                GUI.Label(new Rect(0, 60, 70, 15), "Lines OUT");
                selectedStreet.Name = GUI.TextField(new Rect(45, 20, 90, 15), selectedStreet.Name, 25);
                GUI.Label(new Rect(90, 40, 20, 15), selectedStreet.iFrom.ToString());
                GUI.Label(new Rect(90, 60, 20, 15), selectedStreet.iTo.ToString());
                if (GUI.Button(new Rect(105, 40, 15, 15), "<")) {
                    selectedStreet.iFrom = Mathf.Clamp((selectedStreet.iFrom - 1), 0, 10);
                    selectedStreet.Calculate();
                    script.graphData.ReGenerateIDs();
                    selectedStreet.RecalcJunction();
                    script.graphData.ReGenerateIDs();
                }
                if (GUI.Button(new Rect(120, 40, 15, 15), ">")) {
                    selectedStreet.iFrom = Mathf.Clamp((selectedStreet.iFrom + 1), 0, 10);
                    selectedStreet.Calculate();
                    script.graphData.ReGenerateIDs();
                    selectedStreet.RecalcJunction();
                    script.graphData.ReGenerateIDs();
                }
                if (GUI.Button(new Rect(105, 60, 15, 15), "<")) {
                    selectedStreet.iTo = Mathf.Clamp((selectedStreet.iTo - 1), 0, 10);
                    selectedStreet.Calculate();
                    script.graphData.ReGenerateIDs();
                    selectedStreet.RecalcJunction();
                    script.graphData.ReGenerateIDs();
                }
                if (GUI.Button(new Rect(120, 60, 15, 15), ">")) {
                    selectedStreet.iTo = Mathf.Clamp((selectedStreet.iTo + 1), 0, 10);
                    selectedStreet.Calculate();
                    script.graphData.ReGenerateIDs();
                    selectedStreet.RecalcJunction();
                    script.graphData.ReGenerateIDs();
                }
                // delegate / inhabitants / commercial places / work places / emigrants 
                string[] describe = new string[5] { "Incoming", "Inhabitants", "Shopping places", "Work places", "Leaving" };
                Color[] col = new Color[5] { new Color(0.9f, 0.9f, 0.9f), new Color(0, 1, 0), new Color(0, 0.75f, 1), new Color(1, 1, 0), new Color(1, 0.58f, 0.62f) };
                for (int i = 0; i < 5; i++) {
                    GUI.color = col[i];
                    GUI.Label(new Rect(0, 80 + i * 20, 120, 15), describe[i]);
                    string tmp = GUI.TextField(new Rect(110, 80 + i * 20, 25, 15), selectedStreet.spawns[i].ToString(), 25);
                    tmp = tmp.Length == 0 ? "0" : tmp;
                    try {
                        selectedStreet.spawns[i] = int.Parse(tmp);
                    }
                    catch (FormatException) { }
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }, "Selected Street");
        }
    }

    /// QPathFinder modified
    /// <summary>
    /// The method is responsible for the visualization of paths
    /// </summary>
    void DrawPathLine() {
        if (script == null || script.graphData == null || script.graphData.nodes == null)
            return;

        foreach (Path pd in script.graphData.paths) {
            switch (pd.block) {
                case BlockType.Blocked:
                    Handles.color = Color.red;
                    break;
                case BlockType.Priority:
                    Handles.color = Color.green;
                    break;
                default:
                    Handles.color = Color.yellow;
                    break;
            }
            if (script.drawPaths && sceneMode != SceneMode.SelectJunction)
                Handles.DrawLine(pd.PosOfA, pd.PosOfB);

            Handles.BeginGUI();
            Vector3 currNode = (pd.PosOfA + pd.PosOfB) / 2;
            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(currNode);
            string str = "";
            if (script.showPathId)
                str += "<Color=" + pathGUITextColor + ">" + pd.autoGeneratedID.ToString() + "</Color>";
            if (script.showCosts && pd.hide == HidePath.Shown) {
                if (!string.IsNullOrEmpty(str))
                    str += "<Color=" + costGUITextColor + ">" + "  Cost: " + "</Color>";
                str += "<Color=" + costGUITextColor + ">" + pd.Cost.ToString("0.0") + "</Color>";
            }

            if (!string.IsNullOrEmpty(str))
                GUI.Label(new Rect(guiPosition.x - 10, guiPosition.y - 30, 40, 20), str, RichTextStyle);

            Handles.EndGUI();
        }
        Handles.BeginGUI();
        foreach (Street s in script.graphData.allStreets) {
            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(s.center.position);
            string str = "<Color=#ffffffff>" + s.Name + "</Color>";
            GUI.Label(new Rect(guiPosition.x, guiPosition.y - 20, 40, 20), str, RichTextStyle);
            if (script.showSpawns) {
                int i = 0;
                Color[] col = new Color[5] { new Color(0.9f, 0.9f, 0.9f), new Color(0, 1, 0), new Color(0, 0.75f, 1), new Color(1, 1, 0), new Color(1, 0.58f, 0.62f) };
                for (int j = 0; j < 5; j++) {
                    if (s.spawns[j] > 0) {
                        GUI.backgroundColor = col[j];
                        GUI.Box(new Rect(guiPosition.x + (i - 1) * 40, guiPosition.y, 40, 20), "");//przyjazd/dom/sklep/praca/wyjazd
                        GUI.Label(new Rect(guiPosition.x + (i - 1) * 40, guiPosition.y, 40, 20), s.spawns[j].ToString(), RichTextStyle);
                        i++;
                    }
                }
            }
        }
        GUI.backgroundColor = Color.white;
        Handles.EndGUI();

        if (script.showSpawns) {
            Handles.BeginGUI();
            foreach (Junction j in script.graphData.allJunctions) {
                if (j.phases != null && j.phases.Length > 0 && j.Rondo == false) {
                    Vector2 guiPosition = HandleUtility.WorldToGUIPoint(j.transform.position);
                    GUI.Label(new Rect(guiPosition.x - 20, guiPosition.y - 20, 40, 20), "<Color=" + junctionGUITextColor + ">" + j.timeToPhase.ToString("0.0") + "</Color>", RichTextStyle);
                }
                if (script.countPassings) {
                    foreach (Path p in j.paths) {
                        if (p.transform != null) {
                            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(p.transform.position);
                            GUI.Label(new Rect(guiPosition.x - 20, guiPosition.y - 20, 40, 20), "<Color=" + costGUITextColor + ">" + p.entireQueue.ToString() + "</Color>", RichTextStyle);
                        }
                    }
                }
            }
            Handles.EndGUI();
        }
        if (script.showNodeId) {
            Handles.BeginGUI();
            foreach (Node n in script.graphData.nodes) {
                Vector2 guiPosition = HandleUtility.WorldToGUIPoint(n.position);
                GUI.Label(new Rect(guiPosition.x - 10, guiPosition.y - 30, 20, 20), "<Color=" + nodeGUITextColor + ">" + n.ID.ToString() + "</Color>", RichTextStyle);
            }
            Handles.EndGUI();
        }

        if (sceneMode == SceneMode.SelectJunction && selectedJunction != null && selectedJunction.Rondo == false) {
            Color[] colors = new Color[4] { Color.red, Color.green, Color.blue, Color.yellow };
            if (selectedJunction.phases != null) {
                for (int i = 0; i < selectedJunction.phases.Length; i++) {
                    Handles.color = colors[i];
                    Vector3 v = new Vector3(0, i * 0.1f, 0);
                    foreach (int j in selectedJunction.phases[i].routes) {
                        Handles.DrawLine(selectedJunction.paths[j].PosOfA + v, selectedJunction.paths[j].PosOfB + v);
                    }
                }
            }
        }
        Handles.color = Color.white;
    }

    /// QPathFindermodified
    /// <summary>
    /// The method is responsible for capturing mouse events
    /// </summary>
    void UpdateMouseInput() {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0) {
            OnMouseClick(e.mousePosition);
        }
    }

    /// QPathFinder modified
    /// <summary>
    /// The method is responsible for the reaction to the click of the mouse
    /// </summary>
    /// <param name="mousePos">Position of cursor</param>
    void OnMouseClick(Vector2 mousePos) {
        LayerMask backgroundLayerMask = 1 << LayerMask.NameToLayer(groundColliderLayerName);
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100000f, backgroundLayerMask)) {
            if (sceneMode == SceneMode.AddJunction) {
                Vector3 hitPos = hit.point;
                var junction = CreateJunction(hitPos);
                if (hit.transform.parent && hit.transform.parent.name == "Street") {
                    var street = hit.transform.parent.GetComponent<Street>();
                    var fr = street.from;
                    var to = street.to;
                    ifrom = street.iFrom;
                    ito = street.iTo;
                    DeleteStreet(street);
                    CreateStreet(fr, junction);
                    CreateStreet(junction, to);
                }
            }
            if (sceneMode == SceneMode.AddStreet) {
                if (previous == null) {
                    previous = hit.transform.gameObject.GetComponent<Junction>();
                } else {
                    Junction b = hit.transform.gameObject.GetComponent<Junction>();
                    if (b != null && previous != b) {
                        CreateStreet(previous, b);
                        previous = null;
                    }
                }
            }
            if (sceneMode == SceneMode.SelectJunction) {
                if (hit.transform.name == "Junction") {
                    if (selectedJunction) {
                        selectedJunction.Select(false);
                        selectedJunction = null;
                    }
                    selectedJunction = hit.transform.GetComponent<Junction>();
                    selectedJunction.Select();
                }
            } else {
                if (selectedJunction) {
                    selectedJunction.Select(false);
                    selectedJunction = null;
                }
            }
            if (sceneMode == SceneMode.SelectStreet) {
                if (hit.transform.parent != null && hit.transform.parent.name == "Street") {
                    if (selectedStreet) {
                        selectedStreet.Select(false);
                        selectedStreet = null;
                    }
                    selectedStreet = hit.transform.parent.GetComponent<Street>();
                    selectedStreet.Select();
                }
            } else {
                if (selectedStreet) {
                    selectedStreet.Select(false);
                    selectedStreet = null;
                }
            }
            if (sceneMode == SceneMode.DeleteJunction) {
                if (hit.transform.name == "Junction") {
                    DeleteJunction(hit.transform.GetComponent<Junction>());
                }
            }
            if (sceneMode == SceneMode.DeleteStreet) {
                if (hit.transform.parent.name == "Street") {
                    DeleteStreet(hit.transform.parent.GetComponent<Street>());
                }
            }
        }
    }

    /// <summary>
    /// Method is responsible for creating junction
    /// </summary>
    /// <param name="position">Position where junction should be created</param>
    Junction CreateJunction(Vector3 position) {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.GetComponent<CapsuleCollider>().radius = 2f;
        go.name = "Junction";
        go.transform.localScale = new Vector3(1, 0.1f, 1);
        go.transform.position = position;
        go.GetComponent<Renderer>().material = (Material)AssetDatabase.LoadAssetAtPath("Assets/QPathSimulation/Materials/junction.mat", typeof(Material));
        go.transform.parent = script.transform;

        Junction junction = go.AddComponent<Junction>();
        script.graphData.allJunctions.Add(junction);
        return junction;
    }
    /// <summary>
    /// Method responsible for creating new street
    /// </summary>
    /// <param name="a">First junction</param>
    /// <param name="b">Second junction</param>
    void CreateStreet(Junction a, Junction b) {
        GameObject go = new GameObject("Street");
        go.transform.parent = script.transform;
        Street street = go.AddComponent<Street>();
        street.Init(a, b, ifrom, ito);
        script.graphData.allStreets.Add(street);
        RefreshData();
    }
    /// <summary>
    /// Method responsible for deleting junction
    /// </summary>
    /// <param name="junction">Selected junction</param>
    void DeleteJunction(Junction junction) {
        foreach (Joint jo in junction.joints) {
            script.graphData.allStreets.Remove(jo.street);
        }
        script.graphData.allJunctions.Remove(junction);
        junction.Destroy();
        RefreshData();
    }
    /// <summary>
    /// Method responsible for deleting street
    /// </summary>
    /// <param name="street">Selected Street</param>
    void DeleteStreet(Street street) {
        script.graphData.allStreets.Remove(street);
        street.Destroy();
        RefreshData();
    }
    /// <summary>
    /// Method responsible for recalculate streets, junctions and refresh IDs of paths and nodes
    /// </summary>
    void RefreshData() {
        foreach (Street s in script.graphData.allStreets) {
            s.Calculate();
        }
        script.graphData.ReGenerateIDs();
        foreach (Junction j in script.graphData.allJunctions) {
            j.Calculate();
        }
        script.graphData.ReGenerateIDs();
    }
    /// <summary>
    /// Deletes all graph data
    /// </summary>
    void ClearAll() {
        script.graphData.Clear();
    }

    /// QPathFinder modified
    /// <summary>
    /// Method running when object is Enable
    /// </summary>
    void OnEnable() {
        sceneMode = SceneMode.None;
        script = FindObjectOfType<PathFinder>();
        if (script.save) {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
    }
    /// <summary>
    /// Method running when object is Disable
    /// </summary>
    void OnDisable() {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }
    /// <summary>
    /// Method runs when simulation starts or ends
    /// </summary>
    /// <param name="state">State of simulation</param>
    void OnPlayModeStateChanged(PlayModeStateChange state) {
        if (state == PlayModeStateChange.EnteredEditMode) {
            script.graphData.LoadTimers();
        }
        if (state == PlayModeStateChange.ExitingPlayMode) {
            script.graphData.SaveTimers();
        }
    }

    // QPathFinder
    [MenuItem("GameObject/Create a 2D PathFinder in scene with a collider")]
    public static void Create2DPathFinderObjectInScene() {
        if (GameObject.FindObjectOfType<PathFinder>() == null) {
            var colliderGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderGo.name = "Ground";
            colliderGo.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.black);

            colliderGo.transform.localScale = new Vector3(100f, 100f, 1f); ;
            var boxCollider = colliderGo.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }
    }

    // QPathFinder
    [MenuItem("GameObject/Create a 3D PathFinder in scene with a collider")]
    public static void Create3DPathFinderObjectInScene() {
        if (GameObject.FindObjectOfType<PathFinder>() == null) {
            var colliderGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderGo.name = "Ground";
            colliderGo.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.green);

            colliderGo.transform.localScale = new Vector3(100f, 1f, 100f); ;
            colliderGo.transform.position = Vector3.down * 20;

            var boxCollider = colliderGo.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }
    }

}