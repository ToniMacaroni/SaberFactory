using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FlowUi;
using FlowUi.Helpers;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.Installers;
using SaberFactory.Misc;
using SiraUtil.Zenject;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using Screen = HMUI.Screen;

namespace SaberFactory;

[Plugin(RuntimeOptions.DynamicInit)]
public class Plugin
{
    private const string HarmonyId = "com.tonimacaroni.saberfactory";
    private Harmony _harmony;

    private IPALogger _logger;

    [Init]
    public async void Init(IPALogger logger, Config conf, Zenjector zenjector, PluginMetadata metadata)
    {
        _logger = logger;

        _harmony = new Harmony(HarmonyId);

        var pluginConfig = conf.Generated<PluginConfig>();

        if (!await LoadCsDescriptors())
        {
            return;
        }
        
        FlowUI.Init();

        zenjector.UseLogger(logger);
        zenjector.UseHttpService();

        zenjector.Install<PluginAppInstaller>(Location.App, logger, pluginConfig, metadata);
        zenjector.Install<PluginMenuInstaller>(Location.Menu);
        zenjector.Install<PluginGameInstaller>(Location.Player | Location.MultiPlayer);

        zenjector.Mutate<SaberBurnMarkArea>("Environment", (ctx, ogBurnMark) =>
        {
            var newBurner = CommonHelpers.Upgrade(ogBurnMark, typeof(CustomSaberBurnMarkArea));
            ctx.Container.QueueForInject(newBurner);
        });
    }

    [OnEnable]
    public void OnEnable()
    {
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    [OnDisable]
    public void OnDisable()
    {
        _harmony.UnpatchSelf();
    }

    /// <summary>
    ///     Load the SaberDecriptor / CustomTrail / EventManager classes
    ///     from the CustomSaber namespace so they can be accessed in Saber Factory
    /// </summary>
    private async Task<bool> LoadCsDescriptors()
    {
        try
        {
            Assembly.Load(await Readers.ReadResourceAsync("SaberFactory.Resources.CustomSaberComponents.dll"));
            return true;
        }
        catch (Exception)
        {
            _logger.Info("Couldn't load custom saber descriptors");
            return false;
        }
    }
}