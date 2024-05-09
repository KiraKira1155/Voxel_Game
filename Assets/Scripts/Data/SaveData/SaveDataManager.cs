using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveData
{
    public class SaveDataManager
    {
        string directoryPath;
        const string temperatureMap = "/TempuratureMapData.json";
        const string precipitationMap = "/PrecipitationMapData.json";
        const string vegetationMap = "/VegetationMapData.json";
        const string continentalnessMap = "/ContinentalnessMapData.json";

        public void Init()
        {
            directoryPath = Application.dataPath + "/SaveData";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if(!File.Exists(directoryPath + temperatureMap))
                File.Create(directoryPath + temperatureMap);
            
            if (!File.Exists(directoryPath + precipitationMap))
                File.Create(directoryPath + precipitationMap);
            
            if (!File.Exists(directoryPath + vegetationMap))
                File.Create(directoryPath + vegetationMap);
            
            if (!File.Exists(directoryPath + continentalnessMap))
                File.Create(directoryPath + continentalnessMap);
        }

        public void SaveBiomeTypeData(BiomeData biomeData)
        {
            string json = JsonUtility.ToJson(biomeData);
            StreamWriter sw = new StreamWriter(directoryPath + temperatureMap, false);
            sw.WriteLine(json);
            sw.Close();
        }
    }
}
