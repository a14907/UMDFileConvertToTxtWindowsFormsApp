using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace UmdParser
{
    public static class BufferExtention
    {
        public static string BufferToString(this byte[] buf)
        {
            return Encoding.Unicode.GetString(buf);
        }

        public static byte[] ZLibBufferToString(this byte[] buf, int len)
        {
            //using (var ms = new MemoryStream(buf, 0, len))
            //using (var ds = new GZipStream(ms, CompressionMode.Decompress))
            //{
            //    byte[] decompressed = new byte[32 * 1024 + 10];
            //    int readlen = 0;
            //    int readSum = 0;
            //    do
            //    {
            //        readlen = ds.Read(decompressed, readlen, decompressed.Length - readSum);
            //        readSum += readlen;
            //    } while (readlen != 0);
            //    return decompressed.Take(readSum).ToArray();
            //}

            var strm = new zlib.ZStream();
            strm.next_in = buf;
            strm.avail_in = len;
            if (strm.inflateInit() != zlib.zlibConst.Z_OK)
            {
                return null;
            }
            bool done = false;
            byte[] decompressed = new byte[32 * 1024];
            int status = 0;

            while (!done)
            {
                // Make sure we have enough room and reset the lengths. 
                strm.next_out = decompressed;
                strm.avail_out = decompressed.Length;

                // Inflate another chunk.
                status = strm.inflate(zlib.zlibConst.Z_SYNC_FLUSH);
                if (status == zlib.zlibConst.Z_STREAM_END) done = true;
                else if (status != zlib.zlibConst.Z_OK) break;
            }
            if (strm.inflateEnd() != zlib.zlibConst.Z_OK) return null;

            // Set real length.
            if (done)
            {

                //using (var ms = new MemoryStream(decompressed, 0, (int)strm.total_out))
                //{
                //    using (StreamReader reader = new StreamReader(ms, Encoding.Unicode))
                //    {
                //        var res = reader.ReadToEnd();
                //        return res;
                //    }
                //}
                if (decompressed.Length == (int)strm.total_out)
                {
                    return decompressed;
                }
                return decompressed.Take((int)strm.total_out).ToArray();
            }
            else return null;
        }
    }

}
