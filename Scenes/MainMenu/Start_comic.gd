extends Button

var start_game_sfx = preload("res://Assets/Audio/SFX/UI/SFX_UI_Start_Game.wav")


func _on_Start_comic_pressed():
	get_node("/root/AudioManager").PlaySFX(start_game_sfx)
	$"../AnimationPlayer".play("Transistion_in")

func _startComic():
	get_tree().change_scene("res://Scenes/Comic/ComicPanel.tscn")
