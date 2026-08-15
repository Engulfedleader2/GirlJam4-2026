extends AnimationPlayer

var index = 0

func _ready():
	index += 1
	print(index)
	if $"../Circle2".visible == true:
		play("Transistion_out")

func _play():
	if index == 1:
		play("Transistion_in")
