using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace FirstOne
{
    public class Methods
    {
        public JToken getData()
        {
            StreamReader re = new StreamReader("C:\\Users\\Hp\\Videos\\FirstOne\\FirstOne\\Animal.json");
            JsonTextReader reader = new JsonTextReader(re);
            JsonSerializer se = new JsonSerializer();
            JToken parsedData = (JToken)se.Deserialize(reader);
            
            return parsedData;
            
        }
        public JArray getValues()
        {
            StreamReader re = new StreamReader("C:\\Users\\Hp\\Videos\\FirstOne\\FirstOne\\Animal.json");
            JsonTextReader reader = new JsonTextReader(re);
            JsonSerializer se = new JsonSerializer();
            JArray parsedData = (JArray)se.Deserialize(reader);

            return parsedData;

        }
        public JToken getveg()
        {
            StreamReader re = new StreamReader("C:\\Users\\Hp\\Videos\\FirstOne\\FirstOne\\FruitsAndVegetables.json");
            JsonTextReader reader = new JsonTextReader(re);
            JsonSerializer se = new JsonSerializer();
            JToken parsedData = (JToken)se.Deserialize(reader);

            return parsedData;

        }
        public JArray getValuesofveg()
        {
            StreamReader re = new StreamReader("C:\\Users\\Hp\\Videos\\FirstOne\\FirstOne\\FruitsAndVegetables.json");
            JsonTextReader reader = new JsonTextReader(re);
            JsonSerializer se = new JsonSerializer();
            JArray parsedData = (JArray)se.Deserialize(reader);

            return parsedData;

        }
    }
}