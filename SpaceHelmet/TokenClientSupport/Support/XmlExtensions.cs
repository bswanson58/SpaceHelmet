using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace TokenClientSupport.Support {
    public static class Extensions {
        public static T ? Deserialize<T>( this string value ) {
            var xmlSerializer = new XmlSerializer(typeof(T));

            return (T ?)xmlSerializer.Deserialize( new StringReader( value ));
        }

        public static string Serialize<T>( this T value ) {
            if( value == null )
                return string.Empty;

            var xmlSerializer = new XmlSerializer(typeof(T));

            using( var stringWriter = new StringWriter() ) {
                using( var xmlWriter = XmlWriter.Create( stringWriter )) {
                    xmlSerializer.Serialize( xmlWriter, value );

                    return stringWriter.ToString();
                }
            }
        }
    }
}
