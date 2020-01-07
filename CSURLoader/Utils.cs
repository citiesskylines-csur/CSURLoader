using UnityEngine;
using ColossalFramework;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace CSURLoader
{
    class Utils
    {
        private const string CSUR_REGEX = "CSUR(-(T|R|S))? ([[1-9]?[0-9]D?(L|S|C|R)[1-9]*P?)+(=|-)?([[1-9]?[0-9]D?(L|S|C|R)[1-9]*P?)*";
        public const string CSUR_DUAL_REGEX = "CSUR(-(T|R|S))? ([[1-9]?[0-9]D(L|S|C|R)[1-9]*P?)+(=|-)?([[1-9]?[0-9]D?(L|S|C|R)[1-9]*P?)*";
        public const string CSUR_OFFSET_REGEX = "CSUR(-(T|R|S))? ([[1-9]?[0-9](L|R)[1-9]*P?)+(=|-)?([[1-9]?[0-9](L|R)[1-9]*P?)*";

        public static Dictionary<string, Material> textures;


        public static void LoadTextures()
        {
            // TODO: load texture container within mod
            NetInfo container = PrefabCollection<NetInfo>.FindLoaded("CSURTextureContainer_Data");
            foreach (NetInfo.Segment info in container.m_segments)
            {
                // lod material main texture name is TEXNAME_lod
                string textureKey = info.m_material.name;
                Debug.Log($"loaded {textureKey}");
                textures.Add("CSUR_TEXTURE/" + textureKey, info.m_material);
            }
        }

        public static bool IsCSUR(NetInfo asset)
        {
            if (asset == null || asset.m_netAI.GetType() != typeof(RoadAI)) {
                return false;
            }
            string savenameStripped = asset.name.Substring(asset.name.IndexOf('.') + 1);
            Match m = Regex.Match(savenameStripped, CSUR_REGEX, RegexOptions.IgnoreCase);
            return m.Success;
        }

        public static bool IsCSUROffset(NetInfo asset)
        {
            if (asset == null || (asset.m_netAI.GetType() != typeof(RoadAI) && asset.m_netAI.GetType() != typeof(RoadBridgeAI) && asset.m_netAI.GetType() != typeof(RoadTunnelAI)))
            {
                return false;
            }
            string savenameStripped = asset.name.Substring(asset.name.IndexOf('.') + 1);
            Match m = Regex.Match(savenameStripped, CSUR_OFFSET_REGEX, RegexOptions.IgnoreCase);
            return m.Success;
        }

        public static bool IsCSURDual(NetInfo asset)
        {
            if (asset == null || (asset.m_netAI.GetType() != typeof(RoadAI) && asset.m_netAI.GetType() != typeof(RoadBridgeAI) && asset.m_netAI.GetType() != typeof(RoadTunnelAI)))
            {
                return false;
            }
            string savenameStripped = asset.name.Substring(asset.name.IndexOf('.') + 1);
            Match m = Regex.Match(savenameStripped, CSUR_DUAL_REGEX, RegexOptions.IgnoreCase);
            return m.Success;
        }

        public static bool IsCSURDerivative(NetInfo asset)
        {
            if (asset == null || asset.m_netAI.GetType() != typeof(RoadAI))
            {
                return false;
            }
            string savenameStripped = asset.name.Substring(asset.name.IndexOf('.') + 1);
            Match m = Regex.Match(savenameStripped, CSUR_REGEX + " [a-zA-Z]+", RegexOptions.IgnoreCase);
            return m.Success;
        }

        public static void SetElevatedPillar(NetInfo asset)
        {
            RoadBridgeAI elevatedAI = null;
            if ((asset.m_netAI is RoadBridgeAI) && (Regex.Match(asset.name, "Elevated", RegexOptions.IgnoreCase)).Success && (asset.m_segments.Length != 0))
                elevatedAI = asset.m_netAI as RoadBridgeAI;
            else
                continue;

            //Caculate lane num
            int laneNum = (int)((asset.m_halfWidth - asset.m_pavementWidth) / 3.75 - 0.5);

            if (!CSUROffset.IsCSURDual(asset))
            {
                if (Regex.Match(asset.name, "CSUR-S", RegexOptions.IgnoreCase).Success)
                    laneNum = laneNum - 2;
                else if (Regex.Match(asset.name, "CSUR-T", RegexOptions.IgnoreCase).Success)
                    laneNum = laneNum - 2;

                if (laneNum < 0)
                    laneNum = 0;

                switch (laneNum)
                {
                    case 0:
                    case 1:
                        Debug.Log(asset.name.ToString() + "lane num = " + laneNum.ToString());
                        Debug.Log("Try to Load pillar Ama S-1_Data");
                        elevatedAI.m_bridgePillarOffset = 0.5f;
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama S-1_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama S-1_Data");
                        else
                            Debug.Log("Failed Load pillar Ama S-1_Data");
                        break;
                    case 2:
                        Debug.Log(asset.name.ToString() + "lane num = 2");
                        Debug.Log("Try to Load pillar Ama S-2_Data");
                        elevatedAI.m_bridgePillarOffset = 1f;
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama S-2_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama S-2_Data");
                        else
                            Debug.Log("Failed Load pillar Ama S-2_Data");
                        break;
                    case 3:
                        Debug.Log(asset.name.ToString() + "lane num = 3");
                        Debug.Log("Try to Load pillar Ama S-3_Data");
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama S-3_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama S-3_Data");
                        else
                            Debug.Log("Failed Load pillar Ama S-3_Data");
                        break;
                    case 4:
                        Debug.Log(asset.name.ToString() + "lane num = 4");
                        Debug.Log("Try to Load pillar Ama G-3_Data");
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama G-3_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama G-3_Data");
                        else
                            Debug.Log("Failed Load pillar Ama G-3_Data");
                        break;
                    case 5:
                        Debug.Log(asset.name.ToString() + "lane num = 5");
                        Debug.Log("Try to Load pillar Ama G-4_Data");
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama G-4_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama G-4_Data");
                        else
                            Debug.Log("Failed Load pillar Ama G-4_Data");
                        break;
                    default:
                        Debug.Log(asset.name.ToString() + "lane num = " + laneNum.ToString());
                        Debug.Log("Try to Load pillar Ama G-5_Data");
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama G-5_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama G-5_Data");
                        else
                            Debug.Log("Failed Load pillar Ama G-5_Data");
                        break;
                }
            }
            else
            {
                if (Regex.Match(asset.name, "CSUR-S", RegexOptions.IgnoreCase).Success)
                    laneNum = laneNum - 1;
                else if (Regex.Match(asset.name, "CSUR-T", RegexOptions.IgnoreCase).Success)
                    laneNum = laneNum - 1;
                else if (Regex.Match(asset.name, "CSUR-R", RegexOptions.IgnoreCase).Success)
                    laneNum = laneNum - 1;

                if (laneNum < 0)
                    laneNum = 0;

                laneNum = laneNum * 2;
                switch (laneNum)
                {
                    case 0:
                    case 2:
                        Debug.Log(asset.name.ToString() + "lane num = " + laneNum.ToString());
                        Debug.Log("Try to Load pillar Ama S-2_Data");
                        elevatedAI.m_bridgePillarOffset = 1f;
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama S-2_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama S-2_Data");
                        else
                            Debug.Log("Failed Load pillar Ama S-2_Data");
                        break;
                    case 4:
                        Debug.Log(asset.name.ToString() + "lane num = 4");
                        Debug.Log("Try to Load pillar Ama G-2_Data");
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama G-2_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama G-2_Data");
                        else
                            Debug.Log("Failed Load pillar Ama G-2_Data");
                        break;
                    case 6:
                        Debug.Log(asset.name.ToString() + "lane num = 6");
                        Debug.Log("Try to Load pillar Ama G-6DR_Data");
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama G-6DR_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama G-6DR_Data");
                        else
                            Debug.Log("Failed Load pillar Ama G-6DR_Data");
                        break;
                    case 8:
                        Debug.Log(asset.name.ToString() + "lane num = 8");
                        Debug.Log("Try to Load pillar Ama G-8DR_Data");
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama G-8DR_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama G-8DR_Data");
                        else
                            Debug.Log("Failed Load pillar Ama G-8DR_Data");
                        break;
                    case 10:
                        Debug.Log(asset.name.ToString() + "lane num = 10");
                        Debug.Log("Try to Load pillar Ama G-8DR_Data");
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama G-8DR_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama G-8DR_Data");
                        else
                            Debug.Log("Failed Load pillar Ama G-8DR_Data");
                        break;
                    default:
                        Debug.Log(asset.name.ToString() + "lane num = " + laneNum.ToString());
                        Debug.Log("Try to Load pillar Ama G-8DR_Data");
                        if (PrefabCollection<BuildingInfo>.FindLoaded("Ama G-8DR_Data") != null)
                            elevatedAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Ama G-8DR_Data");
                        else
                            Debug.Log("Failed Load pillar Ama G-8DR_Data");
                        break;
                }
            }
        }

        public static void ApplyTexture(NetInfo asset)
        {
            ApplyTextureSingleMode(asset);
            RoadAI netAI = asset.m_netAI as RoadAI;
            ApplyTextureSingleMode(netAI.m_elevatedInfo);
            ApplyTextureSingleMode(netAI.m_bridgeInfo);
            ApplyTextureSingleMode(netAI.m_slopeInfo);
            ApplyTextureSingleMode(netAI.m_tunnelInfo);

        }

        private static void ApplyTextureSingleMode(NetInfo asset)
        {
            // check whether the mode exists
            if (asset == null) return;
            foreach (NetInfo.Node info in asset.m_nodes)
            {
                string key = info.m_material.name;
                if (key.StartsWith("CSUR_TEXTURE/"))
                {
                    info.m_material = GetSharedMaterial(info.m_material);
                }

            }

            foreach (NetInfo.Segment info in asset.m_segments)
            {
                string key = info.m_material.name;
                if (key.StartsWith("CSUR_TEXTURE/"))
                {
                    info.m_material = GetSharedMaterial(info.m_material);
                }
            }
            asset.InitializePrefab();
            

        }

        private static Material GetSharedMaterial(Material materialKey)
        {
            Material cachedMaterial = materialKey;
            if (materialKey.name.StartsWith("CSUR_TEXTURE/") || materialKey.name.StartsWith("CSUR_LODTEXTURE/"))
            {
                Debug.Log($"Found Shared texture{materialKey.name}");
                Shader shader = materialKey.shader;
                Color color = materialKey.color;
                cachedMaterial = new Material(textures[materialKey.name]);
                cachedMaterial.shader = shader;
                cachedMaterial.color = color;
            }
            return cachedMaterial;
        }

        public static void LinkDerivative(NetInfo asset)
        {
            RoadAI netAI = asset.m_netAI as RoadAI;
            // strip the suffix from derivative name
            string sourceName = asset.name.Substring(asset.name.IndexOf('.') + 1);
            sourceName = sourceName.Substring(0, sourceName.LastIndexOf(' ')) + "_Data";
            NetInfo sourceAsset = PrefabCollection<NetInfo>.FindLoaded(sourceName);
            if (sourceAsset != null)
            {
                Debug.Log($"Linking {asset} to {sourceAsset}");
                RoadAI sourceAI = sourceAsset.m_netAI as RoadAI;
                netAI.m_elevatedInfo = sourceAI.m_elevatedInfo;
                netAI.m_bridgeInfo = sourceAI.m_bridgeInfo;
                netAI.m_slopeInfo = sourceAI.m_slopeInfo;
                netAI.m_tunnelInfo = sourceAI.m_tunnelInfo;
            }
        }


        public static void SetOutsideConnection(NetInfo asset)
        {
            RoadAI netAI = asset.m_netAI as RoadAI;
            netAI.m_outsideConnection = PrefabCollection<BuildingInfo>.FindLoaded("Road Connection");
            RoadBaseAI elevatedAI = netAI.m_elevatedInfo.m_netAI as RoadBaseAI;
            elevatedAI.m_outsideConnection = PrefabCollection<BuildingInfo>.FindLoaded("Road Connection");
            RoadBaseAI tunnelAI = netAI.m_tunnelInfo.m_netAI as RoadBaseAI;
            tunnelAI.m_outsideConnection = PrefabCollection<BuildingInfo>.FindLoaded("Road Connection");

        }

    }

}
