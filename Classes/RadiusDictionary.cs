using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System.Text.RegularExpressions;

namespace Flexinets.Radius
{
    public class RadiusDictionary
    {
        public Dictionary<Byte, DictionaryAttribute> Attributes { get; private set; } = new Dictionary<Byte, DictionaryAttribute>();
        public List<DictionaryVendorAttribute> VendorAttributes { get; private set; } = new List<DictionaryVendorAttribute>();
        private readonly ILog _log = LogManager.GetLogger(typeof(RadiusDictionary));

        /// <summary>
        /// Load the dictionary, damn side effects...
        /// </summary>        
        public RadiusDictionary(String dictionaryPath)
        {
            using (var sr = new StreamReader(dictionaryPath))
            {
                while (sr.Peek() >= 0)
                {
                    var line = sr.ReadLine();
                    if (line.StartsWith("ATTRIBUTE"))
                    {
                        var lineparts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var key = Convert.ToByte(lineparts[2]);

                        // If duplicates are encountered, the last one will prevail
                        // Same behaviour as Radiator
                        if (Attributes.ContainsKey(key))
                        {
                            Attributes.Remove(key);
                        }
                        Attributes.Add(key, new DictionaryAttribute(lineparts[1], key, lineparts[3]));
                    }

                    if (line.StartsWith("VENDORATTR"))
                    {
                        var lineparts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        VendorAttributes.Add(new DictionaryVendorAttribute(
                            Convert.ToUInt32(lineparts[1]),
                            lineparts[2],
                            Convert.ToUInt32(lineparts[3]),
                            lineparts[4]));

                    }
                }

                _log.InfoFormat($"Parsed {Attributes.Count} attributes and {VendorAttributes.Count} vendor attributes from file");
            }
        }
    }
}
