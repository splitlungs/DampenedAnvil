using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent.Mechanics;
using HarmonyLib;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace DampenedAnvil
{
    [HarmonyPatch]
    public class ItemHammer_Patch
    {
        [HarmonyPatch(typeof(ItemHammer), "strikeAnvilSound")]
        public static bool Prefix(ItemHammer __instance, EntityAgent byEntity, bool merge)
        {
            byEntity.Api.Logger.Event("[DampenedAnvil] Attempting to run Prefix.");
            if (byEntity.Api.Side != EnumAppSide.Client)
                return true;
            byEntity.Api.Logger.Event("[DampenedAnvil] Attempting to get Log.");
            
            IPlayer player = (byEntity as EntityPlayer).Player;
            BlockSelection currentBlockSelection = player.CurrentBlockSelection;
            if (currentBlockSelection == null)
            {
                return true;
            }
            string block = byEntity.Api.World.BlockAccessor.GetBlockBelow(currentBlockSelection.Position).Code;
            byEntity.Api.Logger.Event("[DampenedAnvil] Found block {0}.", block);
            if (!block.Contains("log"))
                return true;

            byEntity.Api.Logger.Event("[DampenedAnvil] Found a Log!");

            
            if (player != null && player.CurrentBlockSelection != null)
            {
                float hitDist = DampenedAnvilSystem.Config.HammerHitDistance;
                float hitVol = DampenedAnvilSystem.Config.HammerHitVolume;
                player.Entity.World.PlaySoundAt(merge ? new AssetLocation("sounds/effect/anvilmergehit") : new AssetLocation("sounds/effect/anvilhit"), player.Entity, player, 0.9f + (float)byEntity.World.Rand.NextDouble() * 0.2f, hitDist, hitVol);
            }
            return false;
        }
    }
}
