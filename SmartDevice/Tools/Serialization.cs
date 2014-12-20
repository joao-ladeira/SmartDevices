using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SmartDevice.Tools
{
    public class CustomEncodingStringWriter : StringWriter
    {
        Encoding m_encoding;
        public CustomEncodingStringWriter(Encoding encoding)
        {
            m_encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return m_encoding; }
        }
    }

    public class DataSerializer
    {
        public static string SerializeObject(object oRootData)
        {
            string result = null;

            try
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(oRootData.GetType());

                using (StringWriter stringWriter = new CustomEncodingStringWriter(Encoding.UTF8))
                {
                    XmlWriterSettings xmlSettings = new XmlWriterSettings();
                    xmlSettings.Indent = true;
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlSettings))
                    {
                        dcSerializer.WriteObject(xmlWriter, oRootData);
                    }
                    result = stringWriter.ToString();
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public static T DeserializeObject<T>(string sXml)
        {
            T result = default(T);

            try
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(T));

                using (StringReader stringReader = new StringReader(sXml))
                {
                    XmlReaderSettings xmlSettings = new XmlReaderSettings();
                    xmlSettings.IgnoreComments = true;
                    xmlSettings.IgnoreWhitespace = true;
                    using (XmlReader xmlReader = XmlReader.Create(stringReader, xmlSettings))
                    {
                        result = (T)dcSerializer.ReadObject(xmlReader);
                    }
                }
            }
            catch
            {
                result = default(T);
            }

            return result;
        }
    }

    public class Serializer
    {
        public static string SerializeObject(object oRootData)
        {
            string result = null;

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(oRootData.GetType());

                using (MemoryStream ms = new MemoryStream())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, Encoding.UTF8))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlSerializer.Serialize(xmlWriter, oRootData);

                        ms.Seek(0, SeekOrigin.Begin);
                        UTF8Encoding oUTF8Encoding = new UTF8Encoding();
                        result = oUTF8Encoding.GetString(ms.ToArray());
                    }
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public static string SerializeObject(object oRootData, Encoding encoding)
        {
            string result = null;

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(oRootData.GetType());

                using (MemoryStream ms = new MemoryStream())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, encoding))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlSerializer.Serialize(xmlWriter, oRootData);

                        ms.Seek(0, SeekOrigin.Begin);
                        UTF8Encoding oUTF8Encoding = new UTF8Encoding();
                        result = oUTF8Encoding.GetString(ms.ToArray());
                    }
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public static T DeserializeObject<T>(string sXml)
        {
            T result = default(T);

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                UTF8Encoding encoding = new UTF8Encoding();
                using (MemoryStream ms = new MemoryStream(encoding.GetBytes(sXml)))
                {
                    result = (T)xmlSerializer.Deserialize(ms);
                }
            }
            catch
            {
                result = default(T);
            }

            return result;
        }

        public static T DeserializeObject<T>(string sXml, Encoding encoding)
        {
            T result = default(T);

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                using (MemoryStream ms = new MemoryStream(encoding.GetBytes(sXml)))
                {
                    result = (T)xmlSerializer.Deserialize(ms);
                }
            }
            catch
            {
                result = default(T);
            }

            return result;
        }

    }
}
