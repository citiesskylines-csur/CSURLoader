using ICities;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

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
            CSURLoaderMod.LoadSetting();
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                NetInfo asset = PrefabCollection<NetInfo>.GetLoaded(i);
                if (Utils.IsCSUR(asset))
                {
                    Utils.ApplyTexture(asset);
                    Utils.SetOutsideConnection(asset);
                    Utils.SetColor(asset, new Color((float)CSURLoaderMod.colorR / 128f, (float)CSURLoaderMod.colorG / 128f, (float)CSURLoaderMod.colorB / 128f));
                    if (Utils.IsCSURDerivative(asset))
                    {
                        Utils.LinkDerivative(asset);
                    }    
                }
            }
            Utils.SetSidewalkPillars();
            if (Utils.LOAD_LOD)
            {
                NetManager.instance.RebuildLods();
            }
        }

        // called when unloading begins
        public void OnLevelUnloading()
        {
        }

        // called when unloading finished
        public void OnReleased()
        {
        }
    }
}


