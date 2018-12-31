namespace UmdParser
{
    public class FileSection
    {
        public FileSection(MetaDataEnum propertyEnum)
        {
            PropertyEnum = propertyEnum;
        }
        protected MetaDataEnum PropertyEnum { get; set; }
    }
}