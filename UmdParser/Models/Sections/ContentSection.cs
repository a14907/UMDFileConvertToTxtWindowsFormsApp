using System.Collections.Generic;

namespace UmdParser
{
    public class ContentSection : FileSection
    {
        public ContentSection() : base(MetaDataEnum.Content)
        {

        }
        public string[] Content { get; internal set; }
        public List<byte[]> ContentBuffer { get; set; }
    }
}