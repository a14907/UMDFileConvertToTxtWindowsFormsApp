namespace UmdParser
{
    public class DaySection : FileSection
    {
        public DaySection() : base(MetaDataEnum.Day)
        {

        }
        public string Day { get; set; }

        public override string ToString()
        {
            return $"{Day}日";
        }
    }
}