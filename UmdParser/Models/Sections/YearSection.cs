namespace UmdParser
{
    public class YearSection : FileSection
    {
        public YearSection() : base(MetaDataEnum.Year)
        {

        }
        public string Year { get; set; }

        public override string ToString()
        {
            return $"{Year}年";
        }
    }
}