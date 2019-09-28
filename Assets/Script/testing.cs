using UnityEngine;

public class testing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FileObject file = FileReader.ReadFile("C:\\Users\\Prathamesh\\Desktop\\test\\datafile");
        byte[] bytesArray = DataParser.ObjecttoByteArray<FileObject>(file);
        Debug.Log(bytesArray);
        FileObject obj1 = DataParser.DeserializeObject<FileObject>(bytesArray);
        if (obj1 != null)
        {
            Debug.Log("Success");
            for (int i = 0; i < obj1.names.Length; i++)
            {
                Debug.Log(obj1.names[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
