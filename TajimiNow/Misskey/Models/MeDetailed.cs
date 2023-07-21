using MisskeyDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow.Misskey.Models
{
    internal record MeDetailed
    (
        IReadOnlyList<Note> PinnedNotes
    );
}