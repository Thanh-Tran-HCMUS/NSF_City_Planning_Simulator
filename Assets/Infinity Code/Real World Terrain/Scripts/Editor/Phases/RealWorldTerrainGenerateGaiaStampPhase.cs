/*     INFINITY CODE 2013-2019      */
/*   http://www.infinity-code.com   */

#if GAIA_PRESENT
using System;
using System.IO;
using Gaia;
using InfinityCode.RealWorldTerrain.Generators;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfinityCode.RealWorldTerrain.Phases
{
    public class RealWorldTerrainGenerateGaiaStampPhase : RealWorldTerrainPhase
    {
        private Scanner scanner;
        private Stamper stamper;

        public override string title
        {
            get { return "Generate Gaia Stamp..."; }
        }

        public override void Enter()
        {
            try
            {
                RealWorldTerrainGaiaStampGenerator.Generate();
            }
            catch
            {
                phaseComplete = true;
            }
            if (!phaseComplete) return;

            string basesDir = "Assets/Gaia/Stamps/Bases/";
            if (!Directory.Exists(basesDir)) Directory.CreateDirectory(basesDir);

            string basesDataDir = basesDir + "Data/";
            if (!Directory.Exists(basesDataDir)) Directory.CreateDirectory(basesDataDir);

            GaiaConstants.RawBitDepth bd = GaiaConstants.RawBitDepth.Sixteen;
            int resolution = 0;
            scanner.LoadRawFile(RealWorldTerrainGaiaStampGenerator.fullFilename, GaiaConstants.RawByteOrder.IBM, ref bd, ref resolution);
            scanner.m_featureType = GaiaConstants.FeatureType.Bases;
            scanner.SaveScan();

            AssetDatabase.Refresh();

            Selection.activeGameObject = stamper.gameObject;
            stamper.LoadStamp(basesDir + RealWorldTerrainGaiaStampGenerator.shortFilename + ".jpg");

            Complete();
        }

        public override void Finish()
        {
            if (scanner != null && scanner.gameObject != null) Object.DestroyImmediate(scanner.gameObject);

            scanner = null;
            stamper = null;
        }

        private static string GetAssetPath(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
            return null;
        }

        public override void Start()
        {
            try
            {
                GaiaSessionManager.GetSessionManager();

                GaiaSettings m_settings = (GaiaSettings)Gaia.Utils.GetAssetScriptableObject("GaiaSettings");
                if (m_settings == null) m_settings = GaiaManagerEditor.CreateSettingsAsset();

                GaiaDefaults m_defaults = m_settings.m_currentDefaults;
                GaiaResource m_resources = m_settings.m_currentResources;

                if (TerrainHelper.GetActiveTerrainCount() == 0) m_defaults.CreateTerrain(m_resources);

                GameObject gaiaObj = m_resources.CreateOrFindGaia();
                GameObject stamperObj = GameObject.Find("Stamper");
                if (stamperObj == null)
                {
                    stamperObj = new GameObject("Stamper");
                    stamperObj.transform.parent = gaiaObj.transform;
                    stamper = stamperObj.AddComponent<Stamper>();
                    stamper.m_resources = m_resources;
                    stamper.FitToTerrain();
                    stamperObj.transform.position = new Vector3(stamper.m_x, stamper.m_y, stamper.m_z);
                }
                else stamper = stamperObj.GetComponent<Stamper>();

                GameObject scannerObj = GameObject.Find("Scanner");
                if (scannerObj == null)
                {
                    scannerObj = new GameObject("Scanner");
                    scannerObj.transform.parent = gaiaObj.transform;
                    scannerObj.transform.position = TerrainHelper.GetActiveTerrainCenter(false);
                    scanner = scannerObj.AddComponent<Scanner>();

                    string matPath = GetAssetPath("GaiaScannerMaterial");
                    if (!string.IsNullOrEmpty(matPath)) scanner.m_previewMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                }
                else scanner = scannerObj.GetComponent<Scanner>();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
                phaseComplete = true;
            }
        }
    }
}

#endif