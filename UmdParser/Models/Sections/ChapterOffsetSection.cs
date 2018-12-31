using System.Collections.Generic;

namespace UmdParser
{
    public class ChapterOffsetSection : FileSection
    {
        public ChapterOffsetSection() : base(MetaDataEnum.ChapterOffset)
        {

        }
        public List<int> ChapterOffset { get; set; }
    }
}