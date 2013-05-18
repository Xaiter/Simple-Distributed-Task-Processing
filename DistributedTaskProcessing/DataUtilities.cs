using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DistributedTaskProcessing
{
    /// <summary>
    /// Provides common binary data manipulation functionality.
    /// </summary>
    public static class DataUtilities
    {
        // Public Methods
        public static byte[] Serialize(object value)
        {
            var formatter = new BinaryFormatter();
            var ms = new MemoryStream();

            formatter.Serialize(ms, value);
            return ms.ToArray();
        }

        public static T Deserialize<T>(byte[] data)
        {
            var formatter = new BinaryFormatter();
            var ms = new MemoryStream(data);
            return (T)formatter.Deserialize(ms);
        }

        public static byte[] Compress(byte[] bytes)
        {
            byte[] retValue;
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Compress, true))
                {
                    zipStream.Write(bytes, 0, bytes.Length);
                    retValue = stream.ToArray();
                    zipStream.Close();
                    stream.Close();
                }
            }
            return retValue;
        }

        public static byte[] Decompress(byte[] bytes)
        {
            byte[] retValue;
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress, true))
                {
                    byte[] buffer = new byte[4096];
                    int size;
                    while (true)
                    {
                        size = zipStream.Read(buffer, 0, buffer.Length);
                        if (size > 0)
                            stream.Write(buffer, 0, size);
                        else
                            break;
                    }
                    zipStream.Close();
                    retValue = stream.ToArray();
                    stream.Dispose();
                    zipStream.Dispose();
                    return retValue;
                }
            }
        }

        public static AssemblyName[] GetDependentAssemblies(Assembly root)
        {
            throw new NotImplementedException();
        }


        


        // Private Methods
        private static void GetDependentAssembliesRecursive(Assembly currentRoot, Dictionary<string, AssemblyName> dependencies)
        {
            foreach (var reference in currentRoot.GetReferencedAssemblies())
            {

            }
        }


        // Extension Methods
        public static Assembly GetAssembly(this AssemblyName name)
        {
            return Assembly.Load(name);
        }
    }

    public static class WcfUtilities
    {
        public static T GetServiceProxy<T>(string tcpEndpointUri)
        {
            var binding = new NetTcpBinding();
            var channelFactory = new ChannelFactory<T>(binding, tcpEndpointUri);
            return channelFactory.CreateChannel();
        }
    }
}
