Item {
   x: num
   y: num

   init(x0, y0) {
	  x = x0
	  y = y0
   }

   add(amount: num) {
	  x += amount
	  y += amount
   }

   assign_plus(amount: num) {
	  add(amount)
   }
}

init() {
   list = List(Item)
   
   item = Item(10, 1)
   
   list.add(item)
   list += item

   a = 1
   b = 2
   c = a ¤ b
   c ¤= c

   list[0].add(2)

   list[1] += 8
}