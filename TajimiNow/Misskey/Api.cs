using MisskeyDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow.Misskey
{
    internal class Api
    {
        private static readonly MisskeyDotNet.Misskey api;

        static Api()
        {
            var server = Environment.GetEnvironmentVariable("MISSKEY_SERVER");
            var token = Environment.GetEnvironmentVariable("MISSKEY_TOKEN");

            if (server == null || token == null) throw new InvalidOperationException();

            api = new(server, token);
        }

        public static async Task Post(Note note)
        {
            try
            {
                await api.ApiAsync<Dictionary<string, Note>>("notes/create", note);
            }
            catch { throw; }
        }
    }
}
