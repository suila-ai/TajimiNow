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
            var server = EnvVar.MisskeyServer;
            var token = EnvVar.MisskeyToken;

            if (server == null || token == null) throw new InvalidOperationException();

            api = new(server, token);
        }

        public static async Task Post(PostNote note)
        {
            try
            {
                await api.ApiAsync<Dictionary<string, PostNote>>("notes/create", note);
            }
            catch { throw; }
        }
    }
}
