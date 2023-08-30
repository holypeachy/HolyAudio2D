using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class HolyAudioSaver
{
	private static string filePath = "/AudioSettings.holy";
	
	public static void SaveData(HolyAudioData Data)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + filePath;
		
		// * Enable this if you want to see the full path of the file
		// Debug.Log("HolyAudioSaver|SavePath: \"" + path + "\"");
		
		FileStream stream = new FileStream(path, FileMode.Create);
		
		formatter.Serialize(stream, Data);
		stream.Close();
	}
	
	public static HolyAudioData LoadData()
	{
		string path = Application.persistentDataPath + filePath;
		if(File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);
			
			HolyAudioData data = formatter.Deserialize(stream) as HolyAudioData;
			stream.Close();
			return data;
		}
		else
		{
			Debug.LogError("HolyAudioSaver: Save file is not found in " + path);
			return null;
		}
	}
}
