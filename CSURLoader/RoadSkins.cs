using ColossalFramework;
using UnityEngine;

namespace CSURLoader
{
    public class RoadSkins
    {
        public static Color roadColor = new Color(0.5f, 0.5f, 0.5f);
        public static byte itrafficLight = 1;
        public static byte imedianSign = 1;
        public static bool useCamera = true;
        public static bool disableExpressWalls = false;

        public static readonly string[] trafficLightsAvailable
            = new string[] { "Vanilla", "CSUR" };

        public static readonly string[] medianSignsAvailable
            = new string[] { "None", "CSUR" };

        public static void ChangeAllColor()
        {
            ChangeLoadedRoadColor();
            ChangeBuiltRoadColor();
        }

        public static void ChangeLoadedRoadColor()
        {
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                NetInfo asset = PrefabCollection<NetInfo>.GetLoaded(i);
                if (Utils.IsCSUR(asset))
                {
                    Utils.SetColor(asset, roadColor);
                }
            }
        }

        public static void ChangeBuiltRoadColor()
        {
            for (ushort i = 0; i < Singleton<NetManager>.instance.m_segments.m_size; i++)
            {
                NetInfo asset = Singleton<NetManager>.instance.m_segments.m_buffer[i].Info;
                if (Utils.IsCSUR(asset))
                {
                    Singleton<NetManager>.instance.UpdateSegmentRenderer(i, true);
                }
            }
            for (ushort i = 0; i < Singleton<NetManager>.instance.m_nodes.m_size; i++)
            {
                NetInfo asset = Singleton<NetManager>.instance.m_nodes.m_buffer[i].Info;
                if (Utils.IsCSUR(asset))
                {
                    Singleton<NetManager>.instance.UpdateNodeRenderer(i, true);
                }
            }
            Singleton<NetManager>.instance.UpdateNodeColors();
            Singleton<NetManager>.instance.UpdateSegmentColors();
        }

        public static void LinkSidewalkPillar(string prefabName, string pillar)
        {
            NetInfo pathInfo = PrefabCollection<NetInfo>.FindLoaded(prefabName);
            if (pathInfo == null)
            {
                return;
            }
            PedestrianPathAI pathAI = pathInfo.m_netAI as PedestrianPathAI;
            PedestrianBridgeAI pathElvAI = pathAI.m_elevatedInfo.m_netAI as PedestrianBridgeAI;
            PedestrianBridgeAI pathBridgeAI = pathAI.m_bridgeInfo.m_netAI as PedestrianBridgeAI;
            pathElvAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded(pillar);
            pathBridgeAI.m_bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded(pillar);
        }

        public static void SetSidewalkPillars()
        {
            LinkSidewalkPillar("CSUR Sidewalk 2_Data", "Pedestrian Elevated Pillar");
            LinkSidewalkPillar("CSUR Sidewalk_Data", "Pedestrian Elevated Pillar");
            LinkSidewalkPillar("CSUR SidewalkWithBikeLane__Data", "Pedestrian Elevated Pillar");
            Debug.Log("Successfully set sidewalk pillars");
        }

        public static void ReplaceTrafficLights(NetInfo asset)
        {
            if (!OptionUI.levelLoaded && itrafficLight == 0)
            {
                return;
            }

            // for base modules, just use the module name as the traffic light signature
            // This can be retrieved by taking the substring after the first SPACE.
            string signature = Utils.FStrip(asset.name, ' ');
            // exception 1 is U-turn modules such as CSUR-T 6DR5P=8DR5P where the substring 
            // after the EQUAL sign is the traffic light signature
            if (signature.Contains("="))
            {
                signature = Utils.FStrip(signature, '=');
            }
            // exception 2 is divided asymmetric road such as CSUR 2R3-4R3 where the substring 
            // after the DASH is the traffic light signature
            if (signature.Contains("-"))
            {
                signature = Utils.FStrip(signature, '-');
            }
            Debug.Log(signature);
            // only works on ground mode
            foreach (NetInfo.Lane lane in asset.m_lanes)
            {
                // traffic lights are always on a non-vehicle lane
                if (lane.m_vehicleType == VehicleInfo.VehicleType.None)
                {
                    foreach (NetLaneProps.Prop prop in lane.m_laneProps.m_props)
                    {
                        if (itrafficLight == 1)
                        {
                            if (prop.m_prop &&
                                (prop.m_prop.name == "Traffic Light 02" || prop.m_prop.name == "Traffic Light European 02"))
                            {
                                Debug.Log($"Found traffic light 02 on {asset}");
                                // use Traffic Light 3 for 6--8 lane
                                if (signature.StartsWith("6D") || signature.StartsWith("7D") || signature.StartsWith("8D")
                                    || signature.StartsWith("3R") || signature.StartsWith("4R"))
                                {
                                    prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("CSUR TrafficLight 3_Data");
                                    prop.m_finalProp = prop.m_prop;
                                }
                                // use Traffic Light 4 for 9--14 lane
                                if (signature.StartsWith("9D") || signature.StartsWith("10D")
                                    || signature.StartsWith("11D") || signature.StartsWith("12D")
                                    || signature.StartsWith("13D") || signature.StartsWith("14D")
                                    || signature.StartsWith("5R") || signature.StartsWith("6R")
                                    || signature.StartsWith("7R")
                                    )
                                {
                                    prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("CSUR TrafficLight 4_Data");
                                    prop.m_finalProp = prop.m_prop;
                                }
                            }
                            else if (prop.m_prop &&
                              (prop.m_prop.name == "Traffic Light 02 Mirror" || prop.m_prop.name == "Traffic Light European 02 Mirror"))
                            {
                                // use Traffic Light 3 for 6--8 lane
                                Debug.Log($"Found traffic light 02 Mirror on {asset}");
                                if (signature.StartsWith("6D") || signature.StartsWith("7D") || signature.StartsWith("8D"))
                                {
                                    prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("CSUR TrafficLight 3 Mirror_Data");
                                    prop.m_finalProp = prop.m_prop;
                                }
                                // use Traffic Light 4 for 9--14 lane
                                if (signature.StartsWith("9D") || signature.StartsWith("10D")
                                    || signature.StartsWith("11D") || signature.StartsWith("12D")
                                    || signature.StartsWith("13D") || signature.StartsWith("14D"))
                                {
                                    prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("CSUR TrafficLight 4 Mirror_Data");
                                    prop.m_finalProp = prop.m_prop;
                                }
                            }
                        }
                    }
                }
            }

        }

        public static void ReplaceMedianSigns(NetInfo asset)
        {
            // only works on ground mode
            if (!OptionUI.levelLoaded && imedianSign == 1)
            {
                return;
            }
            foreach (NetInfo.Lane lane in asset.m_lanes)
            {
                // traffic lights are always on a non-vehicle lane
                if (lane.m_vehicleType == VehicleInfo.VehicleType.None)
                {
                    foreach (NetLaneProps.Prop prop in lane.m_laneProps.m_props)
                    {
                        if (imedianSign == 0)
                        {
                            if (prop.m_prop && prop.m_prop.name.Contains("CSUR MidSign"))
                            {
                                prop.m_prop = null;
                                prop.m_finalProp = null;
                            }
                        }
                    }
                }
            }
        }

        public static void ToggleCameras(NetInfo asset)
        {
            // only works on ground mode
            if (!OptionUI.levelLoaded && useCamera)
            {
                return;
            }
            foreach (NetInfo.Lane lane in asset.m_lanes)
            {
                // traffic lights are always on a non-vehicle lane
                if (lane.m_vehicleType == VehicleInfo.VehicleType.None)
                {
                    foreach (NetLaneProps.Prop prop in lane.m_laneProps.m_props)
                    {   
                        if(!useCamera)
                        {
                            if (prop.m_prop && prop.m_prop.name.Contains("CSUR CCTV"))
                            {
                                prop.m_prop = null;
                                prop.m_finalProp = null;
                            }
                        }   
                    }
                }
            }
        }



        public static void DisablePolicyToggle(NetInfo asset)
        {
            // TO BE FILLED
        }

        public static void InvertPolicyToggle(NetInfo asset)
        {
            // TO BE FILLED
        }

        public static void UpdateSkins()
        {
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                NetInfo asset = PrefabCollection<NetInfo>.GetLoaded(i);
                if (Utils.IsCSUR(asset) && Utils.IsTwoWayCSUR(asset)) 
                { 
                    ReplaceTrafficLights(asset);
                    ReplaceMedianSigns(asset);
                    ToggleCameras(asset);
                }
            }
            RefreshNetworks();
        }

        public static void RefreshNetworks()
        {
            // for pillar customization safety, do not refresh any elevated node or segment
            // CURRENTLY road skins only affect ground roads so this is s temporary solution
            for (ushort i = 0; i < Singleton<NetManager>.instance.m_segments.m_size; i++)
            {
                NetInfo asset = Singleton<NetManager>.instance.m_segments.m_buffer[i].Info;
                if (Utils.IsCSUR(asset) && asset.m_netAI.GetType() != typeof(RoadBridgeAI))
                {
                    Singleton<NetManager>.instance.UpdateSegment(i);
                }
            }
            for (ushort i = 0; i < Singleton<NetManager>.instance.m_nodes.m_size; i++)
            {
                NetInfo asset = Singleton<NetManager>.instance.m_nodes.m_buffer[i].Info;
                if (Utils.IsCSUR(asset) && asset.m_netAI.GetType() != typeof(RoadBridgeAI))
                {
                    Singleton<NetManager>.instance.UpdateNode(i);
                }
            }
        }

        public static void DisableStructure(NetInfo asset)
        {
            if (!disableExpressWalls)
            {
                return;
            }
            foreach (NetInfo.Segment segment in asset.m_segments)
            {
                if (segment.m_mesh.name.Contains("structure"))
                {
                    segment.m_forwardRequired |= NetSegment.Flags.BikeBan;
                    segment.m_forwardForbidden |= NetSegment.Flags.BikeBan;
                    segment.m_backwardRequired |= NetSegment.Flags.BikeBan;
                    segment.m_backwardForbidden |= NetSegment.Flags.BikeBan;
                }
            }
        }
    }
}
