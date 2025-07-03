using System;
using Vintagestory.GameContent.Mechanics;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using HarmonyLib;
using Vintagestory.API.Util;

namespace DampenedAnvil
{
    [HarmonyPatch]
    public class BEPulverizer_Patch
    {
        [HarmonyPatch(typeof(BEBehaviorMPPulverizer), "OnClientSideImpact")]
        public static bool Prefix(BEBehaviorMPPulverizer __instance, bool right, ref Vec4f ___rightOffset, ref Vec4f ___leftOffset, ref Vec3d ___hitPos,
            ref SimpleParticleProperties ___dustParticles, ref SimpleParticleProperties ___slideDustParticles, ref SimpleParticleProperties ___bitsParticles,
            ref AssetLocation ___hitSound, ref AssetLocation ___crushSound, ref Vec3i ___rightSlidePos, ref Vec3i ___leftSlidePos)
        {
            if (__instance.bepu.IsComplete)
            {
                // __instance.Api.Logger.Event("[DampenedAnvil] Attempting to get Log.");
                string block = __instance.Api.World.BlockAccessor.GetBlockBelow(__instance.Pos).Code;
                // __instance.Api.Logger.Event("[DampenedAnvil] Found block {0}.", block);
                if (!block.Contains("log"))
                    return true;
                float hitDist = DampenedAnvilConfigSystem.ClientConfig.PulverizerHitDistance;
                float hitVol = DampenedAnvilConfigSystem.ClientConfig.PulverizerHitVolume;

                Vec4f vec4f = (right ? ___rightOffset : ___leftOffset);
                int slotId = ((!right) ? 1 : 0);
                ___hitPos.Set((float)__instance.Position.X + 0.5f + vec4f.X, (float)__instance.Position.InternalY + vec4f.Y, (float)__instance.Position.Z + 0.5f + vec4f.Z);
                __instance.Api.World.PlaySoundAt(___hitSound, ___hitPos.X, ___hitPos.Y, ___hitPos.Z, null, randomizePitch: true, hitDist, hitVol);
                if (!__instance.bepu.Inventory[slotId].Empty)
                {
                    ItemStack itemstack = __instance.bepu.Inventory[slotId].Itemstack;
                    __instance.Api.World.PlaySoundAt(___crushSound, ___hitPos.X, ___hitPos.Y, ___hitPos.Z, null, randomizePitch: true, 8f);
                    ___dustParticles.Color = (___bitsParticles.Color = itemstack.Collectible.GetRandomColor(__instance.Api as ICoreClientAPI, itemstack));
                    ___dustParticles.Color &= 16777215;
                    ___dustParticles.Color |= -939524096;
                    ___dustParticles.MinPos.Set(___hitPos.X - 1.0 / 32.0, ___hitPos.Y, ___hitPos.Z - 1.0 / 32.0);
                    ___bitsParticles.MinPos.Set(___hitPos.X - 1.0 / 32.0, ___hitPos.Y, ___hitPos.Z - 1.0 / 32.0);
                    ___slideDustParticles.MinPos.Set(right ? ___rightSlidePos : ___leftSlidePos);
                    ___slideDustParticles.Color = ___dustParticles.Color;
                    __instance.Api.World.SpawnParticles(___bitsParticles);
                    __instance.Api.World.SpawnParticles(___dustParticles);
                    __instance.Api.World.SpawnParticles(___slideDustParticles);
                }
            }

            return false;
        }
    }
}
