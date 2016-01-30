﻿using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

using Sulakore;

namespace Tanji.GitHub
{
    public class GitRepository
    {
        private static readonly DataContractJsonSerializer _singleReleaseDeserializer;
        private static readonly DataContractJsonSerializer _multipleReleaseDeserializer;

        public string Owner { get; }
        public string Repository { get; }

        public GitRelease LatestRelease { get; private set; }
        public List<GitRelease> Releases { get; private set; }

        static GitRepository()
        {
            _singleReleaseDeserializer = new DataContractJsonSerializer(typeof(GitRelease));
            _multipleReleaseDeserializer = new DataContractJsonSerializer(typeof(List<GitRelease>));
        }
        public GitRepository(string owner, string repository)
        {
            Owner = owner;
            Repository = repository;
        }

        public async Task<GitRelease> GetLatestReleaseAsync()
        {
            LatestRelease = await DownloadJsonObjectAsync<GitRelease>(
                $"https://api.github.com/repos/{Owner}/{Repository}/releases/latest",
                _singleReleaseDeserializer).ConfigureAwait(false);

            return LatestRelease;
        }
        public async Task<List<GitRelease>> GetReleasesAsync()
        {
            Releases = await DownloadJsonObjectAsync<List<GitRelease>>(
                $"https://api.github.com/repos/{Owner}/{Repository}/releases",
                _multipleReleaseDeserializer).ConfigureAwait(false);

            return Releases;
        }

        protected async Task<T> DownloadJsonObjectAsync<T>(string address, DataContractJsonSerializer serializer)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.Headers[HttpRequestHeader.UserAgent] = SKore.ChromeAgent;

                    byte[] jsonData = await webClient
                        .DownloadDataTaskAsync(address).ConfigureAwait(false);

                    using (var jsonStream = new MemoryStream(jsonData))
                        return (T)serializer.ReadObject(jsonStream);
                }
            }
            catch { return default(T); }
        }
    }
}