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
                    if (Utils.IsTwoWayCSUR(asset))
                    {
                        RoadSkins.ReplaceTrafficLights(asset);
                        RoadSkins.ReplaceMedianSigns(asset);
                        RoadSkins.ToggleCameras(asset);
                    }

                    Utils.SetColor(asset, RoadSkins.roadColor);
                    if (Utils.IsCSURDerivative(asset))
                    {
                        if (asset.name.Contains("express"))
                        {
                            RoadSkins.DisableStructure(asset);
                        }
                        Utils.LinkDerivative(asset);
                    }
                }
            }
            RoadSkins.SetSidewalkPillars();
            //Change All color
            RoadSkins.ChangeBuiltRoadColor();
            //Refresh networks to apply skin change
            RoadSkins.RefreshNetworks();
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


