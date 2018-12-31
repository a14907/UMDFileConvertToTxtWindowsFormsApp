namespace UmdParser
{
    public class PublishDateSection : FileSection
    {
        public PublishDateSection() : base(MetaDataEnum.PublishDate)
        {

        }
        public YearSection Year { get; set; }
        public MonthSection Month { get; set; }
        public DaySection Day { get; set; }

        public override string ToString()
        {
            return $"出版日期：{Year}{Month}{Day}";
        }
    }
}