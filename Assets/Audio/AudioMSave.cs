using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class AudioMSave
{
	private static string filePath = "/AudioSettings.bin";
	
	public static void SaveData(AudioData Data)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + filePath;
		
		Debug.Log("AudioMSave|SavePath: \"" + path + "\"");
		
		FileStream stream = new FileStream(path, FileMode.Create);
		
		formatter.Serialize(stream, Data);
		stream.Close();
	}
	
	public static AudioData LoadData()
	{
		string path = Application.persistentDataPath + filePath;
		if(File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);
			
			AudioData data = formatter.Deserialize(stream) as AudioData;
			stream.Close();
			return data;
		}
		else
		{
			Debug.LogError("AudioMSave: Save file is not found in " + path);
			return null;
		}
	}
}
