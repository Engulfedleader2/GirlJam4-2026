using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : Node
{
	public static AudioManager Instance { get; private set; }

	private AudioStreamPlayer musicPlayer;
	private AudioStreamPlayer sfxPlayer;
	private readonly Random random = new Random();

	// One AudioStreamPlayer per active music layer
	private readonly List<AudioStreamPlayer> layerPlayers = new List<AudioStreamPlayer>();
	private AudioStream[] currentLayers;

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

		StopLayeredMusic();

		musicPlayer.Stream = music;
		musicPlayer.Play();
	}
	public void StopMusic()
	{
		musicPlayer.Stop();
	}

	// Plays every layer at once.
	public void PlayLayeredMusic(AudioStream[] layers)
	{
		if (layers == null || layers.Length == 0)
		{
			return;
		}

		if (currentLayers != null && currentLayers.SequenceEqual(layers) && layerPlayers.Count > 0 && layerPlayers[0].Playing)
		{
			return;
		}

		musicPlayer.Stop();
		StopLayeredMusic();

		currentLayers = layers;

		foreach (AudioStream layer in layers)
		{
			var player = new AudioStreamPlayer { Name = "MusicLayer", Stream = layer };
			AddChild(player);
			layerPlayers.Add(player);
		}

		// All layers started together in the same frame so they stay in sync.
		foreach (AudioStreamPlayer player in layerPlayers)
		{
			player.Play();
		}
	}

	public void StopLayeredMusic()
	{
		foreach (AudioStreamPlayer player in layerPlayers)
		{
			player.QueueFree();
		}

		layerPlayers.Clear();
		currentLayers = null;
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

	// Picks one clip at random from a set of variants (e.g. Click_01-04) and plays it.
	public void PlayRandomSFX(AudioStream[] options)
	{
		if (options == null || options.Length == 0)
		{
			return;
		}

		PlaySFX(options[random.Next(options.Length)]);
	}

	public void SetMusicVolume(float volumeDb)
	{
		musicPlayer.VolumeDb = volumeDb;

		foreach (AudioStreamPlayer player in layerPlayers)
		{
			player.VolumeDb = volumeDb;
		}
	}

	public void SetSFXVolume(float volumeDb)
	{
		 sfxPlayer.VolumeDb = volumeDb;
	}
}
