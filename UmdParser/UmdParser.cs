using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace UmdParser
{
    public class DefaultUmdParser : IUmdParser
    {

        public Dictionary<PropertyType, List<PropertySection>> Parse(Stream stream)
        {
            var buf = new byte[33 * 1024];
            var dic = new Dictionary<PropertyType, List<PropertySection>>();
            //前四个字节区分文件类型
            stream.ReadLength(buf, 4);
            //然后是5个固定的字节
            stream.ReadLength(buf, 5);
            //下面一个字节指定文件类型
            var filetype = stream.ReadByte();
            if (filetype != 1)
            {
                throw new Exception("只能读取文件类型的umd文件");
            }
            //接下来2个随机数
            stream.ReadLength(buf, 2);
            dic.Add(PropertyType.FileMeta, new List<PropertySection>());
            //接下来读取文件属性02->ob
            PropertySection p = null;
            do
            {
                p = stream.ReadFileProperty(buf);
                if (p != null)
                {
                    dic[PropertyType.FileMeta].Add(p);
                }
            } while (p != null);
            //章节偏移量
            dic[PropertyType.ChapterOffset] = new List<PropertySection> { stream.ReadChapterOffset(buf) };
            //章节标题
            dic[PropertyType.ChapterTitle] = new List<PropertySection> { stream.ReadChapterTitle(buf, dic[PropertyType.ChapterOffset][0].ChapterOffset.Count) };
            //正文
            dic[PropertyType.Content] = new List<PropertySection> { stream.ReadContent(buf) };
            //封面
            dic[PropertyType.Cover] = new List<PropertySection> { stream.ReadCover(buf) };
            //文件结束 9个字节
            if (9 != (stream.Length - stream.Position))
            {
                Console.WriteLine("文件大小对不上");
            }
            return dic;
        }
    }

}
