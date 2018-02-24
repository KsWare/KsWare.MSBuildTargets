using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using KsWare.MSBuildTargets.Nuget.RestApiV3.Contracts;
using Newtonsoft.Json;

namespace KsWare.MSBuildTargets.Nuget.RestApiV3 {

	public class NuGetApiClientV3 {

		public async Task<SearchResult> Search(string packageId, bool preRelease) {
			// https://api-v2v3search-0.nuget.org/query?q=KsWare.MSBuildTargets&prerelease=false
			return await GetJson<SearchResult>($"https://api-v2v3search-0.nuget.org/query?q={packageId}&prerelease={preRelease}");
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

		public async Task<T> GetJson<T>(string api) {
			Debug.WriteLine($"GET {api}");
			using (var client = new HttpClient()) {
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				using (var response = await client.GetAsync(api)) {
					Log(response.StatusCode);
					response.EnsureSuccessStatusCode();

					//					var content = await response.Content.ReadAsAsync<T>();
					var text = await response.Content.ReadAsStringAsync();
					try {
						return JsonConvert.DeserializeObject<T>(text);
					}
					catch (Exception ex) {
						ex.Data.Add("ResponseText", text);
						throw;
					}
				}
			}
		}

		private void Log(HttpStatusCode statusCode) {
			Debug.WriteLine($"done {(int) statusCode} {statusCode}");
		}
	}

}
