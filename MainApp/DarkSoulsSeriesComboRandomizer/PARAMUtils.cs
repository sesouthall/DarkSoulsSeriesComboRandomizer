using SoulsFormats;

namespace DarkSoulsSeriesComboRandomizer
{
    internal static class PARAMUtils
    {
        public static PARAM LoadParam(IBinder bnd, string nameContains, string paramdefPath)
        {
            var param = PARAM.Read(bnd.Files.Single(f => f.Name.Contains(nameContains)).Bytes);
            param.ApplyParamdef(PARAMDEF.XmlDeserialize(paramdefPath));
            return param;
        }
    }
}