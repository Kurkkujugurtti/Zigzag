﻿Apples {
	quantity: num
	
	init() {
		quantity = 1
	}

	init(initial_quantity: num) {
		quantity = initial_quantity
	}
	
	plus(other: Apples) {
		=> Apples(quantity + other.quantity)
	}

	assign_plus(q: num) {
		quantity += q
	}

	assign_minus(q: num) {
		quantity -= q
	}

	assign_times(q: num) {
		quantity = quantity * q
	}
}

Array { T } {
	private data: T
	count: num

	init(c: num) {
		data = allocate(c * 8)
		count = c
	}
	
	set(i: num, value: T) {
		data[i] = value
	}
	
	get(i: num) {
		=> data[i] as T
	}
}

power_of_two(a) {
	=> a * a
}

init() {
	first_box = Apples()
	second_box = Apples(10)

	first_box += 7
	second_box -= power_of_two(2) * power_of_two(2)

	total_box = first_box + second_box
	total_box *= 10
	
	nums = Array(num, 10)
	nums[1] = first_box.quantity
	nums[2] = nums[1] * second_box.quantity

	=> 0
}