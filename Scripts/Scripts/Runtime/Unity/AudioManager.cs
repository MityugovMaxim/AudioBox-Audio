using System;
using UnityEngine;

namespace AudioBox.Audio
{
	public class AudioManager
	{
		public enum OutputType
		{
			BuiltIn    = 0,
			Headphones = 1,
			Bluetooth  = 2,
			Unknown    = 3,
		}

		delegate void RemoteCommandHandler();

		const string MANUAL_LATENCY_KEY = "MANUAL_LATENCY";

		public static event Action OnPlay;
		public static event Action OnPause;
		public static event Action OnNextTrack;
		public static event Action OnPreviousTrack;
		public static event Action OnSourceChange;

		public static float Latency => m_HardwareLatency + m_ManualLatency;

		static float m_HardwareLatency;
		static float m_ManualLatency;

		public AudioManager()
		{
			m_HardwareLatency = GetHardwareLatency();
			m_ManualLatency   = GetManualLatency();
		}

		public static void SetAudioActive(bool _Value)
		{
			AudioListener.pause = !_Value;
		}

		public static string GetAudioOutputName()
		{
			return "Default speakers";
		}

		public static string GetAudioOutputUID()
		{
			return string.Empty;
		}

		public static OutputType GetAudioOutputType()
		{
			return OutputType.BuiltIn;
		}

		public static float GetManualLatency()
		{
			string key = MANUAL_LATENCY_KEY + GetAudioOutputUID();
			
			return PlayerPrefs.GetFloat(key, 0);
		}

		public static void SetManualLatency(float _ManualLatency)
		{
			m_ManualLatency = _ManualLatency;
			
			string key = MANUAL_LATENCY_KEY + GetAudioOutputUID();
			
			PlayerPrefs.SetFloat(key, _ManualLatency);
		}

		public static float GetHardwareLatency()
		{
			AudioSettings.GetDSPBufferSize(out int length, out int count);
			
			return (float)(length * count) / AudioSettings.GetConfiguration().sampleRate;
		}

		[AOT.MonoPInvokeCallback(typeof(RemoteCommandHandler))]
		static void PlayHandler()
		{
			OnPlay?.Invoke();
		}

		[AOT.MonoPInvokeCallback(typeof(RemoteCommandHandler))]
		static void PauseHandler()
		{
			OnPause?.Invoke();
		}

		[AOT.MonoPInvokeCallback(typeof(RemoteCommandHandler))]
		static void NextTrackHandler()
		{
			OnNextTrack?.Invoke();
		}

		[AOT.MonoPInvokeCallback(typeof(RemoteCommandHandler))]
		static void PreviousTrackHandler()
		{
			OnPreviousTrack?.Invoke();
		}

		[AOT.MonoPInvokeCallback(typeof(RemoteCommandHandler))]
		static void SourceChangedHandler()
		{
			m_HardwareLatency = GetHardwareLatency();
			m_ManualLatency   = GetManualLatency();
			
			OnSourceChange?.Invoke();
		}
	}
}
