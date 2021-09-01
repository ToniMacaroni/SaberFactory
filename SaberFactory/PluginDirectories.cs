using System.IO;
using IPA.Utilities;
using SaberFactory.Helpers;

namespace SaberFactory
{
    public class PluginDirectories
    {
        public DirectoryInfo SaberFactoryDir;
        public DirectoryInfo PresetDir;
        public DirectoryInfo CustomSaberDir;

        private const string CustomSabersDirName = "CustomSabers";

        public PluginDirectories()
        {
            var baseDir = new DirectoryInfo(UnityGame.InstallPath);
            var userDataDir = new DirectoryInfo(UnityGame.UserDataPath);
            SaberFactoryDir = userDataDir.CreateSubdirectory("Saber Factory");
            PresetDir = SaberFactoryDir.CreateSubdirectory("Presets");
            CustomSaberDir = baseDir.GetDirectory(CustomSabersDirName);

            if (!CustomSaberDir.Exists)
            {
                CustomSaberDir = SaberFactoryDir.GetDirectory(CustomSabersDirName);
                PathTools.RelativeExtension = Path.Combine(userDataDir.Name, SaberFactoryDir.Name)+Path.DirectorySeparatorChar;
            }

            if (!CustomSaberDir.Exists)
            {
                CustomSaberDir = userDataDir.GetDirectory(CustomSabersDirName);
                PathTools.RelativeExtension = userDataDir.Name+Path.DirectorySeparatorChar;
            }

            if (!CustomSaberDir.Exists)
            {
                CustomSaberDir = baseDir.CreateSubdirectory(CustomSabersDirName);
                PathTools.RelativeExtension = null;
            }
        }
    }
}