using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;

namespace HydraSquito
{
    [BepInPlugin(GUID, "HydraSquito", "0.0.1")]
    public class HydraSquito : BaseUnityPlugin
    {
        private const string GUID = "org.ltmadness.valheim.hydrasquito";

        private static ConfigEntry<int> newBorns;
        public void Awake()
        {
            newBorns = Config.Bind<int>("Settings", "Number of new Deathsquitos", 2, "Number of Deathsquitos born when one is killed");
            Config.Save();

            Harmony.CreateAndPatchAll(typeof(HydraSquito), GUID);
        }

        [HarmonyPatch(typeof(Character), "OnDeath")]
        [HarmonyPostfix]
        public static void OnDamaged(ref Character __instance)
        {
            if (ZNet.instance.IsServer())
            {
                if (__instance.name.Contains("Deathsquito"))
                {
                    GameObject prefab = ZNetScene.instance.GetPrefab("Deathsquito");
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "You dun goofed!", 0, null);

                    for (int i = 0; i < newBorns.Value; i++)
                    {
                        Vector3 b = UnityEngine.Random.insideUnitSphere * 0.5f;
                        Character component3 = UnityEngine.Object.Instantiate<GameObject>(prefab, __instance.transform.position + __instance.transform.forward + Vector3.up + b, Quaternion.identity).GetComponent<Character>();
                    }
                }
            }
        }
    }
}
