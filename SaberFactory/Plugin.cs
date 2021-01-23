using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Utilities;
using SaberFactory.Configuration;
using SaberFactory.Installers;
using SiraUtil.Zenject;
using System.IO;
using IPALogger = IPA.Logging.Logger;

namespace SaberFactory
{

    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        [Init]
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            var saberFactoryDir = new DirectoryInfo(UnityGame.UserDataPath).CreateSubdirectory("Saber Factory");

            var config = conf.Generated<PluginConfig>();
            zenjector.OnApp<AppInstaller>().WithParameters(logger, config, saberFactoryDir);
            zenjector.OnMenu<Installers.MenuInstaller>();
            zenjector.OnGame<GameInstaller>(false);
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
