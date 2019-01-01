using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UmdParser
{
    public static class StreamExtension
    {
        public static PageOffsetItem ReadPageOffsetItem(this Stream stream, byte[] buf)
        {
            int flag = stream.ReadByte();
            if (flag != 0x23)
            {
                throw new Exception("页面偏移处读取错误");
            }
            int type = stream.ReadByte();
            if (type != (int)MetaDataEnum.PageOffset)
            {
                if (type == (int)MetaDataEnum.FileEnd)
                {
                    //页面偏移
                    stream.Position = stream.Position - 2;
                    return null;
                }
                throw new Exception("页面偏移处读取错误");
            }
            //14个无用字节
            stream.ReadByte();
            stream.ReadLength(buf, 4);
            var offsetType = buf.Take(4).ToArray();
            stream.ReadLength(buf, 9);
            //4个字节的数据和页面偏移有关，内容是偏移数据字节数+9
            stream.ReadLength(buf, 4);
            var pageCount = (BitConverter.ToInt32(buf, 0) - 9) / 4;
            var offsetDatals = new List<int>(pageCount);
            for (int i = 0; i < pageCount; i++)
            {
                stream.ReadLength(buf, 4);
                var olen = BitConverter.ToInt32(buf, 0);
                offsetDatals.Add(olen);
            }
            return new PageOffsetItem
            {
                PageOffset = offsetDatals,
                Type = offsetType
            };
        }

        public static CoverSection ReadCover(this Stream stream, byte[] buf)
        {
            int flag = stream.ReadByte();
            if (flag != 0x23)
            {
                throw new Exception("封面处读取错误");
            }
            int type = stream.ReadByte();
            if (type != (int)MetaDataEnum.Cover)
            {
                if (type == (int)MetaDataEnum.PageOffset)
                {
                    //页面偏移
                    stream.Position = stream.Position - 2;
                    return null;
                }
                throw new Exception("封面处读取错误");
            }
            //13个无用字节
            stream.ReadLength(buf, 13);
            //4个字节的数据和封面字节数有关，内容是封面字节数+9，然后再写入封面字节数据，不需要压缩
            stream.ReadLength(buf, 4);
            var len = BitConverter.ToInt32(buf, 0) - 9;
            if (len > buf.Length)
            {
                buf = new byte[len];
            }
            stream.ReadLength(buf, len);
            return new CoverSection
            {
                CoverBuffer = buf.Take(len).ToArray()
            };

        }
        public static ContentSection ReadContent(this Stream stream, byte[] buf)
        {
            var res = new List<byte[]>();
            do
            {
                //一个0x24
                int flag = stream.ReadByte();
                if (flag == 0x23)
                {
                    //在写完每个数据块后，可以选择做如下两件事的一件或两件（建议用随机数来决定）：
                    var t2 = stream.ReadByte();
                    if (t2 == 0xf1)
                    {
                        //1．  写入1个字节’#’，2个字节的0xf1,2个字节的0x1500，16个字节的0x0
                        stream.ReadLength(buf, 19);
                        continue;
                    }
                    else if (t2 == 0x0a)
                    {
                        //2．  写入1个字节’#’，2个字节的0x0a,2个字节的0x0900，4个字节的随机数
                        stream.ReadLength(buf, 7);
                        continue;
                    }
                    else if (t2 == 0x81)
                    {
                        //在所有正文数据块写入完毕后，写入1个字节’#’,2个字节数据类型0x81，表示正文写入完毕，
                        //2个字节0x0901，4个字节随机数，1个字节0x24，4个字节随机数（一致），
                        //接下来是4个字节，内容是数据块的数目*4+9，
                        //然后，还记得每个数据块写入前都生成了4个字节的随机数吗，
                        //从最后一个开始，倒序写如这些随机数，每个4个字节，结束正文的写入。
                        stream.ReadLength(buf, 12);
                        stream.ReadLength(buf, 4);
                        int l = BitConverter.ToInt32(buf, 0) - 9;
                        stream.ReadLength(buf, l);
                        break;
                    }
                }
                //四个随机数字节
                stream.ReadLength(buf, 4);
                //四个字节，表示长度
                stream.ReadLength(buf, 4);
                int len = BitConverter.ToInt32(buf, 0) - 9;
                //读取
                if (len > buf.Length)
                {
                    throw new Exception("缓冲区太小");
                }
                stream.ReadLength(buf, len);
                var t = buf.ZLibBufferToString(len);
                res.Add(t);
            } while (true);
            return new ContentSection { ContentBuffer = res };
        }
        public static ChapterTitleSection ReadChapterTitle(this Stream stream, byte[] buf, int chapterCount)
        {
            //#
            stream.ReadByte();
            //type
            stream.ReadLength(buf, 2);
            int type = BitConverter.ToInt16(buf, 0);
            if (type != (int)FileSectionType.ChapterTitle)
            {
                return null;
            }
            //读取11无用的字节
            stream.ReadLength(buf, 11);
            //4字节，和标题长度有关，章节1标题长度*2 + 1) + (章节2标题长度*2+1)+…+9
            stream.ReadLength(buf, 4);
            int len = BitConverter.ToInt32(buf, 0) - 9;
            //然后就是写每章标题的内容了，
            //按如下格式写：首先1个字节，内容章节标题长度 * 2，接下来章节标题长度 * 2个字节，
            //内容是章节长度的Unicode编码
            var res = new ChapterTitleSection { ChapterTitle = new List<string>(chapterCount) };
            for (int i = 0; i < chapterCount; i++)
            {
                //一个字节，长度
                //长度的字节，标题
                int l = stream.ReadByte();
                stream.ReadLength(buf, l);
                res.ChapterTitle.Add(Encoding.Unicode.GetString(buf, 0, l));
            }
            return res;
        }
        public static ChapterOffsetSection ReadChapterOffset(this Stream stream, byte[] buf)
        {
            //#
            stream.ReadByte();
            //type
            stream.ReadLength(buf, 2);
            int type = BitConverter.ToInt16(buf, 0);
            if (type != (int)FileSectionType.ChapterOffset)
            {
                return null;
            }
            //读取11无用的字节
            stream.ReadLength(buf, 11);
            //接下来四个字节和章节数目有关（章节数目*4）+9
            stream.ReadLength(buf, 4);
            int chapterLength = (BitConverter.ToInt32(buf, 0) - 9) / 4;
            var res = new ChapterOffsetSection { ChapterOffset = new List<int>(chapterLength) };
            for (int i = 0; i < chapterLength; i++)
            {
                stream.ReadLength(buf, 4);
                res.ChapterOffset.Add(BitConverter.ToInt32(buf, 0));
            }
            return res;
        }
        public static FileSection ReadFileProperty(this Stream stream, byte[] buf, UmdFile file)
        {
            //#开头
            int start = stream.ReadByte();
            if (start != '#')
            {
                stream.Position = stream.Position - 1;
                return null;
            }
            //两个字节，表示类别
            stream.ReadLength(buf, 2);
            int type = BitConverter.ToInt16(buf, 0);
            if (!IsFileProperty(type))
            {
                stream.Position = stream.Position - 3;
                return null;
            }
            //一个无意义字节
            stream.ReadByte();
            //一个长度的字节
            int len = stream.ReadByte() - 5;
            stream.ReadLength(buf, len);
            var typeEnum = (MetaDataEnum)type;
            switch (typeEnum)
            {
                case MetaDataEnum.FileStart:
                    return null;
                case MetaDataEnum.Title:
                    var title = new FileTitleSection
                    {
                        Title = Encoding.Unicode.GetString(buf, 0, len)
                    };
                    file.Title = title;
                    return title;
                case MetaDataEnum.Author:
                    var author = new AuthorSection
                    {
                        Author = Encoding.Unicode.GetString(buf, 0, len)
                    };
                    file.Author = author;
                    return author;
                case MetaDataEnum.Year:
                    var year = new YearSection
                    {
                        Year = Encoding.Unicode.GetString(buf, 0, len)
                    };
                    if (file.PublishDate == null)
                    {
                        file.PublishDate = new PublishDateSection();
                    }
                    file.PublishDate.Year = year;
                    return year;
                case MetaDataEnum.Month:
                    var month = new MonthSection
                    {
                        Month = Encoding.Unicode.GetString(buf, 0, len)
                    };
                    if (file.PublishDate == null)
                    {
                        file.PublishDate = new PublishDateSection();
                    }
                    file.PublishDate.Month = month;
                    return month;
                case MetaDataEnum.Day:
                    var day = new DaySection
                    {
                        Day = Encoding.Unicode.GetString(buf, 0, len)
                    };
                    if (file.PublishDate == null)
                    {
                        file.PublishDate = new PublishDateSection();
                    }
                    file.PublishDate.Day = day;
                    return day;
                case MetaDataEnum.NovelType:
                    var novelType = new NovelTypeSection
                    {
                        NovelType = Encoding.Unicode.GetString(buf, 0, len)
                    };
                    file.NovelType = novelType;
                    return novelType;
                case MetaDataEnum.Publisher:
                    var publisher = new PublisherSection
                    {
                        Publisher = Encoding.Unicode.GetString(buf, 0, len)
                    };
                    file.Publisher = publisher;
                    return publisher;
                case MetaDataEnum.Saler:
                    var saler = new SalerSection
                    {
                        Saler = Encoding.Unicode.GetString(buf, 0, len)
                    };
                    file.Saler = saler;
                    return saler;
                case MetaDataEnum.ContentLength:
                    var contentLength = new ContentLengthSection
                    {
                        ContentLength = BitConverter.ToInt32(buf, 0)
                    };
                    file.ContentLength = contentLength;
                    return contentLength;
                default:
                    return null;
            }
        }

        private static byte[] _propertyTypes = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0b, };
        private static bool IsFileProperty(int type)
        {
            return _propertyTypes.Any(m => m == type);
        }

        public static void ReadLength(this Stream stream, byte[] buf, int length)
        {
            if (buf == null || buf.Length < length)
            {
                throw new ArgumentException("参数不合法", "buf");
            }
            if (length > stream.Length - stream.Position)
            {
                throw new ArgumentException("参数不合法", "length");
            }
            int readlen = 0;
            int readSum = 0;
            do
            {
                readlen = stream.Read(buf, readSum, length - readSum);
                readSum += readlen;
            } while (readlen < length);
        }
    }

}
