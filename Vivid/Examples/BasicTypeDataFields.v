Player {
	Health: normal
	Speed: small
	Accuracy: byte

	Player(health, speed, accuracy) {
		Health = health
		Speed = speed
		Accuracy = accuracy
	}

	get_score() {
		=> Health * 10 + Speed * 5 + Accuracy
	}
}

init() {
	player = Player(100, 7, 999999999)
	=> player.get_score()
}