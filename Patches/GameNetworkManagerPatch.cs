using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static Coins.Plugin;
using static Steamworks.InventoryItem;

namespace Coins.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        private static ManualLogSource logger = Plugin.LoggerInstance;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameNetworkManager.Start))]
        public static void GameNetworkManagerPostfix()
        {
            NetworkManager.Singleton.AddNetworkPrefab(CoinPrefab);
            logger.LogDebug("Added network prefab for coin");
        }
    }
}
