extends Node2D


var index = 1


func _ready():
	if get_node("/root/GameManager").HasSeenTutorial():
		hide()
		$Button.disabled = true
		set_process(false)
		
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if index <= 0:
		index = 1
	elif index == 1:
		page1()
	elif index == 2:
		page2()
	elif index == 3:
		page3()
	elif index == 4:
		page4()
	elif index == 5:
		page5()
	elif index == 6:
		page6()
	elif index == 7:
		page7()
	elif index == 8:
		page8()
	elif index == 9:
		page9()
	elif index >= 10:
		index = 10
		$".".hide()
		$Button.disabled = true
		get_node("/root/GameManager").MarkTutorialSeen()
		set_process(false)

func _input(event):
	if event.is_action_pressed("next"):
		index += 1
	if event. is_action_pressed("previous"):
		index -= 1

#func _on_Button_pressed():
#	index += 1


func page1():
	$"1".show()
	$"2".hide()
	$"3".hide()
	$"4".hide()
	$"5".hide()
	$"6".hide()
	$"7".hide()
	$"8".hide()
	$"9".hide()

func page2():
	$"1".hide()
	$"2".show()
	$"3".hide()
	$"4".hide()
	$"5".hide()
	$"6".hide()
	$"7".hide()
	$"8".hide()
	$"9".hide()

func page3():
	$"1".hide()
	$"2".hide()
	$"3".show()
	$"4".hide()
	$"5".hide()
	$"6".hide()
	$"7".hide()
	$"8".hide()
	$"9".hide()

func page4():
	$"1".hide()
	$"2".hide()
	$"3".hide()
	$"4".show()
	$"5".hide()
	$"6".hide()
	$"7".hide()
	$"8".hide()
	$"9".hide()

func page5():
	$"1".hide()
	$"2".hide()
	$"3".hide()
	$"4".hide()
	$"5".show()
	$"6".hide()
	$"7".hide()
	$"8".hide()
	$"9".hide()

func page6():
	$"1".hide()
	$"2".hide()
	$"3".hide()
	$"4".hide()
	$"5".hide()
	$"6".show()
	$"7".hide()
	$"8".hide()
	$"9".hide()

func page7():
	$"1".hide()
	$"2".hide()
	$"3".hide()
	$"4".hide()
	$"5".hide()
	$"6".hide()
	$"7".show()
	$"8".hide()
	$"9".hide()

func page8():
	$"1".hide()
	$"2".hide()
	$"3".hide()
	$"4".hide()
	$"5".hide()
	$"6".hide()
	$"7".hide()
	$"8".show()
	$"9".hide()

func page9():
	$"1".hide()
	$"2".hide()
	$"3".hide()
	$"4".hide()
	$"5".hide()
	$"6".hide()
	$"7".hide()
	$"8".hide()
	$"9".show()
