extends Node2D


var can_play = false

var index = 1

var start = false

func _ready() -> void:
	$AnimationPlayer.play("Transistion_out")

func _process(delta: float) -> void:
	if can_play == true:
		_pause()
		whatScene()

func _input(event: InputEvent) -> void:
	if Input.is_action_pressed("next") and start == true:
		index += 1
		_play()


func whatScene():
	if index == 2:
		$AnimationPlayer.play("cam_2")
	if index == 3:
		$AnimationPlayer.play("cam_3")
	if index == 4:
		$AnimationPlayer.play("cam_4")
	if index == 5:
		$AnimationPlayer.play("cam_5")
	if index == 6:
		$AnimationPlayer.play("cam_6")
	if index == 7:
		$Circle.hide()
		$AnimationPlayer.play("Transistion_in")
		start = false


func _play():
	can_play = true

func _pause():
	can_play = false

func _start():
	start = true

func _startGame():
	get_tree().change_scene("res://Scenes/Screens/MainGame.tscn")
