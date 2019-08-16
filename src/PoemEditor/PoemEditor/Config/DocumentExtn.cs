//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PoemEditor.Config
//{
//    public static class DocumentExtn
//    {
//        public static int ToInt(this string data)
//        {
//            int i = int.MinValue;
//            int.TryParse(data, out i);
//            return i;
//        }
       
//        public static T Load<T>(this string json)
//        {
//            T doc = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
//            return doc;
//        }
//        public static T Read<T>(this string fileName)
//        {
//            string json = File.ReadAllText(fileName);
//            T doc = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
//            return doc;
//        }
//        public static IJSDocument Write(this IJSDocument doc, string fileName)
//        {
//            string json = Newtonsoft.Json.JsonConvert.SerializeObject(doc);
//            File.WriteAllText(fileName, json);
//            return doc;
//        }
//        public static object Save(this object doc, string fileName)
//        {
//            string json = Newtonsoft.Json.JsonConvert.SerializeObject(doc);
//            File.WriteAllText(fileName, json);
//            return doc;
//        }
//    }
//}
