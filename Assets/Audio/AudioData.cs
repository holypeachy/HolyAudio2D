[System.Serializable]
public class AudioData
{
	// public float[] Volumes{set; get;}

	public string AudioVersion {get; set;}
	public float[] MasterVolumes { get; set; }
	public float[] GroupVolumes { get; set; }

}