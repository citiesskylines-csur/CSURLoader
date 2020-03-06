using ICities;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using ColossalFramework.UI;

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
            if (!Utils.LoadTextures())
            {
                ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel", true);
                panel.SetMessage("CSUR Loader", "Road texture not found. This indicates that " +
                    $"the asset containing textures for CSUR roads are not loaded into the game. " +
                    $"Please check if the file CSURTextureContainer.crp is present in the workshop" +
                    $"folder for CSUR Loader.", true);
                return;
            }
            OptionUI.LoadSetting();
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                NetInfo asset = PrefabCollection<NetInfo>.GetLoaded(i);
                if (Utils.IsCSUR(asset))
                {
                    Utils.ApplyTexture(asset);
                    Utils.SetOutsideConnection(asset);
                    Utils.ApplyGeneralSkins(asset);
                    if (Utils.IsTwoWayCSUR(asset))
                    {
                        Utils.ApplyIntersectionSkins(asset);
                    }

                    Utils.SetColor(asset, RoadSkins.roadColor);
                    if (Utils.IsCSURDerivative(asset))
                    {
                        Utils.LinkDerivative(asset);
                    } else
                    {
                        Utils.LinkBridgeMode(asset);
                    }
                }
                else if (asset != null && OptionUI.changeAllRoadColor)
                {
                    if (asset.m_netAI is RoadAI)
                    {
                        //Debug.Log("Process color change for " + asset.name.ToString());
                        Utils.SetColor(asset, RoadSkins.roadColor);
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


