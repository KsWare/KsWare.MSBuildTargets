namespace KsWare.MSBuildTargets.Nuget.RestApiV3.Contracts {

	class GetLatestVersionResult {

		public int TotalHits { get; set; }

		public _Data[] Data { get; set; }

		public class _Data {
			public string Id { get; set; }
			public string Version { get; set; }
		}
	}

}