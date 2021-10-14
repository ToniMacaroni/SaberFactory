using Hive.Versioning;
using Newtonsoft.Json;
using SiraUtil.Web;
using System;
using System.Threading;
using System.Threading.Tasks;
using Version = Hive.Versioning.Version;

//using SemVer;

//using Version = SemVer.Version;

namespace SaberFactory
{
    internal class PluginManager
    {
        public Version LocalVersion =>
            _localVersion ??= IPA.Loader.PluginManager.GetPluginFromId("SaberFactory").HVersion;

        private readonly IHttpService _httpService;

        private Task<Release> _loadingTask;

        private Version _localVersion;

        private PluginManager(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Release> GetNewestReleaseAsync(CancellationToken cancellationToken)
        {
            _loadingTask ??= GetNewestReleaseAsyncInternal(cancellationToken);
            return await _loadingTask;
        }

        private async Task<Release> GetNewestReleaseAsyncInternal(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpService.GetAsync("https://api.github.com/repos/ToniMacaroni/SaberFactoryV2/releases", cancellationToken: cancellationToken);
                if (response.Successful)
                {
                    var releases = JsonConvert.DeserializeObject<Release[]>(await response.ReadAsStringAsync());
                    var release = releases[0];
                    release.LocalVersion = LocalVersion;
                    return release;
                }

                return null;
            }
            catch (Exception)
            {
                // ignored
                return null;
            }
        }


        internal class Release
        {
            public Version RemoteVersion => _releaseVersion ??= new Version(TagName);

            public bool IsLocalNewest
            {
                get
                {
                    _isLocalNewest ??= new VersionRange($"<={LocalVersion}").Matches(RemoteVersion);
                    return _isLocalNewest.Value;
                }
            }

            [JsonProperty("body")] public string Body;

            public Version LocalVersion;
            [JsonProperty("tag_name")] public string TagName;
            [JsonProperty("html_url")] public string Url;

            private bool? _isLocalNewest;

            private Version _releaseVersion;
        }
    }
}