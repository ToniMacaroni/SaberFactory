using System.IO;
using System.Reflection;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Utilities;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.Installers;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace SaberFactory
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        [Init]
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            var saberFactoryDir = new DirectoryInfo(UnityGame.UserDataPath).CreateSubdirectory("Saber Factory");

            zenjector.OnApp<AppInstaller>().WithParameters(logger, conf, saberFactoryDir);
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
