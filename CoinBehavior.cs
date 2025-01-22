using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using static Coins.Plugin;

namespace Coins
{
    internal class CoinBehavior : NetworkBehaviour
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public AudioClip CoinCollectSFX;

        static Terminal terminal;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Start()
        {
            if (terminal == null)
            {
                terminal = GameObject.FindObjectOfType<Terminal>();
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out PlayerControllerB player)) { return; }
            player.statusEffectAudio.PlayOneShot(CoinCollectSFX, configCoinVolume.Value);
            CollectCoinServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void CollectCoinServerRpc()
        {
            if (!IsServerOrHost) { return; }
            CollectCoinClientRpc();
            NetworkObject.Despawn(true);
        }

        [ClientRpc]
        public void CollectCoinClientRpc()
        {
            terminal.groupCredits += configCoinValue.Value;
        }
    }
}
