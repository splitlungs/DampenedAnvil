using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using HarmonyLib;
using System.Reflection;
using System;

namespace DampenedAnvil
{
    public class DampenedAnvilSystem : ModSystem
    {
        private const double ConfigVersion = 0.01d;
        public const string ConfigFile = "DampenedAnvil_Config.json";
        public static DampenedAnvilConfig Config { get; set; } = null!;
        ICoreServerAPI sApi;
        public override void AssetsLoaded(ICoreAPI api)
        {
            if (!(api is ICoreServerAPI sapi)) return;
            this.sApi = sapi;

            LoadDAConfig();
        }
        public void LoadDAConfig()
        {
            try
            {
                Config = sApi.LoadModConfig<DampenedAnvilConfig>(ConfigFile);
                if (Config == null)
                {
                    Config = new DampenedAnvilConfig();
                    Config.Version = ConfigVersion;
                    sApi.StoreModConfig(Config, ConfigFile);

                    sApi.Logger.Warning("[DampenedAnvil] DampenedAnvil_Config file not found. A new one has been created.");
                }
                else if (Config.Version < ConfigVersion)
                {
                    DampenedAnvilConfig tempConfig = new DampenedAnvilConfig();
                    if (Config.HammerHitDistance != 8f) tempConfig.HammerHitDistance = Config.HammerHitDistance;
                    if (Config.HammerHitVolume != 0.5f) tempConfig.HammerHitVolume = Config.HammerHitVolume;
                    if (Config.HelveHitDistance != 16f) tempConfig.HelveHitDistance = Config.HelveHitDistance;
                    if (Config.HelveHitVolume != 0.5f) tempConfig.HelveHitVolume = Config.HelveHitVolume;

                    tempConfig.Version = ConfigVersion;
                    Config = tempConfig;
                    sApi.StoreModConfig(Config, ConfigFile);

                    sApi.Logger.Warning("[DampenedAnvil] DampenedAnvil_Config file is outdated. Migrated to version {0} successfully.", ConfigVersion);
                }
                else
                    sApi.Logger.Notification("[DampenedAnvil] DampenedAnvil_Config file found. Loaded successfully.");
            }
            catch (Exception e)
            {
                sApi.Logger.Error("[DampenedAnvil] Error loading DampenedAnvil_Config: {0}", e);
                return;
            }
        }

        public override void Start(ICoreAPI api)
        {
            // api.RegisterBlockClass("DampenedAnvilBlock", typeof(DampenedAnvilBlock));
            // api.RegisterBlockEntityClass("DampenedAnvilBE", typeof(DampenedAnvilBE));
            // api.Logger.Notification("[DampenedAnvils] Dampened Anvils started.");

            DoHarmonyPatch(api);
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
