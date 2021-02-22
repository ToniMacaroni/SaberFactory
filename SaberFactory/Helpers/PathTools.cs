using System.IO;
using IPA.Utilities;

namespace SaberFactory.Helpers
{
    public static class PathTools
    {
        public static string SaberFactoryUserPath => Path.Combine(UnityGame.UserDataPath, "Saber Factory");

        public static string ToFullPath(string relativePath) => Path.Combine(UnityGame.InstallPath, relativePath);

        public static string ToRelativePath(string path) => path.Substring(UnityGame.InstallPath.Length+1);

        public static FileInfo GetFile(this DirectoryInfo dir, string fileName)
        {
            return new FileInfo(Path.Combine(dir.FullName, fileName));
        }

        public static void WriteText(this FileInfo file, string text)
        {
            File.WriteAllText(file.FullName, text);
        }

        public static string ReadText(this FileInfo file)
        {
            return File.ReadAllText(file.FullName);
        }
    }
}