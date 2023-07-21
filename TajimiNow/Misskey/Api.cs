using MisskeyDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TajimiNow.Misskey.Models;

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

        public static async Task<Note> Post(PostNote note)
        {
            try
            {
                var res = await api.ApiAsync<Dictionary<string, Note>>("notes/create", note);
                return res["createdNote"];
            }
            catch { throw; }
        }

        public static async Task<IReadOnlyList<Note>> GetPinnedNotes()
        {
            try
            {
                var res = await api.ApiAsync<MeDetailed>("i");
                return res.PinnedNotes;
            }
            catch { throw; }
        }

        public static async Task Pin(string noteId)
        {
            try
            {
                await api.ApiAsync("i/pin", new { noteId });
            } catch { throw; }
        }
        public static async Task Unpin(string noteId)
        {
            try
            {
                await api.ApiAsync("i/unpin", new { noteId });
            } catch { throw; }
        }

    }
}
