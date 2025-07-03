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
    public class DampenedAnvilConfigSystem : ModSystem
    {
        private const double ConfigVersion = 1.00d;
        public const string ConfigFile = "DampenedAnvil_Config.json";
        public static DampenedAnvilConfig ClientConfig { get; set; } = null!;
        public static DampenedAnvilConfig ServerConfig { get; set; } = null!;
        ICoreServerAPI sApi;
        ICoreClientAPI cApi;
        // Load before anything else, especially before ConfigLib does anything.
        public override double ExecuteOrder()
        {
            return 0;
        }
        public override void AssetsLoaded(ICoreAPI api)
        {
            base.AssetsLoaded(api);

            if (api.Side == EnumAppSide.Client)
            {
                cApi = api as ICoreClientAPI;
                LoadDAConfig(api);
            }
            else if (api.Side == EnumAppSide.Server)
            {
                sApi = api as ICoreServerAPI;
                LoadDAConfig(api);
            }
        }
        public void LoadDAConfig(ICoreAPI api)
        {
            try
            {
                var config = api.LoadModConfig<DampenedAnvilConfig>(ConfigFile);
                if (config == null)
                {
                    config = new DampenedAnvilConfig();
                    config.Version = ConfigVersion;
                    api.StoreModConfig(config, ConfigFile);

                    api.Logger.Warning("[DampenedAnvil] DampenedAnvil_Config file not found. A new one has been created.");
                }
                else if (config.Version < ConfigVersion)
                {
                    DampenedAnvilConfig tempConfig = new DampenedAnvilConfig();
                    if (config.HammerHitDistance != 8f) tempConfig.HammerHitDistance = config.HammerHitDistance;
                    if (config.HammerHitVolume != 0.5f) tempConfig.HammerHitVolume = config.HammerHitVolume;
                    if (config.HelveHitDistance != 16f) tempConfig.HelveHitDistance = config.HelveHitDistance;
                    if (config.HelveHitVolume != 0.5f) tempConfig.HelveHitVolume = config.HelveHitVolume;
                    if (config.PulverizerHitDistance != 4f) tempConfig.PulverizerHitDistance = config.PulverizerHitDistance;
                    if (config.PulverizerHitVolume != 0.5f) tempConfig.PulverizerHitVolume = config.PulverizerHitVolume;

                    tempConfig.Version = ConfigVersion;
                    config = tempConfig;
                    api.StoreModConfig(config, ConfigFile);

                    api.Logger.Warning("[DampenedAnvil] DampenedAnvil_Config file is outdated. Migrated to version {0} successfully.", ConfigVersion);
                }   

                if (api.Side == EnumAppSide.Server)
                    ServerConfig = config;
                else if (api.Side == EnumAppSide.Client)
                    ClientConfig = config;

                api.Logger.Notification("[DampenedAnvil] DampenedAnvil_Config file found. Loaded successfully.");

            }
            catch (Exception e)
            {
                sApi.Logger.Error("[DampenedAnvil] Error loading DampenedAnvil_Config: {0}", e);
                return;
            }
        }
    }
}
