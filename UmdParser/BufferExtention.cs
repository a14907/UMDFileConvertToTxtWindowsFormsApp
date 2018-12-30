using System.IO;
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

        public static string ZLibBufferToString(this byte[] buf, int len)
        {
            var strm = new zlib.ZStream();
            strm.next_in = buf;
            strm.avail_in = len;
            if (strm.inflateInit() != zlib.zlibConst.Z_OK)
            {
                return null;
            }
            bool done = false;
            byte[] decompressed = new byte[len * 2];
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

                using (var ms = new MemoryStream(decompressed, 0, (int)strm.total_out))
                {
                    using (StreamReader reader = new StreamReader(ms, Encoding.Unicode))
                    {
                        var res = reader.ReadToEnd();
                        return res;
                    }
                }
            }
            else return null;

        }
    }

}
