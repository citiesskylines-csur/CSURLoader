using ColossalFramework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSURLoader
{
    public class RoadSkins
    {
        public static Color roadColor = new Color(0.5f, 0.5f, 0.5f);




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
    }
}
