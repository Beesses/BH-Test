using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public static class Data
{
    private static string _path = "Data/";


    private static T Load<T>(string resourcesPath) where T : Object =>
            Resources.Load<T>(Path.ChangeExtension(resourcesPath, null));



    public static T GetModelData<T>() where T : SOBase
    {
        return Resources.Load<T>(_path + typeof(T).ToString());
    }

    public static T[] GetAllModelData<T>() where T: SOBase
    {
        return Resources.LoadAll<T>(_path);
    }

    public static T GetDataByString<T>(string dataName) where T: SOBase
    {
        return Resources.Load<T>(_path + dataName);
    }
}
