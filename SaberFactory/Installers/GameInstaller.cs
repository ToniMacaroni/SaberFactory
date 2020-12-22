using SaberFactory.Game;
using SiraUtil.Interfaces;
using Zenject;

namespace SaberFactory.Installers
{
    internal class GameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IModelProvider>().To<SFSaberProvider>().AsSingle();
        }
    }
}