using System.Collections.Generic;

namespace UmdParser
{
    public class PropertySection
    {
        public PropertyEnum PropertyEnum { get; set; }
        public byte[] CoverBuffer { get; set; }
        public string StringExpression { get; set; }
        public long ContentLength { get; set; }
        public List<int> ChapterOffset { get; set; }
        public List<string> ChapterTitle { get; set; }
        public List<string> Content { get; internal set; }
    }
}