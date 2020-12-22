using SaberFactory.Tests;
using Zenject;

namespace SaberFactory.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LoadingTester>().AsSingle();
        }
    }
}