using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

public class JSONSerialization
{
    public static string WriteFromObject(object obj, Type objectType)
    {
        // Create a stream to serialize the object to.
        using (MemoryStream ms = new MemoryStream())
        {
            // Serializer the User object to the stream.
            DataContractJsonSerializer ser = new DataContractJsonSerializer(objectType);
            ser.WriteObject(ms, obj);
            byte[] json = ms.ToArray();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }
    }

    public static object ReadToObject(string json, Type objectType)
    {
        object result = null;
        // Create a stream to deserialize the object from.
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        {
            try
            {
                // Deserialize the JSON string to an object of the specified type.
                DataContractJsonSerializer ser = new DataContractJsonSerializer(objectType);
                result = ser.ReadObject(ms);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        return result;
    }

}
