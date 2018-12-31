namespace UmdParser
{
    public class AuthorSection : FileSection
    {
        public AuthorSection() : base(MetaDataEnum.Author)
        {

        }
        public string Author { get; set; }

        public override string ToString()
        {
            return $"作者:{Author}";
        }
    }
}