/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InfinityCode.RealWorldTerrain.Net;
using InfinityCode.RealWorldTerrain.OSM;
using InfinityCode.RealWorldTerrain.Phases;
using InfinityCode.RealWorldTerrain.Windows;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

#if EASYROADS3D
using EasyRoads3Dv3;
#endif

#if ROADARCHITECT
using GSD.Roads;
#endif

namespace InfinityCode.RealWorldTerrain.Generators
{
    public class RealWorldTerrainRoadGenerator
    {
        private static List<RealWorldTerrainRoadGenerator> roads;
        private static GameObject roadContainer;

        public bool isDuplicate = false;
        public string type;

        private RealWorldTerrainContainer container;
        private string id;
        private List<Vector3> points;
        private RealWorldTerrainOSMWay way;
        private List<Vector3> globalPoints;
        private GameObject roadGo;
        private static List<string> alreadyCreated;
        private static bool loaded = false;
        private static Dictionary<string, RealWorldTerrainOSMNode> nodes;
        private static Dictionary<string, RealWorldTerrainOSMWay> ways;
        private static List<RealWorldTerrainOSMRelation> relations;

#if EASYROADS3D
        private static ERRoadNetwork roadNetwork;
        private static ERRoadType roadType;
        private static ERRoadType[] erRoadTypes;
        private static string[] roadTypeNames;
#endif

#if ROADARCHITECT
        private static GSDRoadSystem tRoadSystem;
        private List<GSDSplineN> splines;
#endif

        public static string url
        {
            get
            {
                string highwayType = "'highway'";
                if (prefs.roadTypeMode == RealWorldTerrainRoadTypeMode.simple)
                {
                    if ((int)prefs.roadTypes != -1)
                    {
                        BitArray ba = new BitArray(System.BitConverter.GetBytes((int)prefs.roadTypes));
                        List<string> types = new List<string>();
                        for (int i = 0; i < 32; i++)
                        {
                            if (ba.Get(i))
                            {
                                string s = Enum.GetName(typeof(RealWorldTerrainRoadType), (RealWorldTerrainRoadType)(1 << i)).ToLowerInvariant();
                                types.Add(s);
                            }
                        }
                        highwayType += "~'" + string.Join(@"|", types.ToArray()) + "'";
                    }
                }
                
                string data = string.Format(RealWorldTerrainCultureInfo.numberFormat, "node({0},{1},{2},{3});way(bn)[{4}];(._;>;);out;", prefs.coordinatesTo.y, prefs.coordinatesFrom.x, prefs.coordinatesFrom.y, prefs.coordinatesTo.x, highwayType);
                return RealWorldTerrainOSMUtils.osmURL + RealWorldTerrainDownloadManager.EscapeURL(data);
            }
        }

        public static string filename
        {
            get
            {
                return System.IO.Path.Combine(RealWorldTerrainEditorUtils.osmCacheFolder, string.Format("roads_{0}_{1}_{2}_{3}_{4}.osm", prefs.coordinatesTo.y, prefs.coordinatesFrom.x,
                    prefs.coordinatesFrom.y, prefs.coordinatesTo.x, (int)prefs.roadTypes));
            }
        }

        public static string compressedFilename
        {
            get
            {
                return filename + "c";
            }
        }

        private static RealWorldTerrainPrefs prefs
        {
            get { return RealWorldTerrainWindow.prefs; }
        }

        private Vector3 secondPoint
        {
            get { return points[1]; }
        }

        private Vector3 preLastPoint
        {
            get { return points[points.Count - 2]; }
        }

        private Vector3 lastPoint
        {
            get { return points.Last(); }
        }

        private Vector3 firstPoint
        {
            get { return points.First(); }
        }

        public RealWorldTerrainRoadGenerator(RealWorldTerrainOSMWay way, RealWorldTerrainContainer container)
        {
            if (roads == null) roads = new List<RealWorldTerrainRoadGenerator>();

            this.container = container;
            this.way = way;
            id = way.id;
            type = this.way.GetTagValue("highway");

            globalPoints = RealWorldTerrainOSMUtils.GetGlobalPointsFromWay(this.way, nodes);

            DetectDuplicates();
            if (isDuplicate) return;

            points = new List<Vector3>(globalPoints.Count);
            foreach (Vector3 gp in globalPoints) points.Add(RealWorldTerrainEditorUtils.CoordsToWorldWithElevation(gp, container));

            NormalizeDistance();
            TrimPoints();

            roads.Add(this);
        }

        public static void CombineRoads()
        {
            if (roads.Count < 2) return;
            int index = 1;
            while (index < roads.Count)
            {
                RealWorldTerrainRoadGenerator road2 = roads[index];
                if (road2.points.Count < 2)
                {
                    index++;
                    continue;
                }

                bool merged = false;
                for (int i = 0; i < index; i++)
                {
                    RealWorldTerrainRoadGenerator road1 = roads[i];

                    if (road1.type != road2.type) continue;
                    if (road1.points.Count < 2) continue;

                    const float offset = 0.1f;
                    if ((road2.firstPoint - road1.lastPoint).magnitude < offset && RealWorldTerrainUtils.Angle2D(road1.preLastPoint, road1.lastPoint, road2.secondPoint) < 20)
                    {
                        road1.points.AddRange(road2.points.GetRange(1, road2.points.Count - 1));
                        roads.Remove(road2);
                        merged = true;
                        break;
                    }
                    if ((road2.lastPoint - road1.firstPoint).magnitude < offset && RealWorldTerrainUtils.Angle2D(road1.secondPoint, road1.firstPoint, road2.preLastPoint) < 20)
                    {
                        road1.points.InsertRange(0, road2.points.GetRange(0, road2.points.Count - 1));
                        roads.Remove(road2);
                        merged = true;
                        break;
                    }
                    if ((road2.lastPoint - road1.lastPoint).magnitude < offset && RealWorldTerrainUtils.Angle2D(road1.preLastPoint, road1.lastPoint, road2.preLastPoint) < 20)
                    {
                        List<Vector3> r2points = road2.points.GetRange(0, road2.points.Count - 1);
                        r2points.Reverse();
                        road1.points.AddRange(r2points);
                        roads.Remove(road2);
                        merged = true;
                        break;
                    }
                    if ((road2.firstPoint - road1.firstPoint).magnitude < offset && RealWorldTerrainUtils.Angle2D(road1.secondPoint, road1.firstPoint, road2.secondPoint) < 20)
                    {
                        List<Vector3> r2points = road2.points.GetRange(1, road2.points.Count - 1);
                        r2points.Reverse();
                        road1.points.InsertRange(0, r2points);
                        roads.Remove(road2);
                        merged = true;
                        break;
                    }
                }
                if (!merged) index++;
            }
        }

        private void CreateEasyRoads()
        {
#if EASYROADS3D
            ERRoadType activeRoadType = roadType;
            if (prefs.roadTypeMode == RealWorldTerrainRoadTypeMode.advanced)
            {
                string highway = way.GetTagValue("highway");
                int index = -1;
                for (int i = 0; i < roadTypeNames.Length; i++)
                {
                    if (highway == roadTypeNames[i])
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1) return;
                if (string.IsNullOrEmpty(prefs.erRoadTypes[index])) return;
                string roadTypeName = prefs.erRoadTypes[index];
                activeRoadType = erRoadTypes.FirstOrDefault(t => t.roadTypeName == roadTypeName);
                if (activeRoadType == null) return;
            }

            ERRoad road = roadNetwork.CreateRoad("road " + way.id, activeRoadType, points.ToArray());
            road.SnapToTerrain(prefs.erSnapToTerrain);
            road.gameObject.isStatic = false;
            road.gameObject.AddComponent<RealWorldTerrainOSMMeta>().GetFromOSM(way);
#endif
        }

        private void CreateRoadArchitectRoad()
        {
#if ROADARCHITECT
            roadGo = tRoadSystem.AddRoad();
            GSDRoad road = roadGo.GetComponent<GSDRoad>();
            road.opt_HeightModEnabled = false;
            road.opt_bShouldersEnabled = type == "primary";
            road.opt_DetailModEnabled = false;
            road.opt_bMaxGradeEnabled = false;
            road.opt_TreeModEnabled = false;

            if (type == "residential")
            {
                road.opt_LaneWidth = 2;
            }

            if (way.HasTagKey("surface"))
            {
                string surface = way.GetTagValue("surface");
                if (surface == "unpaved")
                {
                    road.opt_tRoadMaterialDropdown = GSDRoad.RoadMaterialDropdownEnum.Dirt;
                    road.opt_LaneWidth = 2.5f;
                }
            }

            if (way.HasTagKey("tracktype"))
            {
                road.opt_tRoadMaterialDropdown = GSDRoad.RoadMaterialDropdownEnum.Dirt;
                road.opt_LaneWidth = 2.5f;
            }

            road.transform.position = firstPoint;
            road.gameObject.AddComponent<RealWorldTerrainOSMMeta>().GetFromOSM(way);

            Vector3 offset = new Vector3(0, 0.5f, 0);
            splines = new List<GSDSplineN>();

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 point = points[i];
                GameObject tNodeObj = new GameObject("Node" + i);
                GSDSplineN tNode = tNodeObj.AddComponent<GSDSplineN>();
                tNodeObj.AddComponent<RealWorldTerrainRoadArchitectNode>();
                tNodeObj.transform.position = point + offset;
                tNodeObj.transform.parent = road.GSDSplineObj.transform;
                tNode.idOnSpline = i;
                tNode.GSDSpline = road.GSDSpline;
                tNode.bNeverIntersect = true;
                splines.Add(tNode);
            }

            road.UpdateRoad();
#endif
        }

        private void DetectDuplicates()
        {
            for (int i = 0; i < roads.Count; i++)
            {
                RealWorldTerrainRoadGenerator r = roads[i];
                if (r.globalPoints.Count != globalPoints.Count) continue;

                bool findDiff = globalPoints.Where((t, j) => (r.globalPoints[j] - t).magnitude > 0.0001f).Any();
                if (!findDiff)
                {
                    isDuplicate = true;
                    return;
                }
            }
        }

        public static void Dispose()
        {
            loaded = false;
            ways = null;
            relations = null;
            nodes = null;
            roadContainer = null;
            alreadyCreated = null;
        }

        public static void Download()
        {
            if (!prefs.generateRoads || prefs.roadTypes == 0 || File.Exists(compressedFilename)) return;
            if (File.Exists(filename))
            {
                byte[] data = File.ReadAllBytes(filename);
                OnDownloadComplete(ref data);
            }
            else
            {
                RealWorldTerrainDownloadItemUnityWebRequest item = new RealWorldTerrainDownloadItemUnityWebRequest(url)
                {
                    filename = filename,
                    averageSize = 600000,
                    exclusiveLock = RealWorldTerrainOSMUtils.OSMLocker,
                    ignoreRequestProgress = true
                };

                item.OnData += OnDownloadComplete;
            }
        }

        public static void Generate(RealWorldTerrainContainer container)
        {
            if (!loaded)
            {
                Load();
                if (ways == null || ways.Count == 0)
                {
                    RealWorldTerrainPhase.phaseComplete = true;
                    return;
                }

                foreach (KeyValuePair<string, RealWorldTerrainOSMWay> pair in ways)
                {
                    new RealWorldTerrainRoadGenerator(pair.Value, container);
                }

                CombineRoads();

                if (RealWorldTerrainWindow.generateTarget is RealWorldTerrainItem)
                {
                    RealWorldTerrainItem item = RealWorldTerrainWindow.generateTarget as RealWorldTerrainItem;
                    roadContainer = RealWorldTerrainUtils.CreateGameObject(container, "Roads " + item.x + "x" + (item.container.terrainCount.y - item.y - 1));
                    roadContainer.transform.position = item.transform.position;
                }
                else roadContainer = RealWorldTerrainUtils.CreateGameObject(container, "Roads");

#if EASYROADS3D
                if (prefs.roadEngine == "EasyRoads3D")
                {
                    if (Object.FindObjectOfType<ERModularBase>() == null)
                    {
                        GameObject roadNetworkGO = Object.Instantiate(Resources.Load("ER Road Network")) as GameObject;
                        roadNetworkGO.name = "Road Network";
                        roadNetworkGO.transform.position = Vector3.zero;
                    }

                    roadNetwork = new ERRoadNetwork();
                    roadNetwork.roadNetwork.importSideObjectsAlert = false;
                    roadNetwork.roadNetwork.importRoadPresetsAlert = false;
                    roadNetwork.roadNetwork.importCrossingPresetsAlert = false;
                    roadNetwork.roadNetwork.importSidewalkPresetsAlert = false;

                    ERRoad[] erRoads = roadNetwork.GetRoads();
                    for (int i = 0; i < erRoads.Length; i++) Object.DestroyImmediate(erRoads[i].gameObject);

                    Material roadMaterial = Resources.Load("Materials/roads/single lane") as Material;
                    if (roadMaterial == null) roadMaterial = Resources.Load("Materials/roads/road material") as Material;

                    float roadWidth = 6;
                    if (prefs.sizeType == 2)
                    {
                        double fromX = prefs.coordinatesFrom.x;
                        double fromY = prefs.coordinatesFrom.y;
                        double toX = prefs.coordinatesTo.x;
                        double toY = prefs.coordinatesTo.y;

                        double rangeX = toX - fromX;

                        double scfY = Math.Sin(fromY * Mathf.Deg2Rad);
                        double sctY = Math.Sin(toY * Mathf.Deg2Rad);
                        double ccfY = Math.Cos(fromY * Mathf.Deg2Rad);
                        double cctY = Math.Cos(toY * Mathf.Deg2Rad);
                        double cX = Math.Cos(rangeX * Mathf.Deg2Rad);
                        double sizeX1 = Math.Abs(RealWorldTerrainUtils.EARTH_RADIUS * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
                        double sizeX2 = Math.Abs(RealWorldTerrainUtils.EARTH_RADIUS * Math.Acos(sctY * sctY + cctY * cctY * cX));
                        double sizeX = (sizeX1 + sizeX2) / 2.0;
                        double sizeZ = RealWorldTerrainUtils.EARTH_RADIUS * Math.Acos(scfY * sctY + ccfY * cctY);

                        RealWorldTerrainVector2i tCount = prefs.terrainCount;
                        const float baseScale = 1000;

                        double sX = sizeX / tCount.x * baseScale * prefs.terrainScale.x;
                        double sZ = sizeZ / tCount.y * baseScale * prefs.terrainScale.z;

                        double scaleX = sX / prefs.fixedTerrainSize.x;
                        double scaleZ = sZ / prefs.fixedTerrainSize.z;
                        roadWidth /= (float)(scaleX + scaleZ) / 2;
                    }

                    //Debug.Log(roadWidth);

                    roadType = new ERRoadType
                    {
                        roadWidth = roadWidth,
                        roadMaterial = roadMaterial
                    };

                    if (prefs.roadTypeMode == RealWorldTerrainRoadTypeMode.advanced)
                    {
                        erRoadTypes = roadNetwork.roadNetwork.GetRoadTypes();
                        roadTypeNames = Enum.GetNames(typeof(RealWorldTerrainRoadType));
                    }
                }
#endif
#if ROADARCHITECT
                if (prefs.roadEngine == "Road Architect")
                {
                    tRoadSystem = roadContainer.AddComponent<GSDRoadSystem>();
                    RealWorldTerrainUtils.CreateGameObject(roadContainer, "Intersections");
                }
#endif

                alreadyCreated = new List<string>();
            }

            RealWorldTerrainTimer timer = RealWorldTerrainTimer.Start();

            for (int i = RealWorldTerrainPhase.index; i < roads.Count; i++)
            {
                RealWorldTerrainRoadGenerator road = roads[i];
                if (road.points.Count < 2) continue;
                if (alreadyCreated.Contains(road.id)) continue;
                alreadyCreated.Add(road.id);
                if (prefs.roadEngine == "EasyRoads3D") road.CreateEasyRoads();
                else if (prefs.roadEngine == "Road Architect") road.CreateRoadArchitectRoad();

                if (timer.seconds > 1)
                {
                    RealWorldTerrainPhase.index = i + 1;
                    RealWorldTerrainPhase.phaseProgress = RealWorldTerrainPhase.index / (float)roads.Count;
                    return;
                }
            }

            Dispose();
            RealWorldTerrainPhase.phaseComplete = true;
        }

        private static void Load()
        {
            if (prefs.roadTypes == 0) return;
            RealWorldTerrainOSMUtils.LoadOSM(compressedFilename, out nodes, out ways, out relations);
            loaded = true;
        }

        private void NormalizeDistance()
        {
            int i = 0;
            while (i < points.Count - 1)
            {
                Vector3 p1 = points[i];
                Vector3 p2 = points[i + 1];
                if ((p1 - p2).magnitude < 10)
                {
                    points[i] = Vector3.Lerp(p1, p2, 0.5f);
                    points.RemoveAt(i + 1);
                }
                else i++;
            }

            i = 0;
            while (i < points.Count - 1)
            {
                Vector3 p1 = points[i];
                Vector3 p2 = points[i + 1];
                if ((p1 - p2).magnitude > 40) points.Insert(i + 1, Vector3.Lerp(p1, p2, 0.5f));
                else i++;
            }
        }

        private void NormalizePoints()
        {
            List<Vector3> newPoints = new List<Vector3>();
            newPoints.AddRange(RealWorldTerrainUtils.SpliceList(points, 0));
            Vector3 lastPoint = newPoints[0];

            while (points.Count > 0)
            {
                Vector3 closestPoint = Vector3.zero;
                float closestDistance = Single.MaxValue;
                foreach (Vector3 point in points)
                {
                    float distance = (new Vector2(point.x, point.z) - new Vector2(lastPoint.x, lastPoint.z)).magnitude;
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPoint = point;
                    }
                }

                newPoints.Add(closestPoint);
                lastPoint = closestPoint;
                points.Remove(closestPoint);
            }

            points = newPoints;
        }

        private static void OnDownloadComplete(ref byte[] data)
        {
            RealWorldTerrainOSMUtils.GenerateCompressedFile(data, ref nodes, ref ways, ref relations, compressedFilename);
        }

        private void TrimPoints()
        {
            int index = 0;
            while (index < points.Count)
            {
                Vector3 p = points[index];
                if (p.x < 0 || p.z < 0 || p.x > container.size.x || p.z > container.size.z) points.RemoveAt(index);
                else index++;
            }
        }
    }
}
