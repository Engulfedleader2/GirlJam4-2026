using Godot;
using System;

public class AudioManager : Node
{
	public static AudioManager Instance { get; private set; }

	private AudioStreamPlayer musicPlayer;
	private AudioStreamPlayer sfxPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;

		musicPlayer = new AudioStreamPlayer { Name = "MusicPlayer" };
		AddChild(musicPlayer);

		sfxPlayer = new AudioStreamPlayer { Name = "SFXPlayer" };
		AddChild(sfxPlayer);
	}

	public void PlayMusic(AudioStream music)
	{
		if (music == null)
		{
			return;
		}

		// Keeps track playing if no new audio call is detected
		if (musicPlayer.Stream == music && musicPlayer.Playing)
		{
			return;
		}

		musicPlayer.Stream = music;
		musicPlayer.Play();
	}
	public void StopMusic()
	{
		musicPlayer.Stop();
	}

	public void PlaySFX(AudioStream sfx)
	{
		if (sfx == null)
		{
			return;
		}

		sfxPlayer.Stream = sfx;
		sfxPlayer.Play();
	}

	public void SetMusicVolume(float volumeDb)
	{
		musicPlayer.VolumeDb = volumeDb;
	}

	public void SetSFXVolume(float volumeDb)
	{
		 sfxPlayer.VolumeDb = volumeDb;
	}
}
