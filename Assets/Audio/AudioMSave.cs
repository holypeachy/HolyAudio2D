using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public static class AudioMSave
{
	public static void SaveData(AudioData AudioData)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/AudioSettings.bin";
		Debug.Log(path);
		FileStream stream = new FileStream(path, FileMode.Create);
		
		formatter.Serialize(stream, AudioData);
		stream.Close();
	}
	
	public static AudioData LoadData()
	{
		string path = Application.persistentDataPath + "/AudioSettings.bin";
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
			Debug.LogError("AudioMSAVE: Save file is not found in " + path);
			return null;
		}
	}
}
