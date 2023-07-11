using MisskeyDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow
{
    internal class MisskeyApi
    {
        private static Misskey api;

        static MisskeyApi()
        {
            var server = Environment.GetEnvironmentVariable("MISSKEY_SERVER");
            var token = Environment.GetEnvironmentVariable("MISSKEY_TOKEN");

            if (server == null || token == null) throw new InvalidOperationException();

            api = new(server, token);
        }

        public static async Task Post(string text)
        {
            await api.ApiAsync<Dictionary<string, Note>>("notes/create", new
            {
                text,
                visibility = Environment.GetEnvironmentVariable("MISSKEY_VISIBILITY") ?? "followers",

            });
        }
    }
}
