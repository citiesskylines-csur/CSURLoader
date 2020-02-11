using ICities;
using System.IO;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace CSURLoader
{
    public class OptionUI
    {
        static byte colorR = 128;
        static byte colorG = 128;
        static byte colorB = 128;
        static UISlider ColorRSlider;
        static UISlider ColorGSlider;
        static UISlider ColorBSlider;
        static bool grayscale;


        public static bool levelLoaded;

        public static void OnSettingsUI(UIHelperBase helper)
        {
            LoadSetting();
            UIHelperBase roadColorGroup = helper.AddGroup("CSUR Road Color Settings");
            ColorRSlider = roadColorGroup.AddSlider("R" + "(" + colorR.ToString() + ")", 0, 255, 1, colorR, OnRedSliderChanged) as UISlider;
            ColorRSlider.parent.Find<UILabel>("Label").width = 500f;
            ColorGSlider = roadColorGroup.AddSlider("G" + "(" + colorG.ToString() + ")", 0, 255, 1, colorG, OnGreenSliderChanged) as UISlider;
            ColorGSlider.parent.Find<UILabel>("Label").width = 500f;
            ColorBSlider = roadColorGroup.AddSlider("B" + "(" + colorB.ToString() + ")", 0, 255, 1, colorB, OnBlueSliderChanged) as UISlider;
            ColorBSlider.parent.Find<UILabel>("Label").width = 500f;
            roadColorGroup.AddCheckbox("Change all sliders together", grayscale, (index) => OnGrayscaleSet(index));

            UIHelperBase roadSkinGroup = helper.AddGroup("CSUR Road Prop Settings (Will take effect after next load)");
            roadSkinGroup.AddDropdown("Traffic Lights", RoadSkins.trafficLightsAvailable, RoadSkins.itrafficLight, (index) => OnTrafficLightSet(index));
            roadSkinGroup.AddDropdown("Median Signs", RoadSkins.medianSignsAvailable, RoadSkins.imedianSign, (index) => OnMedianSignSet(index));
            roadSkinGroup.AddCheckbox("Traffic Cameras", RoadSkins.useCamera, (index) => OnCameraSet(index));
            roadSkinGroup.AddCheckbox("Trigger retaining walls for non-sidewalk roads using Bike Ban policy",
                RoadSkins.disableExpressWalls, (index) => OnDisableExpressWallsSet(index));


            SaveSetting();
        }


        private static void OnTrafficLightSet(int index)
        {
            RoadSkins.itrafficLight = (byte) index;
            //if (levelLoaded) RoadSkins.UpdateSkins();
            SaveSetting();
        }

        private static void OnMedianSignSet(int index)
        {
            RoadSkins.imedianSign = (byte) index;
            //if (levelLoaded) RoadSkins.UpdateSkins();
            SaveSetting();
        }

        private static void OnCameraSet(bool index)
        {
            RoadSkins.useCamera = index;
            //if (levelLoaded) RoadSkins.UpdateSkins();
            SaveSetting();
        }

        private static void OnDisableExpressWallsSet(bool index)
        {
            RoadSkins.disableExpressWalls = index;
            //if (levelLoaded) RoadSkins.UpdateSkins();
            SaveSetting();
        }

        private static void OnGrayscaleSet(bool index)
        {
            grayscale = index;
            SaveSetting();
        }


        private static void OnAllSlidersChanged(float newVal)
        {
            colorR = (byte)newVal;
            colorG = (byte)newVal;
            colorB = (byte)newVal;
            RoadSkins.roadColor.r = colorR / 255f;
            RoadSkins.roadColor.g = colorG / 255f;
            RoadSkins.roadColor.b = colorB / 255f;
            ColorRSlider.tooltip = colorR.ToString();
            ColorRSlider.parent.Find<UILabel>("Label").text = "R" + "(" + colorR.ToString() + ")";
            ColorGSlider.tooltip = colorG.ToString();
            ColorGSlider.parent.Find<UILabel>("Label").text = "G" + "(" + colorG.ToString() + ")";
            ColorBSlider.tooltip = colorB.ToString();
            ColorBSlider.parent.Find<UILabel>("Label").text = "B" + "(" + colorB.ToString() + ")";
            ColorRSlider.value = newVal;
            ColorGSlider.value = newVal;
            ColorBSlider.value = newVal;

            if (levelLoaded) RoadSkins.ChangeAllColor();
            Debug.Log($"colors changed to" + colorB.ToString());
            SaveSetting();

        }

        private static void OnRedSliderChanged(float newVal)
        {
            if (grayscale)
            {
                OnAllSlidersChanged(newVal);
            }
            else
            {
                colorR = (byte)newVal;
                RoadSkins.roadColor.r = colorR / 255f;
                ColorRSlider.tooltip = colorR.ToString();
                ColorRSlider.parent.Find<UILabel>("Label").text = "R" + "(" + colorR.ToString() + ")";
                if (levelLoaded) RoadSkins.ChangeAllColor();
                Debug.Log($"colorR changed to" + colorR.ToString());
                SaveSetting();
            }
        }

        private static void OnGreenSliderChanged(float newVal)
        {
            if (grayscale)
            {
                OnAllSlidersChanged(newVal);
            }
            else
            {
                colorG = (byte)newVal;
                RoadSkins.roadColor.g = colorG / 255f;
                ColorGSlider.tooltip = colorG.ToString();
                ColorGSlider.parent.Find<UILabel>("Label").text = "G" + "(" + colorG.ToString() + ")";
                if (levelLoaded) RoadSkins.ChangeAllColor();
                Debug.Log($"colorR changed to" + colorG.ToString());
                SaveSetting();
            }
        }

        private static void OnBlueSliderChanged(float newVal)
        {
            if (grayscale)
            {
                OnAllSlidersChanged(newVal);
            }
            else
            {
                colorB = (byte)newVal;
                RoadSkins.roadColor.b = colorB / 255f;
                ColorBSlider.tooltip = colorB.ToString();
                ColorBSlider.parent.Find<UILabel>("Label").text = "B" + "(" + colorB.ToString() + ")";
                if (levelLoaded) RoadSkins.ChangeAllColor();
                Debug.Log($"colorR changed to" + colorB.ToString());
                SaveSetting();
            }
        }

        public static void SaveSetting()
        {
            FileStream fs = File.Create("CSURLoader_setting.txt");
            StreamWriter streamWriter = new StreamWriter(fs);
            streamWriter.WriteLine(colorR);
            streamWriter.WriteLine(colorG);
            streamWriter.WriteLine(colorB);
            streamWriter.WriteLine(RoadSkins.itrafficLight);
            streamWriter.WriteLine(RoadSkins.imedianSign);
            streamWriter.WriteLine(RoadSkins.useCamera);
            streamWriter.WriteLine(RoadSkins.disableExpressWalls);
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
                if (!byte.TryParse(strLine, out colorR)) { colorR = 128; }
                strLine = sr.ReadLine();
                if (!byte.TryParse(strLine, out colorG)) { colorG = 128; }
                strLine = sr.ReadLine();
                if (!byte.TryParse(strLine, out colorB)) { colorB = 128; }
                strLine = sr.ReadLine();
                if (!byte.TryParse(strLine, out RoadSkins.itrafficLight)) { RoadSkins.itrafficLight = 1; }
                strLine = sr.ReadLine();
                if (!byte.TryParse(strLine, out RoadSkins.imedianSign)) { RoadSkins.imedianSign = 1; }
                strLine = sr.ReadLine();
                if (!bool.TryParse(strLine, out RoadSkins.useCamera)) { RoadSkins.useCamera = true; }
                strLine = sr.ReadLine();
                if (!bool.TryParse(strLine, out RoadSkins.disableExpressWalls)) { RoadSkins.disableExpressWalls = false; }
                RoadSkins.roadColor.r = colorR / 255f;
                RoadSkins.roadColor.g = colorG / 255f;
                RoadSkins.roadColor.b = colorB / 255f;
                sr.Close();
                fs.Close();
            }
        }
    }
}
