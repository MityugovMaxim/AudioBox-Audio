using System;
using System.Runtime.InteropServices;
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

		[DllImport("__Internal")]
		static extern float AudioManager_GetInputLatency();

		[DllImport("__Internal")]
		static extern float AudioManager_GetOutputLatency();

		[DllImport("__Internal")]
		static extern void AudioManager_RegisterRemoteCommands(
			RemoteCommandHandler _PlayHandler,
			RemoteCommandHandler _PauseHandler,
			RemoteCommandHandler _NextTrackHandler,
			RemoteCommandHandler _PreviousTrackHandler,
			RemoteCommandHandler _SourceChangedHandler
		);

		[DllImport("_Internal")]
		static extern void AudioManager_UnregisterRemoteCommands();

		[DllImport("__Internal")]
		static extern void AudioManager_EnableAudio();

		[DllImport("__Internal")]
		static extern void AudioManager_DisableAudio();

		[DllImport("__Internal")]
		static extern string AudioManager_GetOutputName();

		[DllImport("__Internal")]
		static extern string AudioManager_GetOutputUID();

		[DllImport("__Internal")]
		static extern int AudioManager_GetOutputType();

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
			AudioManager_RegisterRemoteCommands(
				PlayHandler,
				PauseHandler,
				NextTrackHandler,
				PreviousTrackHandler,
				SourceChangedHandler
			);
			
			m_HardwareLatency = GetHardwareLatency();
			m_ManualLatency   = GetManualLatency();
		}

		public static void SetAudioActive(bool _Value)
		{
			if (_Value)
				AudioManager_EnableAudio();
			else
				AudioManager_DisableAudio();
		}

		public static string GetAudioOutputName()
		{
			return AudioManager_GetOutputName();
		}

		public static string GetAudioOutputUID()
		{
			return AudioManager_GetOutputUID();
		}

		public static OutputType GetAudioOutputType()
		{
			return (OutputType)AudioManager_GetOutputType();
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
			float latency = AudioManager_GetOutputLatency();
			if (latency > 0)
				Debug.LogFormat("[AudioManager] Detected {0}ms latency.", latency);
			return latency;
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