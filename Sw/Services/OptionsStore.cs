using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeStack.Community.StockFit.Sw.Services
{
    public class OptionsStore
    {
        private readonly string m_Location;
        private readonly JsonSerializer m_Serializer;

        public OptionsStore()
        {
            m_Location = Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData),
                        "CodeStack\\StockMaster");

            m_Serializer = new JsonSerializer()
            {
                Formatting = Formatting.Indented
            };
        }

        public string Location
        {
            get
            {
                if (!Directory.Exists(m_Location))
                {
                    Directory.CreateDirectory(m_Location);
                }

                return m_Location;
            }
        }

        private string GetOptionsFilePath<TOpts>()
        {
            var filePath = Path.Combine($"{Location}\\{typeof(TOpts).Name}.json");
            return filePath;
        }

        public void Save<TOpts>(TOpts options)
            where TOpts : new()
        {
            try
            {
                using (var file = File.CreateText(GetOptionsFilePath<TOpts>()))
                {
                    m_Serializer.Serialize(file, options);
                }
            }
            catch
            {
            }
        }

        public TOpts Load<TOpts>()
            where TOpts : new()
        {
            var filePath = GetOptionsFilePath<TOpts>();

            if (File.Exists(filePath))
            {
                try
                {
                    using (var file = File.OpenText(filePath))
                    {
                        return (TOpts)m_Serializer.Deserialize(file, typeof(TOpts));
                    }
                }
                catch
                {
                }
            }

            return new TOpts();
        }
    }
}
