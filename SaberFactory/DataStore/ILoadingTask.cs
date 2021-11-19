using System.Threading.Tasks;

namespace SaberFactory.DataStore
{
    internal interface ILoadingTask
    {
        public Task CurrentTask { get; }
    }
}