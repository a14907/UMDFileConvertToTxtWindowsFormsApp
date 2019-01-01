using System.Collections.Generic;

namespace UmdParser
{
    public class PageOffsetSection : FileSection
    {
        public PageOffsetSection() : base(MetaDataEnum.PageOffset)
        {

        }
        public List<PageOffsetItem> PageOffsetCollection { get; set; }
    }

    public class PageOffsetItem
    {
        public byte[] Type { get; set; }
        public List<int> PageOffset { get; set; }
    }
}