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
        UmdFile Parse(Stream stream);
    }

    public class UmdFile
    {
        public ChapterOffsetSection ChapterOffset { get; internal set; }
        public ChapterTitleSection ChapterTitle { get; internal set; }
        public ContentSection Content { get; internal set; }
        public CoverSection Cover { get; internal set; }

        public FileTitleSection Title { get; internal set; }
        public AuthorSection Author { get; internal set; }
        public PublishDateSection PublishDate { get; internal set; }
        public NovelTypeSection NovelType { get; internal set; }
        public PublisherSection Publisher { get; internal set; }
        public SalerSection Saler { get; internal set; }
        public ContentLengthSection ContentLength { get; internal set; }
        public PageOffsetSection PageOffset { get; internal set; }

        public IEnumerable<FileSection> GetFileMetaData()
        {
            if (Title != null)
                yield return Title;
            if (Author != null)
                yield return Author;
            if (PublishDate != null)
                yield return PublishDate;
            if (NovelType != null)
                yield return NovelType;
            if (Publisher != null)
                yield return Publisher;
            if (Saler != null)
                yield return Saler;
            if (ContentLength != null)
                yield return ContentLength;
        }

        /// <summary>
        /// 获取章节内容
        /// </summary>
        /// <param name="index">以0开头的章节数</param>
        /// <returns>章节内容</returns>
        public string GetChapterContent(int index)
        {
            if (Content.Content[index]!=null)
            {
                return Content.Content[index];
            }
            var chapterCount = ChapterOffset.ChapterOffset.Count;
            var offsetLs = ChapterOffset.ChapterOffset;
            var bufLs = Content.ContentBuffer;

            if (index + 1 > chapterCount || index < 0)
            {
                throw new ArgumentException("章节数错误");
            }

            var start = offsetLs[index];
            var end = index + 1 == chapterCount ? (int)ContentLength.ContentLength : offsetLs[index + 1];
            
            int sum = 0;
            for (int i = 0; i < Content.ContentBuffer.Count; i++)
            {
                sum += bufLs[i].Count();
                if (sum > start)
                {
                    var skipCount = bufLs.Where((item, a) => a < i).Aggregate(0, (t, item) => t + item.Count());
                    var leftSkipCount = start - skipCount;
                    var takeLen = end - start;
                    byte[] contentBuf = null;
                    if (bufLs[i].Count() - leftSkipCount >= takeLen)
                    {
                        contentBuf = bufLs[i].Skip(leftSkipCount).Take(takeLen).ToArray();
                    }
                    else
                    {
                        var len = bufLs[i].Count() - leftSkipCount;
                        var totalBuf = new List<IEnumerable<byte>>();
                        totalBuf.Add(bufLs[i].Skip(leftSkipCount).Take(len));
                        var leftlen = takeLen - len;
                        var tempi = i;
                        while (leftlen != 0)
                        {
                            var nextBufLen = bufLs[++tempi].Count();
                            if (nextBufLen >= leftlen)
                            {
                                totalBuf.Add(bufLs[tempi].Take(leftlen));
                                leftlen = 0;
                                break;
                            }
                            else
                            {
                                totalBuf.Add(bufLs[tempi]);
                                leftlen -= nextBufLen;
                            }
                        }
                        var totalLen = totalBuf.Aggregate(0, (a, b) => a + b.Count());
                        var lsTotal = new byte[totalLen];
                        int copySum = 0;
                        foreach (var item in totalBuf)
                        {
                            var iarr = item.ToArray();
                            iarr.CopyTo(lsTotal, copySum);
                            copySum += iarr.Length;
                        }
                        contentBuf = lsTotal;
                    }

                    using (var ms = new MemoryStream(contentBuf))
                    using (StreamReader sr = new StreamReader(ms, Encoding.Unicode))
                    {
                        var str = sr.ReadToEnd();
                        Content.Content[index] = str;
                        return str;
                    }
                }
            }
            return null;
        }
    }
}
