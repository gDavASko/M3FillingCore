using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "M3Core/Config", fileName = "M3CellsConfigs", order = 0)]
public class M3CellsConfigs : ScriptableObject
{
   [SerializeField] private List<ObjectIDPair<StringID, CellsSlotsConfig>> configs = null;

   public CellsSlotsConfig GetRandomConfig()
   {
      return configs[Random.Range(0, configs.Count - 1)].Obj;
   }

   public void SaveSlotsConfig(ObjectIDPair<StringID, CellsSlotsConfig> config)
   {
      for (int i = 0; i < configs.Count; i++)
      {
         if (configs[i].ID == config.ID)
         {
            configs.RemoveAt(i);
         }
      }

      configs.Add(config);
   }
}