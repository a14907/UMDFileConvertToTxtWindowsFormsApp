namespace UmdParser
{
    public class ContentLengthSection : FileSection
    {
        public ContentLengthSection() : base(MetaDataEnum.ContentLength)
        {

        }
        public long ContentLength { get; set; }

        public override string ToString()
        {
            return $"文件长度:{ContentLength}";
        }
    }
}