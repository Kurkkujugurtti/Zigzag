import large_function()

f(x) {
   y = x + 2
   z = x - 2

   large_function()

   z += 4

   large_function()

   => x + y + z
}

init() {
   f(10)
}