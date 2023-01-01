
namespace SaberFactory.UI.Flow
{
    public interface IPresetInfo
    {
        string Name { get; }

        bool IsMonitorOnly { get; set; }
        
        void Delete();
    }
}