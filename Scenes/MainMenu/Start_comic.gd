extends Button




func _on_Start_comic_pressed():
	$"../AnimationPlayer".play("Transistion_in")

func _startComic():
	get_tree().change_scene("res://Scenes/Comic/ComicPanel.tscn")
