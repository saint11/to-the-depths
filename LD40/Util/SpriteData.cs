using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LD40
{
    public class SpriteData
    {
        private static Dictionary<string, Sprite> Cache;
        public static void Init()
        {
            Cache = new Dictionary<string, Sprite>();

        }

        public static void Load(string path)
        {
            var xml = new XmlDocument();
            xml.Load(path);
            CacheAnimations(xml);
        }

        private static void CacheAnimations(XmlDocument xmlDoc)
        {

            foreach (XmlNode n in xmlDoc["Sprites"])
            {
                if (n is XmlComment) continue;
                XmlElement xml = n as XmlElement;
                Sprite sprite = Sprite.Load(xml);
                Cache.Add(xml.GetAttribute("name"), sprite);
            }
        }

        public static Sprite GetAnimation(string source)
        {
            return Sprite.CopyOf(Cache[source]);
        }
    }
}