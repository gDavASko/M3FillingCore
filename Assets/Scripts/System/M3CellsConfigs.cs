using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "M3Core/Config", fileName = "M3CellsConfigs", order = 0)]
public class M3CellsConfigs : ScriptableObject
{
   [SerializeField] private List<ObjectIDPair<StringID, CellsSlotsConfig>> configs = null;

   private List<CellsSlotsConfig> _enabledConfigs = new List<CellsSlotsConfig>();

   private List<CellsSlotsConfig> EnabledConfigs
   {
      get
      {
         if (_enabledConfigs.Count == 0)
         {
            foreach (var config in configs)
            {
               if(config.Obj.Enabled)
                  _enabledConfigs.Add(config.Obj);
            }
         }

         return _enabledConfigs;
      }
   }

   public CellsSlotsConfig GetRandomConfig()
   {
      var config = EnabledConfigs[Random.Range(0, EnabledConfigs.Count - 1)];
      EnabledConfigs.Remove(config);
      return config;
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