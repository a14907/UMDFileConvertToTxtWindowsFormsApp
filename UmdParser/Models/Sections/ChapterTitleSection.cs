using System.Collections.Generic;

namespace UmdParser
{
    public class ChapterTitleSection : FileSection
    {
        public ChapterTitleSection() : base(MetaDataEnum.ChapterTitle)
        {

        }
        public List<string> ChapterTitle { get; set; }
    }
}