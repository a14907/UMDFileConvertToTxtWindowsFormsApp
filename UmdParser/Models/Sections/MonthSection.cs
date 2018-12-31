namespace UmdParser
{
    public class MonthSection : FileSection
    {
        public MonthSection() : base(MetaDataEnum.Month)
        {

        }
        public string Month { get; set; }

        public override string ToString()
        {
            return $"{Month}月";
        }
    }
}