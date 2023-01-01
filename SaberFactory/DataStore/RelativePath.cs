using SaberFactory.Helpers;

namespace SaberFactory.DataStore;

/// <summary>
/// Struct that models a relative path to the games base dir
/// </summary>
public readonly struct RelativePath
{
    public readonly string Path;

    public RelativePath(string path)
    {
        Path = path;
    }

    public string ToAbsolutePath()
    {
        return PathTools.ToFullPath(Path);
    }

    public static RelativePath FromAbsolutePath(string path)
    {
        return new RelativePath(PathTools.ToRelativePath(path));
    }

    public static implicit operator string(RelativePath relPath)
    {
        return relPath.Path;
    }
}