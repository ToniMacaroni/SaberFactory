using System.IO;
using IPA.Utilities;

namespace SaberFactory.Helpers
{
    public static class PathTools
    {
        public static string RelativeExtension;

        public static string SaberFactoryUserPath => Path.Combine(UnityGame.UserDataPath, "Saber Factory");

        public static string ToFullPath(string relativePath)
        {
            return Path.Combine(UnityGame.InstallPath, relativePath);
        }

        public static string ToRelativePath(string path)
        {
            return path.Substring(UnityGame.InstallPath.Length + 1);
        }

        public static string GetSubDir(string relPath)
        {
            relPath = CorrectRelativePath(relPath);

            var split = relPath.Split(Path.DirectorySeparatorChar);
            if (split.Length < 3)
            {
                return "";
            }

            var output = "";
            for (var i = 1; i < split.Length - 1; i++)
            {
                output += split[i];
                if (i != split.Length - 2)
                {
                    output += "\\";
                }
            }

            return output;
        }

        public static string CorrectRelativePath(string path)
        {
            if (!string.IsNullOrEmpty(RelativeExtension) && path.StartsWith(RelativeExtension))
            {
                return path.Substring(RelativeExtension.Length);
            }

            return path;
        }

        public static FileInfo GetFile(this DirectoryInfo dir, string fileName)
        {
            return new FileInfo(Path.Combine(dir.FullName, fileName));
        }

        public static DirectoryInfo GetDirectory(this DirectoryInfo dir, string dirName, bool create = false)
        {
            if (create)
            {
                return dir.CreateSubdirectory(dirName);
            }

            return new DirectoryInfo(Path.Combine(dir.FullName, dirName));
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