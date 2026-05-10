using System.Diagnostics.CodeAnalysis;

namespace DarkSoulsSeriesComboRandomizer
{
    public interface IModdedFile
    {
        public bool TryUpdate([NotNullWhen(false)] out string? errorMessage);
        public void RevertUpdate();
    }
}