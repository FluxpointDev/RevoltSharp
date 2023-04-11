using System;
using System.Threading.Tasks;

namespace RevoltSharp; 

public class Downloadable<T> {
	private readonly Func<Task<T>> _downloader;

	public Downloadable(Func<Task<T>> downloader) {
		_downloader = downloader;
	}
	
	public Task<T> DownloadAsync() {
		return _downloader();
	}
}
