using System.Globalization;


namespace RequestsManagementSystem.Core.Extentions
{
    public static class DateExtentionMethods
    {
        public static string ConvertToArabicDate(this DateTime date)
        {
            CultureInfo arabicCulture = new CultureInfo("ar-EG");

            string arabicDate = date.ToString("d MMMM", arabicCulture);

            return arabicDate;
        }
    }
}
