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
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        private static ManualLogSource logger = Plugin.LoggerInstance;

        const float verticalOffset = 0.5f;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(StartOfRound.OnShipLandedMiscEvents))]
        public static void OnShipLandedMiscEventsPostfix()
        {
            if (!IsServerOrHost) { return; }

            // Inside
            if (configInsideAmount.Value != 0f)
            {
                int spawnAmount = (int)(RoundManager.Instance.insideAINodes.Length * configInsideAmount.Value);
                logger.LogDebug($"Spawning {spawnAmount} coins inside");

                List<GameObject> nodes = RoundManager.Instance.insideAINodes.ToList();
                for (int i = 0; i < spawnAmount; i++)
                {
                    GameObject node = GetRandomAINode(nodes);
                    nodes.Remove(node);

                    if (node == null)
                    {
                        i--;
                        continue;
                    }

                    CoinBehavior coin = GameObject.Instantiate(CoinPrefab, node.transform.position + Vector3.up * verticalOffset, Quaternion.identity).GetComponent<CoinBehavior>();
                    coin.NetworkObject.Spawn(destroyWithScene: true);
                }
            }

            // Outside
            if (configOutsideAmount.Value != 0f)
            {
                int spawnAmount = (int)(RoundManager.Instance.outsideAINodes.Length * configOutsideAmount.Value);
                logger.LogDebug($"Spawning {spawnAmount} coins outside");

                List<GameObject> nodes = RoundManager.Instance.outsideAINodes.ToList();
                for (int i = 0; i < spawnAmount; i++)
                {
                    GameObject node = GetRandomAINode(nodes);
                    nodes.Remove(node);

                    if (node == null)
                    {
                        i--;
                        continue;
                    }

                    CoinBehavior coin = GameObject.Instantiate(CoinPrefab, node.transform.position + Vector3.up * verticalOffset, Quaternion.identity).GetComponent<CoinBehavior>();
                    coin.NetworkObject.Spawn(destroyWithScene: true);
                }
            }
        }

        static GameObject GetRandomAINode(List<GameObject> nodes)
        {
            int randIndex = UnityEngine.Random.Range(0, nodes.Count);
            return nodes[randIndex];
        }
    }
}
