f(a, b, c, d, e, f) {
   => a + b + c + d + e + f
}

export g(a: num, b: num) {
   # a + 1 + 0.5a + 4a + b + 1 + 2b + 0.25b
   # 5.5a + 3.25b + 2
   => f(a + 1, a / 2, a * 4, b + 1, b * 2, b / 4)
}

init() {
   g(1, 1)
   => 1
}