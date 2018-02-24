using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using KsWare.MSBuildTargets.Nuget.RestApiV3.Contracts;
using Newtonsoft.Json;

namespace KsWare.MSBuildTargets.Nuget.RestApiV3 {

	public class NuGetApiClientV3 {

		public async Task<string> Search(string packageId, bool preRelease) {
			// https://api-v2v3search-0.nuget.org/query?q=KsWare.MSBuildTargets&prerelease=false
			return await GetJsonText($"https://api-v2v3search-0.nuget.org/query?q={packageId}&prerelease={preRelease}");
		}

		public async Task<string> GetLatestVersion(string packageId, bool preRelease) {
			// https://api-v2v3search-0.nuget.org/query?q=KsWare.MSBuildTargets&prerelease=false
			var jt = await GetJsonText($"https://api-v2v3search-0.nuget.org/query?q={packageId}&prerelease={preRelease}");
			var r = JsonConvert.DeserializeObject<GetLatestVersionResult>(jt);
			return r.Data[0].Version;
		}


		public async Task<string> GetJsonText(string url) {
			Debug.WriteLine($"GET {url}");
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				using (var response = await client.GetAsync(url)) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();
					var content = await response.Content.ReadAsStringAsync();
					return content;
				}
			}
		}

		private void Log(HttpStatusCode statusCode) {
			Debug.WriteLine($"done {(int) statusCode} {statusCode}");
		}
	}

}
/*
// 20180223164023
// https://api-v2v3search-0.nuget.org/query?q=KsWare.MSBuildTargets&prerelease=false

{
  "@context": {
    "@vocab": "http://schema.nuget.org/schema#",
    "@base": "https://api.nuget.org/v3/registration3/"
  },
  "totalHits": 1,
  "lastReopen": "2018-02-23T15:32:00.4739885Z",
  "index": "v3-lucene2-v2v3-20171018",
  "data": [
    {
      "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/index.json",
      "@type": "Package",
      "registration": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/index.json",
      "id": "KsWare.MSBuildTargets",
      "version": "0.1.56",
      "description": "Excecutes commands on build.",
      "summary": "",
      "title": "KsWare.MSBuildTargets",
      "iconUrl": "https://raw.githubusercontent.com/KsWare/KsWare.MSBuildTargets/master/icon.png",
      "licenseUrl": "https://raw.githubusercontent.com/KsWare/KsWare.MSBuildTargets/master/LICENSE",
      "projectUrl": "https://github.com/KsWare/KsWare.MSBuildTargets",
      "tags": [
        "msbuild",
        "NuGet",
        "package",
        "visual-studio",
        "KsWare"
      ],
      "authors": [
        "KsWare"
      ],
      "totalDownloads": 1238,
      "verified": false,
      "versions": [
        {
          "version": "0.1.10",
          "downloads": 33,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.10.json"
        },
        {
          "version": "0.1.12",
          "downloads": 28,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.12.json"
        },
        {
          "version": "0.1.15",
          "downloads": 35,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.15.json"
        },
        {
          "version": "0.1.16",
          "downloads": 34,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.16.json"
        },
        {
          "version": "0.1.21",
          "downloads": 35,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.21.json"
        },
        {
          "version": "0.1.23",
          "downloads": 33,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.23.json"
        },
        {
          "version": "0.1.24",
          "downloads": 33,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.24.json"
        },
        {
          "version": "0.1.25",
          "downloads": 34,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.25.json"
        },
        {
          "version": "0.1.26",
          "downloads": 33,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.26.json"
        },
        {
          "version": "0.1.27",
          "downloads": 33,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.27.json"
        },
        {
          "version": "0.1.28",
          "downloads": 33,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.28.json"
        },
        {
          "version": "0.1.29",
          "downloads": 31,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.29.json"
        },
        {
          "version": "0.1.30",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.30.json"
        },
        {
          "version": "0.1.31",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.31.json"
        },
        {
          "version": "0.1.32",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.32.json"
        },
        {
          "version": "0.1.33",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.33.json"
        },
        {
          "version": "0.1.34",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.34.json"
        },
        {
          "version": "0.1.35",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.35.json"
        },
        {
          "version": "0.1.36",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.36.json"
        },
        {
          "version": "0.1.37",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.37.json"
        },
        {
          "version": "0.1.38",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.38.json"
        },
        {
          "version": "0.1.39",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.39.json"
        },
        {
          "version": "0.1.40",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.40.json"
        },
        {
          "version": "0.1.41",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.41.json"
        },
        {
          "version": "0.1.42",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.42.json"
        },
        {
          "version": "0.1.43",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.43.json"
        },
        {
          "version": "0.1.44",
          "downloads": 28,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.44.json"
        },
        {
          "version": "0.1.45",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.45.json"
        },
        {
          "version": "0.1.46",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.46.json"
        },
        {
          "version": "0.1.47",
          "downloads": 32,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.47.json"
        },
        {
          "version": "0.1.48",
          "downloads": 33,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.48.json"
        },
        {
          "version": "0.1.49",
          "downloads": 29,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.49.json"
        },
        {
          "version": "0.1.55",
          "downloads": 0,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.55.json"
        },
        {
          "version": "0.1.56",
          "downloads": 0,
          "@id": "https://api.nuget.org/v3/registration3/ksware.msbuildtargets/0.1.56.json"
        }
      ]
    }
  ]
}
*/