using System;
using System.Threading.Tasks;

namespace RevoltSharp; 

public class Downloadable<TId, TDownload>
{
	private readonly Func<Task<TDownload?>> _downloader;
	
	public TId Id { get; }

	internal Downloadable(TId id, Func<Task<TDownload?>> downloader)
	{
		Id = id;
		_downloader = downloader;
	}
	
	public Task<TDownload?> GetOrDownloadAsync()
	{
		return _downloader();
	}
}
