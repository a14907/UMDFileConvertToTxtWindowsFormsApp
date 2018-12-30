using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmdParser
{
    public interface IUmdParser
    {
        Dictionary<PropertyType, List<PropertySection>> Parse(Stream stream);
    }

    public class UmdFile
    {

    }
}
