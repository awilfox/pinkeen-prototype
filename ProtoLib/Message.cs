using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCPProject.ProtoLib
{
    /// <summary>
    /// Defines a DCP message.
    /// </summary>
    public class Message
    {
        private string source;
        private string destination;
        private string command;
        private Dictionary <string, List<string> > param;

        /// <summary>
        /// The entity the message is being sent from.
        /// </summary>
        public string Source
        {
            get { return source; }
        }

        /// <summary>
        /// The entity the message is being sent to.
        /// </summary>
        public string Destination
        {
            get { return destination; }
        }

        /// <summary>
        /// The command the message represents.
        /// </summary>
        public string Command
        {
            get { return command; }
        }

        /// <summary>
        /// Retreives the values for a specified key.
        /// </summary>
        /// <param name="key">The key to retrieve values for.</param>
        /// <returns>An array of strings with the values for that key.</returns>
        public string[] ValueListForKey(string key)
        {
            return param[key].ToArray();
        }

        /// <summary>
        /// Retrieve the first value for the specified key.
        /// </summary>
        /// <param name="key">The key to retrieve the first value of.</param>
        /// <returns>The first value of the specified key.</returns>
        public string FirstValueForKey(string key)
        {
            return param[key][0];
        }

        /// <summary>
        /// Determines if the specified key has been specified in this message.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>true if the key is present; false otherwise.</returns>
        public bool HasValueForKey(string key)
        {
            return param != null && param.ContainsKey(key);
        }

        /// <summary>
        /// Create a new message.
        /// </summary>
        /// <param name="dst">The destination of the message.</param>
        /// <param name="cmd">The command of the message.</param>
        /// <param name="p">The keys/values of any parameters.</param>
        public Message(string dst, string cmd,
                       IDictionary< string, List<string> > p)
            : this("*", dst, cmd, p)
        {
        }

        /// <summary>
        /// Return a UTF-8 string from a null-terminated slice of a byte array.
        /// </summary>
        /// <param name="b">The byte array.</param>
        /// <param name="start">The position of the byte array the string starts at.</param>
        /// <param name="length">Will contain the length of the string in bytes (NOT characters).</param>
        /// <returns>The UTF-8 string.</returns>
        private static string StringFromByteSlice(byte[] b, int start, out int length)
        {
            byte[] next = b.Skip(start).TakeWhile((x) => (x != 0)).ToArray();
            length = next.Count();
            return Encoding.UTF8.GetString(next, 0, length);
        }

        /// <summary>
        /// Create a Message object from a raw DCP message frame.
        /// </summary>
        /// <param name="bytes">The byte sequence containing a DCP message frame.</param>
        /// <returns>A Message object loaded from the frame.</returns>
        public static Message FromBytes(IEnumerable<Byte> bytes)
        {
            string src, dst, cmd;
            short len;
            int curr = 2;
            byte[] friendlyBytes = bytes.ToArray();

            /*
             * The Windows Store API does not allow access to
             * System.Net.IPAddress, so we can't use ntohs().
             * Instead, we have to do it ourselves.
             * I'm glad they at least let us check endianness...
             */
            byte[] lenbytes = friendlyBytes.Take(2).ToArray();
            if(BitConverter.IsLittleEndian) { lenbytes = lenbytes.Reverse().ToArray(); }

            len = BitConverter.ToInt16(lenbytes, 0);

            if(len > Protocol.MAX_FRAME_LEN || len != bytes.Count())
            {
#if DEBUG
                throw new ArgumentOutOfRangeException("Length", String.Format("Invalid length of {0} received.", len));
#endif
                return null;
            }

            int size;
            src = StringFromByteSlice(friendlyBytes, curr, out size);
            curr += size + 1;

            dst = StringFromByteSlice(friendlyBytes, curr, out size);
            curr += size + 1;

            cmd = StringFromByteSlice(friendlyBytes, curr, out size);
            curr += size + 1;

            return new Message(src, dst, cmd, null);
        }

        /// <summary>
        /// Serialise the current Message to a byte representation.
        /// </summary>
        /// <returns>A Byte array containing the raw DCP frame representation of this message.</returns>
        public byte[] ToBytes()
        {
            List<string> strs = new List<string>();

            strs.Add(source);
            strs.Add(destination);
            strs.Add(command);
            
            foreach(string key in param.Keys)
            {
                foreach(string value in param[key])
                {
                    strs.Add(key);
                    strs.Add(value);
                }
            }

            List<byte> result = new List<byte>();
            // length + \0.
            result.Add(0); result.Add(0); result.Add(0);
            foreach (string s in strs)
            {
                result.AddRange(Encoding.UTF8.GetBytes(s));
                result.Add(0);
            }

            short length = (short)result.Count;
            if (length > 1600) return null; // too long

            byte[] lenbytes = BitConverter.GetBytes(length);
            if (BitConverter.IsLittleEndian) lenbytes = lenbytes.Reverse().ToArray();

            byte[] ret = result.ToArray();
            ret[0] = lenbytes[0];
            ret[1] = lenbytes[1];

            return ret;
        }

        /// <summary>
        /// Create a new message with a custom source.
        /// </summary>
        /// <param name="src">The source of the message.</param>
        /// <param name="dst">The destination of the message.</param>
        /// <param name="cmd">The command of the message.</param>
        /// <param name="p">The keys/values of any parameters.</param>
        internal Message(string src, string dst, string cmd,
                         IDictionary<string, List<string>> p)
        {
            source = src;
            destination = dst;
            command = cmd;
            param = new Dictionary<string, List<string>>(p);
        }
    }
}
