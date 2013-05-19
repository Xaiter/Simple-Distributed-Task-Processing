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
using System.Xml.Serialization;

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
            var serializer = new XmlSerializer(value.GetType());            
            var ms = new MemoryStream();

            serializer.Serialize(ms, value);
            
            var data = ms.ToArray();
            File.WriteAllBytes("C:\\test\\Serialize.xml", data);
            return data;
        }

        public static object Deserialize(byte[] data, Type type)
        {
            var serializer = new XmlSerializer(type);
            var ms = new MemoryStream(data);
            File.WriteAllBytes("C:\\test\\Deserialize.xml", data);
            return serializer.Deserialize(ms);
        }

        public static T Deserialize<T>(byte[] data)
        {
            return (T)Deserialize(data, typeof(T));
        }

        public static byte[] Compress(byte[] bytes)
        {
            byte[] returnValue;

            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Compress, true))
                {
                    zipStream.Write(bytes, 0, bytes.Length);
                    zipStream.Close();
                }

                returnValue = stream.ToArray();
                stream.Close();
            }

            return returnValue;
        }

        public static byte[] Decompress(byte[] bytes)
        {
            
            using (MemoryStream stream = new MemoryStream())
            {
                var ms = new MemoryStream(bytes);
                using (GZipStream zipStream = new GZipStream(ms, CompressionMode.Decompress, true))
                {
                    byte[] buffer = new byte[8192];
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
                    byte[] retValue = stream.ToArray();
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
            var channelFactory = new ChannelFactory<T>(GetTcpBinding(), tcpEndpointUri);
            return channelFactory.CreateChannel();
        }

        public static NetTcpBinding GetTcpBinding()
        {
            var binding = new NetTcpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 134217728;
            return binding;
        }
    }
}
