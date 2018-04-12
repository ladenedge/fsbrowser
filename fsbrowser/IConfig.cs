
namespace FSBrowser
{
    public interface IConfig
    {
        /// <summary>
        /// Gets the the topmost allowed directory (the "chroot jail") for the filesystem browser.
        /// </summary>
        string HomeDirectory { get; }
    }
}
