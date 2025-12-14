using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lab_9
{
    public static class Serializer
    {
        public static void Save(string path, Element element)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, element);
            }
        }

        public static Element Load(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var bf = new BinaryFormatter();
                return (Element)bf.Deserialize(fs);
            }
        }
    }
}