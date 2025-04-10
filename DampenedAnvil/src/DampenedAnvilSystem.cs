using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using HarmonyLib;
using System.Reflection;
using System;
using Vintagestory.GameContent;

namespace DampenedAnvil
{
    public class DampenedAnvilSystem : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            // api.RegisterBlockClass("DampenedAnvilBlock", typeof(BlockAnvil));
            // api.RegisterBlockEntityClass("DampenedAnvilBE", typeof(BlockEntityAnvil));
            
            DoHarmonyPatch(api);

            api.Logger.Notification("[DampenedAnvils] Dampened Anvils started.");
        }

        private static void DoHarmonyPatch(ICoreAPI api)
        {
            if (harmony == null)
            {
                harmony = new Harmony("DampenedAnvilPatch");
                try
                {
                    harmony.PatchAll(Assembly.GetExecutingAssembly());
                    api.Logger.Event("[DampenedAnvil] Dampened Anvil Harmony patches applied successfully.");
                }
                catch (Exception ex)
                {
                    api.Logger.Error($"[DampenedAnvil] Exception during patching: {ex}");
                }
            }
        }
        public override void Dispose()
        {
            harmony?.UnpatchAll("DampenedAnvilPatch");
        }

        private static Harmony harmony;
    }
}
