extends AnimationPlayer

var index = 0

func _ready():
	if $"../Circle2".visible == true:
		play("Transistion_out")

func _play():
	if index == 1:
		play("Transistion_in")
