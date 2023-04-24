using OpenTK.Audio.OpenAL;

namespace CyphEngine.Audio;

public class AudioManager : IDisposable
{
	private ALDevice _audioDevice;
	private ALContext _audioContext;

	internal AudioManager()
	{
		_audioDevice = ALC.OpenDevice(null);
		_audioContext = ALC.CreateContext(_audioDevice, new ALContextAttributes());
		ALC.MakeContextCurrent(_audioContext);
	}

	public void Dispose()
	{
		ALC.MakeContextCurrent(ALContext.Null);
		ALC.DestroyContext(_audioContext);
		ALC.CloseDevice(_audioDevice);
	}
}