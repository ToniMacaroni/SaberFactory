using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SaberFactory.Configuration;
using SaberFactory.Installers;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace SaberFactory
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static IPALogger Log;

        [Init]
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Log = logger;
            zenjector.OnApp<AppInstaller>().WithParameters(logger, conf.Generated<PluginConfig>());
            zenjector.OnMenu<Installers.MenuInstaller>();
            zenjector.OnGame<GameInstaller>();
        }

        [OnStart]
        public void OnApplicationStart()
        {
        }

        [OnExit]
        public void OnApplicationQuit()
        {
        }
    }
}
