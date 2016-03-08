using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace Flexinets.Radius
{
    public class RadiusDictionary
    {
        private Dictionary<Byte, DictionaryAttribute> _attributes;
        public Dictionary<Byte, DictionaryAttribute> Attributes
        {
            get
            {
                return _attributes;
            }
        }

        private List<DictionaryVendorAttribute> _vendorAttributes;
        public List<DictionaryVendorAttribute> VendorAttributes
        {
            get
            {
                return _vendorAttributes;
            }
        }

        private readonly ILog _log = LogManager.GetLogger(typeof(RadiusDictionary));

        /// <summary>
        /// Load the dictionary, damn side effects...
        /// </summary>        
        public RadiusDictionary(String dictionaryPath)
        {
            _attributes = new Dictionary<Byte, DictionaryAttribute>();
            _vendorAttributes = new List<DictionaryVendorAttribute>();

            using (var sr = new StreamReader(dictionaryPath))
            {
                var attributeCount = 0;
                var vsaCount = 0;

                while (sr.Peek() >= 0)
                {
                    var line = sr.ReadLine();
                    if (line.StartsWith("ATTRIBUTE"))
                    {
                        var lineparts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var key = Convert.ToByte(lineparts[2]);

                        // If duplicates are encountered, the last one will prevail
                        // Same behaviour as Radiator
                        if (_attributes.ContainsKey(key))
                        {
                            _attributes.Remove(key);
                        }
                        _attributes.Add(key, new DictionaryAttribute(key, lineparts[1], lineparts[3]));
                        attributeCount++;
                    }

                    if (line.StartsWith("VENDORATTR"))
                    {
                        var lineparts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        _vendorAttributes.Add(new DictionaryVendorAttribute(
                            Convert.ToUInt32(lineparts[1]),
                            Convert.ToUInt32(lineparts[3]),
                            lineparts[2],
                            lineparts[4]));
                        
                        vsaCount++;
                    }                    
                }

                _log.InfoFormat("Parsed {0} attributes and {1} vendor attributes from file", attributeCount, vsaCount);
            }
        }
    }
}
