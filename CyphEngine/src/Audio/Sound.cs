using NAudio.Wave;
using OpenTK.Audio.OpenAL;

namespace CyphEngine.Audio;

public class Sound : IDisposable
{
	private int _handle;
	public int Handle => _handle;
	
	internal Sound(int channels, int sampleRate, int bitsPerSample, byte[] data)
	{
		AL.GenBuffer(out _handle);

		ALFormat format;
		if (channels == 1 && bitsPerSample == 8)
		{
			format = ALFormat.Mono8;
		}
		else if (channels == 1 && bitsPerSample == 16)
		{
			format = ALFormat.Mono16;
		}
		else if (channels == 2 && bitsPerSample == 8)
		{
			format = ALFormat.Stereo8;
		}
		else if (channels == 2 && bitsPerSample == 16)
		{
			format = ALFormat.Stereo16;
		}
		else
		{
			throw new InvalidOperationException();
		}
		
		AL.BufferData(_handle, format, data, sampleRate);
	}

	internal static Sound? FromFile(string filePath)
	{
		if (Path.GetExtension(filePath).Equals(".wav", StringComparison.CurrentCultureIgnoreCase))
		{
			WaveFileReader reader = new WaveFileReader(filePath);
			int channels = reader.WaveFormat.Channels;
			int sampleRate = reader.WaveFormat.SampleRate;
			int bitsPerSample = reader.WaveFormat.BitsPerSample;
			using MemoryStream ms = new MemoryStream();
			reader.CopyTo(ms);
			byte[] data = ms.ToArray();

			return new Sound(channels, sampleRate, bitsPerSample, data);
		}
		else if (Path.GetExtension(filePath).Equals(".mp3", StringComparison.CurrentCultureIgnoreCase))
		{
			Mp3FileReaderBase reader = new Mp3FileReaderBase(filePath, format => new AcmMp3FrameDecompressor(format));
			int channels = reader.WaveFormat.Channels;
			int sampleRate = reader.WaveFormat.SampleRate;
			int bitsPerSample = reader.WaveFormat.BitsPerSample;
			using MemoryStream ms = new MemoryStream();
			reader.CopyTo(ms);
			byte[] data = ms.ToArray();

			return new Sound(channels, sampleRate, bitsPerSample, data);
		}

		return null;
	}
	
	public void Dispose()
	{
		AL.DeleteBuffer(_handle);
	}
}