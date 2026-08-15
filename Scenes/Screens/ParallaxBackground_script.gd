extends ParallaxBackground

# Speed of the scrolling background
export (Vector2) var scroll_speed = Vector2(-25, 0)

func _process(delta):
	# Continuously increase the scroll offset
	scroll_offset += scroll_speed * delta
