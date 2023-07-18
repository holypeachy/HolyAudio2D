[System.Serializable]
public class HolyAudioData
{
	public string AudioVersion {get; set;}
	
	public float[] MasterVolumes { get; set; }
	public float[] GroupVolumes { get; set; }//fuckyou

}