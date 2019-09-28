using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileReader 
{
   public static FileObject ReadFile(string path) {
        string[] text = File.ReadAllLines(path);
        if (text.Length == 0)
        {
            Debug.Log("Data file is empty.");
            return null;
        }
        FileObject fileObject = new FileObject();
        fileObject.url = text[0];
        fileObject.names = new string[text.Length - 1];
        for (int i = 0; i < fileObject.names.Length; i++)
        {
            fileObject.names[i] = text[i+1];
        }
        return fileObject;
    }
}
