using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaberFactory.DataStore;
using SaberFactory.Models;
using SiraUtil.Logging;
using SiraUtil.Web;
using Zenject;

namespace SaberFactory.Misc
{
    internal class RemotePartRetriever : IInitializable, ILoadingTask
    {
        public const string RemoteSaberUrl = "https://raw.githubusercontent.com/ToniMacaroni/SaberFactory/main/SaberList.json";

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
            _logger.Warn("Loading remote sabers");
            
            RemoteSabers.Clear();
            
            try
            {
                #if DEBUG
                // var content = File.ReadAllText(@"C:\Users\name1\Desktop\SaberFactory\SaberList.json");
                var response = await _httpService.GetAsync("https://raw.githubusercontent.com/ToniMacaroni/SaberFactory/development/SaberList.json");
                var content = await response.ReadAsStringAsync();
                #else
                var response = await _httpService.GetAsync(RemoteSaberUrl);
                var content = await response.ReadAsStringAsync();
                #endif

                var sabers = JsonConvert.DeserializeObject<RemoteSaberListData>(content);
                
                foreach (var saberInitData in sabers.SaberInitDatas)
                {
                    RemoteSabers.Add(_container.Instantiate<RemoteLocationPart>(new object[]{saberInitData}));
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
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
            [JsonProperty("items")]
            public List<RemoteLocationPart.InitData> SaberInitDatas { get; set; }
        }
    }
}