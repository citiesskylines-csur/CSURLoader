using ICities;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using ColossalFramework;

namespace CSURLoader
{
    public class CSURLoaderLoading : ILoadingExtension
    {
        // called when level loading begins
        public void OnCreated(ILoading loading)
        {
            Utils.textures = new Dictionary<string, Material>();
        }

        // called when level is loaded
        public void OnLevelLoaded(LoadMode mode)
        {
            // loads texture container
            Utils.LoadTextures();
            OptionUI.LoadSetting();
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                NetInfo asset = PrefabCollection<NetInfo>.GetLoaded(i);
                if (Utils.IsCSUR(asset))
                {
                    Utils.ApplyTexture(asset);
                    Utils.SetOutsideConnection(asset);
                    Utils.SetColor(asset, RoadSkins.roadColor);
                    if (Utils.IsCSURDerivative(asset))
                    {
                        Utils.LinkDerivative(asset);
                    }    
                }
                else if(OptionUI.changeAllRoadColor)
                {
                    if ((asset.m_netAI is RoadAI))
                    {
                        //Debug.Log("Process color change for " + asset.name.ToString());
                        Utils.SetColor(asset, RoadSkins.roadColor);
                    }
                }
            }
            //Change All color
            RoadSkins.ChangeBuiltRoadColor();
            RoadSkins.SetSidewalkPillars();
            if (Utils.LOAD_LOD)
            {
                NetManager.instance.RebuildLods();
            }
            OptionUI.levelLoaded = true;
        }

        // called when unloading begins
        public void OnLevelUnloading()
        {
        }

        // called when unloading finished
        public void OnReleased()
        {
            OptionUI.levelLoaded = false;
        }
    }
}


