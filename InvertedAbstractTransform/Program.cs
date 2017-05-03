using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;

namespace InvertedAbstractTransform
{
    class Program
    {
        static void Main(string[] args)
        {
            //Getting some papers
            string requestUrl = @"https://westus.api.cognitive.microsoft.com/academic/v1.0/evaluate?attributes=Ti,E&expr=Composite(AA.AuN%3D%3D%27kuansan%20wang%27)";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            webRequest.Headers["Ocp-Apim-Subscription-Key"]= "Your own api key!"; //Put your own api key here!
            string responseStr = (new StreamReader(webRequest.GetResponse().GetResponseStream())).ReadToEnd();
            JObject jResponse = (JObject)JsonConvert.DeserializeObject(responseStr);


            //Get extended attribute for the first paper in the response
            var jExtendedAttribute = (JObject)JsonConvert.DeserializeObject(jResponse["entities"][0]["E"].Value<string>());

            //Extract inverted index attributes for reconstruction
            var jInvertedAbstract = jExtendedAttribute["IA"];
            int indexLength = jInvertedAbstract["IndexLength"].ToObject<int>();
            Dictionary<string, int[]> invertedIndex = jInvertedAbstract["InvertedIndex"].ToObject<Dictionary<string, int[]>>();

            //Abstract should be reconstructed
            string reconstructedAbstract = ReconstructInvertedAbstract(indexLength, invertedIndex);
        }

        public static string ReconstructInvertedAbstract(int indexLength, Dictionary<string, int[]> invertedIndex)
        {
            string[] abstractStr = new string[indexLength];

            foreach(var pair in invertedIndex)
            {
                string word = pair.Key;
                foreach(var index in pair.Value)
                {
                    abstractStr[index] = word;
                }
            }

            return String.Join(" ", abstractStr);

        }

    }
}
