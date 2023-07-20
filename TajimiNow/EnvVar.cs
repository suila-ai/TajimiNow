using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow
{
    internal static class EnvVar
    {
        public static string? MisskeyServer { get; } = Environment.GetEnvironmentVariable("MISSKEY_SERVER");
        public static string? MisskeyToken { get; } = Environment.GetEnvironmentVariable("MISSKEY_TOKEN");
        public static string? AmedasPointCode { get; } = Environment.GetEnvironmentVariable("MISSKEY_AMEDAS_POINT_CODE");
        public static string? ForecastOfficeCode { get; } = Environment.GetEnvironmentVariable("MISSKEY_FORECAST_OFFICE_CODE");
        public static string? ForecastAreaCode { get; } = Environment.GetEnvironmentVariable("MISSKEY_FORECAST_AREA_CODE");
        public static string AmedasVisibility { get; } = Environment.GetEnvironmentVariable("MISSKEY_AMEDAS_VISIBILITY") ?? "specified";
        public static string ForecastVisibility { get; } = Environment.GetEnvironmentVariable("MISSKEY_FORECAST_VISIBILITY") ?? "specified";
        public static string MinMaxTempVisibility { get; } = Environment.GetEnvironmentVariable("MISSKEY_MIN_MAX_TEMP_VISIBILITY") ?? "specified";
        public static double MaxChanceThreshold { get; } = double.Parse(Environment.GetEnvironmentVariable("MISSKEY_MAX_CHANCE_THRESHOLD") ?? "100.0");
        public static string? MaxChanceFile { get; } = Environment.GetEnvironmentVariable("MISSKEY_MAX_CHANCE_FILE");
        public static string? MaxAchievedFile { get; } = Environment.GetEnvironmentVariable("MISSKEY_MAX_ACHIEVED_FILE");
        public static double MinChanceThreshold { get; } = double.Parse(Environment.GetEnvironmentVariable("MISSKEY_MIN_CHANCE_THRESHOLD") ?? "-100.0");
        public static string? MinChanceFile { get; } = Environment.GetEnvironmentVariable("MISSKEY_MIN_CHANCE_FILE");
        public static string? MinAchievedFile { get; } = Environment.GetEnvironmentVariable("MISSKEY_MIN_ACHIEVED_FILE");

        public static string? ForecastOverwriteDate { get; } = Environment.GetEnvironmentVariable("MISSKEY_FORECAST_OVERWRITE_DATE");
    }
}
