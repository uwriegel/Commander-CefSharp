using System.Diagnostics;

namespace Commander
{
    static class FileVersion
    {
        public static bool HasInfo(this FileVersionInfo fvi)
            => fvi != null && !(fvi.FileMajorPart == 0 && fvi.FileMinorPart == 0 && fvi.FileBuildPart == 0 && fvi.FilePrivatePart == 0);

        public static string GetVersion(this FileVersionInfo fvi)
            => fvi.HasInfo() ? $"{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}.{fvi.FilePrivatePart}" : null;

        public static VersionComparer Comparer = new VersionComparer();
    }
}
