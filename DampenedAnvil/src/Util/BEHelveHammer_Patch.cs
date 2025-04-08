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
    public class BEHelveHammer_Patch
    {
        [HarmonyPatch(typeof(BEHelveHammer))]
        [HarmonyPatch("get_Angle")]
        public static bool Prefix(BEHelveHammer __instance, ref float __result, 
            ref BEBehaviorMPToggle ___mptoggle, ref BlockEntityAnvil ___targetAnvil, ref bool ___obstructed, ref bool ___didHit, 
            ref double ___angleBefore, ref double ___ellapsedInameSecGrow, ref float ___rnd, ref float ___vibrate)
        {
            __instance.Api.Logger.Event("[DampenedAnvil] Attempting to run Prefix.");
            if (__instance.Api.Side != EnumAppSide.Client || ___targetAnvil == null)
                return true;
            __instance.Api.Logger.Event("[DampenedAnvil] Attempting to get Log.");
            string block = __instance.Api.World.BlockAccessor.GetBlockBelow(___targetAnvil.Pos).Code;
            __instance.Api.Logger.Event("[DampenedAnvil] Found block {0}.", block);
            if (!block.Contains("log"))
                return true;

            __instance.Api.Logger.Event("[DampenedAnvil] Found a Log!");

            if (___mptoggle == null)
            {
                __result = 0f;
                return false;
            }

            if (___obstructed)
            {
                __result = (float)___angleBefore;
                return false;
            }

            double num = __instance.Api.World.Calendar.TotalHours * 60.0 * 2.0;
            double num2 = __instance.facing.Index switch
            {
                3 => ___mptoggle.isRotationReversed() ? 1.9 : 0.6,
                1 => ___mptoggle.isRotationReversed() ? (-0.65) : (-1.55),
                0 => ___mptoggle.isRotationReversed() ? (-0.4) : 1.2,
                _ => ___mptoggle.isRotationReversed() ? 1.8 : 1.2,
            };
            double num3 = Math.Abs(Math.Sin(GameMath.Mod((double)___mptoggle.AngleRad * 2.0 + num2 - (double)___rnd, Math.PI * 20.0)) / 4.5);
            float num4 = (float)num3;
            if (___angleBefore > num3)
            {
                num4 -= (float)(num - ___ellapsedInameSecGrow) * 1.5f;
            }
            else
            {
                ___ellapsedInameSecGrow = num;
            }

            num4 = Math.Max(0f, num4);
            ___vibrate *= 0.5f;
            if (num4 <= 0.01f && !___didHit)
            {
                ___didHit = true;
                ___vibrate = 0.02f;
                if (__instance.Api.Side == EnumAppSide.Client && ___targetAnvil != null)
                {
                    float hitDist = DampenedAnvilSystem.Config.HelveHitDistance;
                    float hitVol = DampenedAnvilSystem.Config.HelveHitVolume;
                    __instance.Api.World.PlaySoundAt(new AssetLocation("sounds/effect/anvilhit"), (float)(__instance.Pos.X + __instance.facing.Normali.X * 3) + 0.5f, (float)__instance.Pos.Y + 0.5f, (float)(__instance.   Pos.Z + __instance.facing.Normali.Z * 3) + 0.5f, null, 0.3f + (float)__instance.Api.World.Rand.NextDouble() * 0.2f, hitDist, hitVol);
                    ___targetAnvil.OnHelveHammerHit();
                }
            }
            if ((double)num4 > 0.2)
            {
                ___didHit = false;
            }

            ___angleBefore = num3;
            float num5 = num4 + (float)Math.Sin(num) * ___vibrate;
            if (___targetAnvil?.WorkItemStack != null)
            {
                num5 = Math.Max(3f / 64f, num5);
            }

            __result = num5;
            return false;
        }
    }
}
