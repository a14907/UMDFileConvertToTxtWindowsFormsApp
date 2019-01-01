using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace UmdParser
{
    public class DefaultUmdParser : IUmdParser
    {

        public UmdFile Parse(Stream stream)
        {
            var buf = new byte[33 * 1024];
            var file = new UmdFile();
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
            //接下来读取文件属性02->ob
            FileSection p = null;
            do
            {
                p = stream.ReadFileProperty(buf, file);
            } while (p != null);
            //章节偏移量
            file.ChapterOffset = stream.ReadChapterOffset(buf);
            //章节标题
            file.ChapterTitle = stream.ReadChapterTitle(buf, file.ChapterOffset.ChapterOffset.Count);
            //正文
            file.Content = stream.ReadContent(buf);
            //封面
            file.Cover = stream.ReadCover(buf);
            //页面偏移
            PageOffsetItem pageoffsetItem = null;
            do
            {
                pageoffsetItem = stream.ReadPageOffsetItem(buf);

                if (file.PageOffset == null)
                {
                    file.PageOffset = new PageOffsetSection { PageOffsetCollection = new List<PageOffsetItem>() };
                }
                if (pageoffsetItem != null)
                    file.PageOffset.PageOffsetCollection.Add(pageoffsetItem);
            } while (pageoffsetItem != null);
            //文件结束 9个字节
            stream.ReadByte();
            var end = stream.ReadByte();
            if (end != 0x0c)
            {
                stream.ReadLength(buf, (int)(stream.Length - stream.Position));
                Console.WriteLine("文件大小对不上");
            }

            if (file.Content.Content == null)
            {
                file.Content.Content = new string[file.ChapterOffset.ChapterOffset.Count];
            }

            return file;
        }
    }

}
