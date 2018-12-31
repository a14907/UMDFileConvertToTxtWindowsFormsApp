namespace UmdParser
{


    public class CoverSection : FileSection
    {
        public CoverSection() : base(MetaDataEnum.Cover)
        {

        }
        public byte[] CoverBuffer { get; set; }
    }
}