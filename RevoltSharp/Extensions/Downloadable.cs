using System;
using System.Threading.Tasks;

namespace RevoltSharp;

/// <summary>
/// Cached object or downloadable from the Revolt instance API.
/// </summary>
public class Downloadable<TId, TDownload>
{
    private readonly Func<Task<TDownload?>> _downloader;

    public TId Id { get; }

    internal Downloadable(TId id, Func<Task<TDownload?>> downloader)
    {
        Id = id;
        _downloader = downloader;
    }

    /// <summary>
    /// Get the object from cache or download it from the Revolt instance API if not cached.
    /// </summary>
    public Task<TDownload?> GetOrDownloadAsync()
    {
        return _downloader();
    }
}
