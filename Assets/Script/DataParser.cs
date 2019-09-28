using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class DataParser 
{
    public static byte[] ObjecttoByteArray<T>(T obj)
    {
        using (MemoryStream memStream = new MemoryStream())
        {
            BinaryFormatter binSerializer = new BinaryFormatter();
            if (obj.GetType() == typeof(FileObject))
            {
                byte[] buffer = { 0x00 };
                memStream.Write(buffer, 0, 1);
            }
            else if (obj.GetType() == typeof(SetModel)){
                byte[] buffer = { 0x01 };
                memStream.Write(buffer, 0, 1);
            }
            else if (obj.GetType() == typeof(SetPosition)){
                byte[] buffer = { 0x02 };
                memStream.Write(buffer, 0, 1);
            }
            else
            {
                Console.WriteLine("Invalid object provided.");
                return null;
            }
            binSerializer.Serialize(memStream, obj);
            return memStream.ToArray();
        }
    }

    public static T DeserializeObject<T>(byte[] serializedObj) where T:class
    {
        if (serializedObj[0] == 0x00)
        {
            if (typeof(T) != typeof(FileObject))
            {
                return null;
            }
        }
        if (serializedObj[0] == 0x01)
        {
            if (typeof(T) != typeof(SetModel))
            {
                return null;
            }
        }
        if (serializedObj[0] == 0x02)
        {
            if (typeof(T) != typeof(SetPosition))
            {
                return null;
            }
        }
        T obj;
        using (MemoryStream memStream = new MemoryStream())
        {
            BinaryFormatter binSerializer = new BinaryFormatter();
            memStream.Write(serializedObj, 1, serializedObj.Length-1);
            memStream.Seek(0, SeekOrigin.Begin);
            obj = (T)binSerializer.Deserialize(memStream);
        }
        return obj;
    }

}
