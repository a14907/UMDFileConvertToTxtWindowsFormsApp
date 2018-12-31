namespace UmdParser
{
    public class PublisherSection : FileSection
    {
        public PublisherSection() : base(MetaDataEnum.Publisher)
        {

        }
        public string Publisher { get; set; }

        public override string ToString()
        {
            return $"出版人：{Publisher}";
        }
    }
}