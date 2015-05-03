
namespace Dotpay.Common
{
    public static class EnumExtension
    {
        public static string ToLangString(this System.Enum @enum)
        {
            return @enum.GetType().Name + @enum.ToString("F");
        }
    }

}
