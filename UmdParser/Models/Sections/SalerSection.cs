namespace UmdParser
{
    public class SalerSection : FileSection
    {
        public SalerSection() : base(MetaDataEnum.Saler)
        {

        }
        public string Saler { get; set; }

        public override string ToString()
        {
            return $"销售人：{Saler}";
        }
    }
}