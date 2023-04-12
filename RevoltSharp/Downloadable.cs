using System;
using System.Threading.Tasks;

namespace RevoltSharp; 

public class Downloadable<TId, TDownload> {
	private readonly Func<Task<TDownload>> _downloader;
	
	public TId Id { get; }

	public Downloadable(TId id, Func<Task<TDownload>> downloader) {
		Id = id;
		_downloader = downloader;
	}
	
	public Task<TDownload> DownloadAsync() {
		return _downloader();
	}
}
