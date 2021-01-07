namespace NAudioService.Shared.Models
{
    interface IAudioObjectIO
    {
        void Open(string sourcePath);

        void Save(string destinationPath);
    }
}
