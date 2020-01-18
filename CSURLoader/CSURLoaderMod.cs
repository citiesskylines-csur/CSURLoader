
using ICities;
using System.IO;
using UnityEngine;

namespace CSURLoader
{
    public class CSURLoaderMod : IUserMod
    {
        public string Name => "CSUR Loader";
        public string Description => "Load shared textures for CSUR roads";

        public void OnSettingsUI(UIHelperBase helper)
        {
            OptionUI.OnSettingsUI(helper);
        }


    }
}
