using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaberFactory.DataStore;
using SaberFactory.Models;
using SiraUtil.Logging;
using SiraUtil.Web;
using UnityEngine;
using Zenject;

namespace SaberFactory.Misc
{
    internal class RemotePartRetriever : IInitializable, ILoadingTask
    {
#if DEBUG
        public const string RemoteSaberUrl = "https://raw.githubusercontent.com/ToniMacaroni/SaberFactory/development/Saberlist.json";
#else
        public const string RemoteSaberUrl = "https://raw.githubusercontent.com/ToniMacaroni/SaberFactory/main/Saberlist.json";
#endif        

        public Status RetrievingStatus { get; private set; } = Status.Loading;
        public Task CurrentTask { get; private set; }
        
        public List<RemoteLocationPart> RemoteSabers { get; private set; } = new List<RemoteLocationPart>();

        private readonly IHttpService _httpService;
        private readonly DiContainer _container;
        private readonly SiraLog _logger;

        private RemotePartRetriever(IHttpService httpService, DiContainer container, SiraLog logger)
        {
            _httpService = httpService;
            _container = container;
            _logger = logger;
        }

        public void Initialize()
        {
            CurrentTask = Retrieve();
        }

        public async Task Retrieve()
        {
            Debug.LogWarning("Loading remote sabers");
            
            try
            {
                var response = await _httpService.GetAsync(RemoteSaberUrl);
                var content = await response.ReadAsStringAsync();
                var sabers = JsonConvert.DeserializeObject<RemoteSaberListData>(content);
                
                foreach (var saberInitData in sabers.SaberInitDatas)
                {
                    RemoteSabers.Add(_container.Instantiate<RemoteLocationPart>(new object[]{saberInitData}));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RetrievingStatus = Status.Failed;
                return;
            }
            
            RetrievingStatus = Status.Success;
        }

        public void RemoveSaber(RemoteLocationPart saber)
        {
            RemoteSabers.Remove(saber);
        }
        
        public enum Status
        {
            Loading,
            Success,
            Failed
        }
        
        public class RemoteSaberListData
        {
            public List<RemoteLocationPart.InitData> SaberInitDatas { get; set; }
        }
    }
}