namespace TajimiNow
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var lastTime = DateTime.MinValue;

            while (true)
            {
                if (DateTime.Now - lastTime > TimeSpan.FromMinutes(15))
                {
                    var amedas = await Amedas.Get(52606, DateTime.Now);

                    if (amedas != null)
                    {
                        lastTime = amedas.Time;
                        await MisskeyApi.Post($"🌡 {amedas.Temperature} ℃ 💨 {amedas.WindSpeed} m/s\n☀ {amedas.SunshineHours} min/h 🌧 {amedas.Precipitation1h} mm/h\n({amedas.Time.ToString("HH:mm")})");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }
    }
}