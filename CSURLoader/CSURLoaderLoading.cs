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
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                NetInfo asset = PrefabCollection<NetInfo>.GetLoaded(i);
                if (Utils.IsCSUR(asset))
                {
                    Utils.SetElevatedPillar(asset, Utils.GetPillar(asset));
                    Utils.ApplyTexture(asset);
                    Utils.SetOutsideConnection(asset);
                    if (Utils.IsCSURDerivative(asset))
                    {
                        Utils.LinkDerivative(asset);
                    }
                }
            }
           
            // Needs to refresh existing networks, otherwise there will be LOD problems
            for (ushort i = 0; i < NetManager.instance.m_nodes.m_size; i++)
            {
                NetInfo asset = NetManager.instance.m_nodes.m_buffer[i].Info;
                if (Utils.IsCSUR(asset))
                {
                    NetManager.instance.UpdateNode(i);
                }
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


