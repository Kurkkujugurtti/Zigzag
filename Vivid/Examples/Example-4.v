#Foo {
#	a
#	b
#
#	Foo(x, y) {
#		a = x * x
#		b = y + y
#	}
#
#	sum() {
#		=> a + b
#	}
#}

f(a, b) {
	x = a + a - 1 * b * 7
	y = a - a * x * b
	z = x * y
	=> z
}

#fibonacci(iterations) {
#	i = 0
 #   first = 0
#	second = 1
#	next = 0
#
#	loop (i < iterations) {
#		if i <= 1 {
#			next = i
#		} 
#		else {
#			next = first + second
#			first = second
#			second = next
#		}
#
#		++i
#	}
#}

init() {
	a = 3
	b = f(1 + a, 2 * a + f(a, (a - 1) * (a + 1)))

	#fibonacci(b)

	#foo = Foo(a, b)
	#c = foo.sum()
	#foo.a = a + a

	#if a > a + a {
	#	f(a, a)
	#}
	#else if a > 3 {
	#	f(a + a, a + a)
	#}
	#else if f(1, 2) < f(2, 3) {
	#	=> 13434
	#}
	#else {
	#	f(a + a, a + a)
	#}

	#foo[0] = a[b]

	#list = List()
	#list.create(10)
	#list.add(foo)

	#loop() {
	#	f(a, 2)
	#	a = a + 1
	#}

	#a = 1 + 2
	#b = 1 + 2
	#c = a + b
	#d = a + b + 1 + 2
	#=> d
}
