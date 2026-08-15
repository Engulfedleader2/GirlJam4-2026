extends AnimationPlayer

var index = 0

func _ready():
	var gm = get_node("/root/GameManager")
	if gm.ConsumeTransitionFlag():
		play("Transistion_out")
	else:
		$"../Circle2".hide() 
	#index += 1
	#print(index)
	#if $"../Circle2".visible == true:
	#	play("Transistion_out")

func _play():
	if index == 1:
		play("Transistion_in")
