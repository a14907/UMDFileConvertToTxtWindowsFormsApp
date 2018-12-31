namespace UmdParser
{
    public class NovelTypeSection : FileSection
    {
        public NovelTypeSection() : base(MetaDataEnum.NovelType)
        {

        }
        public string NovelType { get; set; }

        public override string ToString()
        {
            return $"作品类别：{NovelType}";
        }
    }
}