using System;
using System.Collections.Generic;

namespace SaberFactory.Instances.Trail
{
    /// <summary>
    /// Keeps track of all saber instances
    /// </summary>
    public class SaberInstanceList
    {
        private readonly List<WeakReference<SaberInstance>> _list = new List<WeakReference<SaberInstance>>();

        public PlayerTransforms PlayerTransforms { get; set; }
        
        public int Count => _list.Count;

        public void Add(SaberInstance saberInstance)
        {
            _list.Add(new WeakReference<SaberInstance>(saberInstance));
            saberInstance.PlayerTransforms = PlayerTransforms;
        }
        
        public void Remove(SaberInstance saberInstance)
        {
            _list.Remove(_list.Find(wr => wr.TryGetTarget(out var si) && si == saberInstance));
        }

        public void Clear()
        {
            _list.Clear();
        }

        public List<SaberInstance> GetAll()
        {
            var newList = new List<SaberInstance>();
            for (var i = _list.Count - 1; i >= 0; i--)
            {
                if (_list[i].TryGetTarget(out var saberInstance))
                {
                    newList.Add(saberInstance);
                }
                else
                {
                    _list.RemoveAt(i);
                }
            }

            return newList;
        }
    }
}