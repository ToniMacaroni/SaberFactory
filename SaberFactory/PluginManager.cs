using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
//using SemVer;
using SiraUtil;
using Hive.Versioning;
using Version = Hive.Versioning.Version;
//using Version = SemVer.Version;

namespace SaberFactory
{
    internal class PluginManager
    {
        private readonly WebClient _webClient;

        private Task<Release> _loadingTask;

        private Version _localVersion;

        private PluginManager(WebClient webClient)
        {
            _webClient = webClient;
        }

        public Version LocalVersion =>
            _localVersion ??= IPA.Loader.PluginManager.GetPluginFromId("SaberFactory").HVersion;

        public async Task<Release> GetNewestReleaseAsync(CancellationToken cancellationToken)
        {
            _loadingTask ??= GetNewestReleaseAsyncInternal(cancellationToken);
            return await _loadingTask;
        }

        private async Task<Release> GetNewestReleaseAsyncInternal(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _webClient.GetAsync("https://api.github.com/repos/ToniMacaroni/SaberFactoryV2/releases", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var releases = response.ContentToJson<Release[]>();
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
            [JsonProperty("tag_name")] public string TagName;
            [JsonProperty("body")] public string Body;
            [JsonProperty("html_url")] public string Url;

            public Version LocalVersion;

            private Version _releaseVersion;

            public Version RemoteVersion => _releaseVersion ??= new Version(TagName);

            private bool? _isLocalNewest;

            public bool IsLocalNewest
            {
                get
                {
                    _isLocalNewest ??= new VersionRange($"<={LocalVersion}").Matches(RemoteVersion);
                    return _isLocalNewest.Value;
                }
            }
        }
    }
}