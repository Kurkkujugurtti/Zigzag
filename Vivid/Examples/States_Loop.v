f(a, b) {
	a = 1 + b
	b = 2 + a

	loop() {
		a += 1 + b
		b = a * 2
	}

	c = a + b
	=> c
}

init() {
	f(1, 2)
}