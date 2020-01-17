using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System.IO;
using UnityEngine;

namespace CSURLoader
{
    public class CSURLoaderMod : IUserMod
    {
        public string Name => "CSUR Loader";
        public string Description => "Load shared textures for CSUR roads";
        public static int colorR = 128;
        public static int colorG = 128;
        public static int colorB = 128;
        static UISlider ColorRSlider;
        static UISlider ColorGSlider;
        static UISlider ColorBSlider;

        public void OnSettingsUI(UIHelperBase helper)
        {
            LoadSetting();
            UIHelperBase group = helper.AddGroup("CUSR Road Color Changer");
            ColorRSlider = group.AddSlider("R" + "(" + colorR.ToString() + ")", 0, 256, 1, colorR, onColorRChanged) as UISlider;
            ColorRSlider.parent.Find<UILabel>("Label").width = 500f;
            ColorGSlider = group.AddSlider("G" + "(" + colorG.ToString() + ")", 0, 256, 1, colorG, onColorGChanged) as UISlider;
            ColorGSlider.parent.Find<UILabel>("Label").width = 500f;
            ColorBSlider = group.AddSlider("B" + "(" + colorB.ToString() + ")", 0, 256, 1, colorB, onColorBChanged) as UISlider;
            ColorBSlider.parent.Find<UILabel>("Label").width = 500f;
            SaveSetting();
        }

        private static void onColorRChanged(float newVal)
        {
            colorR = (int)newVal;
            ColorRSlider.tooltip = newVal.ToString();
            ColorRSlider.parent.Find<UILabel>("Label").text = "R" + "(" + colorR.ToString() + ")";
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                NetInfo asset = PrefabCollection<NetInfo>.GetLoaded(i);
                if (Utils.IsCSUR(asset))
                {
                    Utils.SetColor(asset, new Color((float)CSURLoaderMod.colorR / 128f, (float)CSURLoaderMod.colorG / 128f, (float)CSURLoaderMod.colorB / 128f));
                }
            }
            for (ushort i = 0; i < Singleton<NetManager>.instance.m_segments.m_size; i++)
            {
                NetInfo asset = Singleton<NetManager>.instance.m_segments.m_buffer[i].Info;
                if (Utils.IsCSUR(asset))
                {
                    Singleton<NetManager>.instance.UpdateSegment(i);
                }
            }
            Debug.Log($"colorR changed to" + newVal.ToString());
            SaveSetting();
        }

        private static void onColorGChanged(float newVal)
        {
            colorG = (int)newVal;
            ColorGSlider.tooltip = newVal.ToString();
            ColorGSlider.parent.Find<UILabel>("Label").text = "G" + "(" + colorG.ToString() + ")";
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                NetInfo asset = PrefabCollection<NetInfo>.GetLoaded(i);
                if (Utils.IsCSUR(asset))
                {
                    Utils.SetColor(asset, new Color((float)CSURLoaderMod.colorR / 128f, (float)CSURLoaderMod.colorG / 128f, (float)CSURLoaderMod.colorB / 128f));
                }
            }
            for (ushort i = 0; i < Singleton<NetManager>.instance.m_segments.m_size; i++)
            {
                NetInfo asset = Singleton<NetManager>.instance.m_segments.m_buffer[i].Info;
                if (Utils.IsCSUR(asset))
                {
                    Singleton<NetManager>.instance.UpdateSegment(i);
                }
            }
            Debug.Log($"colorG changed to" + newVal.ToString());
            SaveSetting();
        }

        private static void onColorBChanged(float newVal)
        {
            colorB = (int)newVal;
            ColorBSlider.tooltip = newVal.ToString();
            ColorBSlider.parent.Find<UILabel>("Label").text = "B" + "(" + colorB.ToString() + ")";
            for (uint i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                NetInfo asset = PrefabCollection<NetInfo>.GetLoaded(i);
                if (Utils.IsCSUR(asset))
                {
                    Utils.SetColor(asset, new Color((float)CSURLoaderMod.colorR / 128f, (float)CSURLoaderMod.colorG / 128f, (float)CSURLoaderMod.colorB / 128f));
                }
            }
            for (ushort i = 0; i < Singleton<NetManager>.instance.m_segments.m_size; i++)
            {
                NetInfo asset = Singleton<NetManager>.instance.m_segments.m_buffer[i].Info;
                if (Utils.IsCSUR(asset))
                {
                    Singleton<NetManager>.instance.UpdateSegment(i);
                }
            }
            Debug.Log($"colorB changed to" + newVal.ToString());
            SaveSetting();
        }

        public static void SaveSetting()
        {
            //save langugae
            FileStream fs = File.Create("CSURLoader_setting.txt");
            StreamWriter streamWriter = new StreamWriter(fs);
            streamWriter.WriteLine(colorR);
            streamWriter.WriteLine(colorG);
            streamWriter.WriteLine(colorB);
            streamWriter.Flush();
            fs.Close();
        }

        public static void LoadSetting()
        {
            if (File.Exists("CSURLoader_setting.txt"))
            {
                FileStream fs = new FileStream("CSURLoader_setting.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string strLine = sr.ReadLine();
                if (!int.TryParse(strLine, out colorR)) { colorR = 128; }
                strLine = sr.ReadLine();
                if (!int.TryParse(strLine, out colorG)) { colorG = 128; }
                strLine = sr.ReadLine();
                if (!int.TryParse(strLine, out colorB)) { colorB = 128; }
                sr.Close();
                fs.Close();
            }
        }
    }
}
