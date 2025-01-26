using System.Globalization;
namespace RequestsManagementSystem.Core.Extentions
{
    public static class DateExtentionMethods
    {
        public static string ConvertToArabicDate(this DateTime date, bool DateTime = false)
        {
            CultureInfo arabicCulture = new CultureInfo("ar-EG");

            string arabicDate = DateTime? 
                date.ToString("d MMMM hh:mm tt", arabicCulture) // 1 يناير 12:00 ص
                : date.ToString("d MMMM", arabicCulture); // 1 يناير

            return arabicDate;
        }
    }
}