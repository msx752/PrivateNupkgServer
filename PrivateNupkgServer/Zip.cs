using privatenupkgserver.Models.Nuspec;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace privatenupkgserver
{
    public static class Zip
    {
        private static async Task<byte[]> ReadNuspecRawFromPackageAsync(Stream package, bool leaveOpen = true)
        {
            if (package == null)
                throw new NullReferenceException(nameof(package));
            if (!package.CanRead)
                throw new UnauthorizedAccessException(nameof(package));

            try
            {
                using (var arc = new ZipArchive(package, ZipArchiveMode.Read, leaveOpen))
                {
                    var nuspecEntry = arc.Entries.Where(e => e.FullName == e.Name).FirstOrDefault(e => e.Name.EndsWith(".nuspec"));
                    if (nuspecEntry == null)
                        throw new NullReferenceException(nameof(arc));

                    byte[] output = new byte[1024 * 1024];
                    using (var zipStream = nuspecEntry.Open())
                        await zipStream.ReadAsync(output, 0, output.Length).ContinueWith((t) => Array.Resize(ref output, t.Result));
                    return output;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<Nuspec> ReadNuspecFromPackageAsync(Stream package)
        {
            var nuspecRaw = await ReadNuspecRawFromPackageAsync(package);
            if (!Nuspec.TryParse(nuspecRaw, out var nuspec))
                throw new EntryPointNotFoundException();
            return nuspec;
        }

        public static Nuspec ReadNuspec(string fileName)
        {
            try
            {
                using (var arc = ZipFile.OpenRead(fileName))
                {
                    var nuspecEntry = arc.Entries.Where(e => e.FullName == e.Name).FirstOrDefault(e => e.Name.EndsWith(".nuspec"));
                    if (nuspecEntry == null)
                        return default(Nuspec);
                    using (var zipStream = nuspecEntry.Open())
                    {
                        Nuspec.TryParse(zipStream, out var nuspec);
                        return nuspec;
                    }
                }
            }
            catch
            {
            }
            return default(Nuspec);
        }
    }
}