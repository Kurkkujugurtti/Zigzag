basic_calculation(a, b) {
	=> a + b - a * b / (1.0 + a) + 3.14159
}

decimal_array_test(a, b, c) {
	array = decimal[(a + c) as num]
	array[a as num] = b + c
	=> array[(b + 7) as num] + c
}

call_test_end(a, b, c) {
	=> a + b + c
}

call_test_start(number) {
	=> call_test_end(number, 1.0 + number, number * 2)
}

#f(a: num) {
#	=> a * 2
#}

#f(a: decimal, b: decimal) {
#	=> a / b
#}

g(i: decimal) {
	array = decimal[100]
	array[i as num] = i
	=> array
}

init() {
	a = 100.0
	b = 700.0
	
	#f(a as num)
	#f(a, b)

	g(100.0)

	c = basic_calculation(a, b)
	d = decimal_array_test(a, b, c)
	e = call_test_start(7)
	=> a + b + c + d - e
}