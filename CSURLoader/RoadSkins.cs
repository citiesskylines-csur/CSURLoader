using ColossalFramework;
using UnityEngine;

namespace CSURLoader
{
    public class RoadSkins
    {
        public static Color roadColor = new Color(0.5f, 0.5f, 0.5f);
        public static byte itrafficLight = 1;
        public static byte imedianSign = 1;
        public static byte iroadArrow = 1;
        public static bool useCamera = true;
        public static bool policyToggleExpressWalls = false;
        public static bool policyInvertUnderground = false;

        public static readonly string[] trafficLightsAvailable
            = new string[] { "Vanilla", "CSUR", "American"};

        public static readonly string[] roadArrowsAvailable
            = new string[] { "Vanilla", "CSUR", "American" };

        public static readonly string[] medianSignsAvailable
            = new string[] { "None", "CSUR", "American"};

        public static void ChangeAllColor()
        {
            ChangeLoadedRoadColor();
            ChangeBuiltRoadColor();
        }


        public enum HorizontalTrafficLightSize : byte
        {
            SMALL,
            MEDIUM,
            LARGE,
        }

        public static HorizontalTrafficLightSize GetTrafficLightSize(string assetName)
        {
            // for base modules, just use the module name as the traffic light signature
            // This can be retrieved by taking the substring after the first SPACE.
            string signature = Utils.FStrip(assetName, ' ');
            // exception 1 is U-turn modules such as CSUR-T 6DR5P=8DR5P where the substring 
            // after the EQUAL sign is the traffic light signature
            if (signature.Contains("="))
            {
                signature = Utils.FStrip(signature, '=');
            }
            // exception 2 is divided asymmetric road such as CSUR 2R3-4R3 where the substring 
            // after the DASH is the traffic light signature
            // use Traffic Light 3 for 6--8 lane
            // use Traffic Light 4 for 9--14 lane
            if (signature.Contains("-"))
            {
                signature = Utils.FStrip(signature, '-');
            }
            if (signature.StartsWith("9D") || signature.StartsWith("10D") || signature.StartsWith("11D")
             || signature.StartsWith("12D") || signature.StartsWith("13D") || signature.StartsWith("14D")
             || signature.StartsWith("5R") || signature.StartsWith("6R") || signature.StartsWith("7R")
             || signature.StartsWith("6DR4") || signature.StartsWith("6DR44") || signature.StartsWith("6DR4P4") || signature.StartsWith("6DR5P4")
             || signature.StartsWith("8DR2") || signature.StartsWith("8DR52")
             || signature.StartsWith("6DR6") || signature.StartsWith("8DR4")) 
            {
                return HorizontalTrafficLightSize.LARGE;
            } else if (signature.StartsWith("6D") || signature.StartsWith("7D") || signature.StartsWith("8D")
                    || signature.StartsWith("3R") || signature.StartsWith("4R")
                    || signature.StartsWith("4DR4") || signature.StartsWith("4DR34") || signature.StartsWith("4DR4P4") || signature.StartsWith("4DR5P4")
                    || signature.StartsWith("6DR2") || signature.StartsWith("6DR42") || signature.StartsWith("6DR5P2")
                )
            {
                return HorizontalTrafficLightSize.MEDIUM;
            } else 
            {
                return HorizontalTrafficLightSize.SMALL;
            }
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
                else if (OptionUI.changeAllRoadColor)
                {
                    if ((asset.m_netAI is RoadAI))
                    {
                        //Debug.Log("Process color change for " + asset.name.ToString());
                        Utils.SetColor(asset, roadColor);
                    }
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
            LinkSidewalkPillar("CSUR SidewalkWithBikeLane_Data", "Pedestrian Elevated Pillar");
            Debug.Log("Successfully set sidewalk pillars");
        }


        public static void ReplaceArrows(NetInfo asset)
        {
            if (!OptionUI.levelLoaded && iroadArrow == 1)
            {
                return;
            }
            if (asset.m_netAI is RoadAI)
            {
                RoadAI roadAI = asset.m_netAI as RoadAI;
                ReplaceArrows(roadAI.m_elevatedInfo);
                ReplaceArrows(roadAI.m_bridgeInfo);
                ReplaceArrows(roadAI.m_tunnelInfo);
                ReplaceArrows(roadAI.m_slopeInfo);
            }
            foreach (NetInfo.Lane lane in asset.m_lanes)
            {
                // arrows are always on a car lane
                if (lane.m_vehicleType == VehicleInfo.VehicleType.Car)
                {
                    foreach (NetLaneProps.Prop prop in lane.m_laneProps.m_props)
                    {
                        if (prop.m_flagsRequired == NetLane.Flags.Forward && prop.m_flagsForbidden == NetLane.Flags.LeftRight)
                        {
                            if (iroadArrow == 0)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("Road Arrow F");
                            } else if (iroadArrow == 2)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("[US] Arrow Straight_Data");
                            }
                            prop.m_finalProp = prop.m_prop;
                        }
                        if (prop.m_flagsRequired == NetLane.Flags.ForwardRight && prop.m_flagsForbidden == NetLane.Flags.Left)
                        {
                            if (iroadArrow == 0)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("Road Arrow FR");
                            }
                            else if (iroadArrow == 2)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("[US] Arrow Straight or Right_Data");
                            }
                            prop.m_finalProp = prop.m_prop;
                        }
                        if (prop.m_flagsRequired == NetLane.Flags.Left && prop.m_flagsForbidden == NetLane.Flags.ForwardRight)
                        {
                            if (iroadArrow == 0)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("Road Arrow L");
                            }
                            else if (iroadArrow == 2)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("[US] Arrow Left_Data");
                            }
                            prop.m_finalProp = prop.m_prop;
                        }
                        if (prop.m_flagsRequired == NetLane.Flags.LeftForward && prop.m_flagsForbidden == NetLane.Flags.Right)
                        {
                            if (iroadArrow == 0)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("Road Arrow LF");
                            }
                            else if (iroadArrow == 2)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("[US] Arrow Straight or Left_Data");
                            }
                            prop.m_finalProp = prop.m_prop;
                        }
                        if (prop.m_flagsRequired == NetLane.Flags.Right && prop.m_flagsForbidden == NetLane.Flags.LeftForward)
                        {
                            if (iroadArrow == 0)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("Road Arrow R");
                            }
                            else if (iroadArrow == 2)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("[US] Arrow Right_Data");
                            }
                            prop.m_finalProp = prop.m_prop;
                        }
                    }
                }
            }
        }

        public static void ReplaceTrafficLights(NetInfo asset)
        {
            if (!OptionUI.levelLoaded && itrafficLight == 0)
            {
                return;
            }

            HorizontalTrafficLightSize trafficLightSize = GetTrafficLightSize(asset.name);

            // only works on ground mode
            foreach (NetInfo.Lane lane in asset.m_lanes)
            {
                // traffic lights are always on a non-vehicle lane
                if (lane.m_vehicleType == VehicleInfo.VehicleType.None)
                {
                    foreach (NetLaneProps.Prop prop in lane.m_laneProps.m_props)
                    {
                        if (itrafficLight == 1 || itrafficLight == 2)
                        {
                            if (prop.m_prop &&
                                (prop.m_prop.name == "Traffic Light 02" || prop.m_prop.name == "Traffic Light European 02"))
                            {
                               
                                if (trafficLightSize == HorizontalTrafficLightSize.MEDIUM)
                                {
                                    if (itrafficLight == 1)
                                    {
                                        prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("CSUR TrafficLight 3_Data");
                                    }
                                    else if (itrafficLight == 2)
                                    {
                                        prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("New Traffic Light Grey 23_Data");
                                    }
                                    prop.m_finalProp = prop.m_prop;
                                }
                                else if (trafficLightSize == HorizontalTrafficLightSize.LARGE)
                                {
                                    if (itrafficLight == 1)
                                    {
                                        prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("CSUR TrafficLight 4_Data");
                                    }
                                    else if (itrafficLight == 2)
                                    {
                                        prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("New Traffic Light Grey 24_Data");
                                    }
                                    prop.m_finalProp = prop.m_prop;
                                }
                                else
                                {
                                    if (itrafficLight == 2)
                                    {
                                        prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("New Traffic Light Grey 11_Data");
                                        prop.m_finalProp = prop.m_prop;
                                    }
                                }
                            }
                            if (itrafficLight == 2 && prop.m_prop &&
                                (prop.m_prop.name == "Traffic Light 01" || prop.m_prop.name == "Traffic Light European 01"))
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("New Traffic Light Grey 01_Data");
                                prop.m_finalProp = prop.m_prop;
                            }
                            if (itrafficLight == 1 && prop.m_prop &&
                                  (prop.m_prop.name == "Traffic Light 02 Mirror" || prop.m_prop.name == "Traffic Light European 02 Mirror"))
                            {
                                if (trafficLightSize == HorizontalTrafficLightSize.MEDIUM)
                                {
                                    prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("CSUR TrafficLight 3 Mirror_Data");
                                    prop.m_finalProp = prop.m_prop;
                                }
                                else if (trafficLightSize == HorizontalTrafficLightSize.LARGE)
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
                        if (prop.m_prop && prop.m_prop.name.Contains("CSUR MidSign"))
                        {
                            if (imedianSign == 0)
                            {
                                prop.m_prop = null;
                                prop.m_finalProp = null;
                            } else if (imedianSign == 2)
                            {
                                prop.m_prop = PrefabCollection<PropInfo>.FindLoaded("R2 R4-7 Keep Right Sign_Data");
                                prop.m_finalProp = prop.m_prop;
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

        public static void ToggleStructure(NetInfo asset)
        {
            if (!policyToggleExpressWalls)
            {
                return;
            }
            foreach (NetInfo.Segment segment in asset.m_segments)
            {
                if (segment.m_mesh.name.Contains("structure"))
                {
                    segment.m_forwardRequired |= NetSegment.Flags.BikeBan;
                    segment.m_backwardRequired |= NetSegment.Flags.BikeBan;
                }
            }
        }

        public static void InvertUndergroundPolicyToggle(NetInfo asset)
        {
            if (!policyInvertUnderground)
            {
                return;
            }
            RoadAI roadAI = asset.m_netAI as RoadAI;
            foreach (NetInfo.Segment segment in roadAI.m_slopeInfo.m_segments)
            {
                if (segment.m_mesh.name.Contains("lines"))
                {
                    segment.m_forwardRequired ^= NetSegment.Flags.BikeBan;
                    segment.m_forwardForbidden ^= NetSegment.Flags.BikeBan;
                    segment.m_backwardRequired ^= NetSegment.Flags.BikeBan;
                    segment.m_backwardForbidden ^= NetSegment.Flags.BikeBan;
                }
            }
            foreach (NetInfo.Segment segment in roadAI.m_tunnelInfo.m_segments)
            {
                if (segment.m_mesh.name.Contains("lines"))
                {
                    segment.m_forwardRequired ^= NetSegment.Flags.BikeBan;
                    segment.m_forwardForbidden ^= NetSegment.Flags.BikeBan;
                    segment.m_backwardRequired ^= NetSegment.Flags.BikeBan;
                    segment.m_backwardForbidden ^= NetSegment.Flags.BikeBan;
                }
            }
        }
    }
}
