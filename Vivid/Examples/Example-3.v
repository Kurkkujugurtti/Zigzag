test(a, b) {
	=> a + b
}

init() {
	a = 5
	b = 7
	c = a * b + 2 * a * 3 * b - a + b + b * 5
	d = c * b - a * c - 3 * 4 * b * a
	e = d * c * b * a - a * b + d
	f = a + 1 + 2 + 3 + 4 + b
	test(d, e)
	g = c + d - e * (e - 1) * (e + d) + (a + c)
	h = 3434 * a + b - e * f + c * 23465
	i = d - g + 24 * g + f
	=> d * e + g
}