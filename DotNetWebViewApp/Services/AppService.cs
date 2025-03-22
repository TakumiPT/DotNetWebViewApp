using System.Globalization;

namespace DotNetWebViewApp.Services
{
    /// <summary>
    /// Service to handle application-related operations.
    /// </summary>
    public class AppService
    {
        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public string GetVersion() => Application.ProductVersion;

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public string GetName() => Application.ProductName;

        /// <summary>
        /// Gets the path of the application.
        /// </summary>
        public string GetAppPath() => Application.StartupPath;

        /// <summary>
        /// Quits the application.
        /// </summary>
        public void Quit() => Application.Exit();

        /// <summary>
        /// Gets the preferred system languages.
        /// </summary>
        public string[] GetPreferredSystemLanguages() => CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(c => c.Name)
            .ToArray();

        /// <summary>
        /// Gets the locale of the application.
        /// </summary>
        public string GetLocale() => CultureInfo.CurrentCulture.Name;

        /// <summary>
        /// Gets the locale country code of the application.
        /// </summary>
        public string GetLocaleCountryCode() => RegionInfo.CurrentRegion.TwoLetterISORegionName;
    }
}
