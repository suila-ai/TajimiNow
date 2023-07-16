using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow.Misskey
{
    internal record Note(string text, string visibility)
    {
        public NoteWithFiles AddFiles(string[] fileIds) => new(text, visibility, fileIds);
    }

    internal record NoteWithFiles(string text, string visibility, string[] fileIds) : Note(text, visibility);
}
