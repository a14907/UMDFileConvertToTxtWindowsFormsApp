namespace UmdParser
{
    public class FileTitleSection : FileSection
    {
        public FileTitleSection() : base(MetaDataEnum.Title)
        {

        }
        public string Title { get; set; }

        public override string ToString()
        {
            return $"文件标题:{Title}";
        }
    }
}