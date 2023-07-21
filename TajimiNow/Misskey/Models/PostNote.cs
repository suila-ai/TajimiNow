using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TajimiNow.Misskey.Models
{
    internal record PostNote(string text, string visibility)
    {
        public virtual PostNoteWithFiles AddFiles(params string[] fileIds) => new(text, visibility, fileIds);
    }

    internal record PostNoteWithFiles(string text, string visibility, string[] fileIds) : PostNote(text, visibility)
    {
        public override PostNoteWithFiles AddFiles(params string[] fileIds) => new(text, visibility, this.fileIds.Concat(fileIds).ToArray());
    }
}
