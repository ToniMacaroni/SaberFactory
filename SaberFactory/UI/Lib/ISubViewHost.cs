namespace SaberFactory.UI.Lib
{
    internal interface ISubViewHost
    {
        bool IsActive { get; }

        void Open();
        void Close();
    }
}