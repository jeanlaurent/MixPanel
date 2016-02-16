using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MixPanel
{
    class Program
    {
        static void Main(string[] args)
        {
            MixPanel mixPanel = new MixPanel("02d793d05a0e60cd3febce7de3373492");
            mixPanel.track("This is the first event");
            mixPanel.track("Second one");
            mixPanel.track("Here is the third one");
            mixPanel.track("FOURTH");
        }

    }

    public class MixPanel
    {
        private string token;
        private DataContractJsonSerializer serializer;
        
        public MixPanel(string token)
        {
            this.token = token;
            this.serializer = new DataContractJsonSerializer(typeof(Message));
        }

        public Boolean track(string text)
        {
            return send(new Message(this.token, text));
        }

        private Boolean send(Message message)
        {
            string base64Output = toBase64(toJson(message));
            //System.Diagnostics.Debug.WriteLine(base64Output);
            WebRequest request = WebRequest.Create("http://api.mixpanel.com/track/?data=" + base64Output);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            return response.StatusCode == HttpStatusCode.OK;
        }


        private string toJson(Message message)
        { 
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, message);
            stream.Position = 0;
            return new StreamReader(stream).ReadToEnd();
        }

        private string toBase64(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }


    [DataContract]
    public class Message
    {
        [DataMember(Name = "event")]
        public String mEvent;
        [DataMember]
        public Properties properties;

        public Message(string token, string message)
        {
            this.properties = new Properties(token);
            this.mEvent = message;
        }
    }


    [DataContract]
    public class Properties
    {
        [DataMember]
        public String token;
        [DataMember(EmitDefaultValue = false)]
        public String distinct_id;

        public Properties(string token)
        {
            this.token = token;
        }
    }

}
