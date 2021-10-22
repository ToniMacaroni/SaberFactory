using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaberFactory.Models;

namespace SaberFactory.Helpers
{
    internal class RandomUtil
    {
        private readonly Random RNG = new Random();
        private readonly List<int> _lastSelectedRandoms = new List<int> { 1 };

        public int RandomNumber(int count)
        {
            lock (RNG)
            {
                return RNG.Next(count);
            }
        }
        
        public T RandomizeFrom<T>(IList<T> meta)
        {
            if (_lastSelectedRandoms.Count == meta.Count)
            {
                _lastSelectedRandoms.Clear();
            }

            int idx;
            do
            {
                idx = RandomNumber(meta.Count);
            } while (_lastSelectedRandoms.Contains(idx));

            _lastSelectedRandoms.Add(idx);

            return meta[idx];
        }
    }
}