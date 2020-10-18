Factory { T } {
   dummy: num

   create() {
	  => T()
   }
}

LargeFactory { X, Y } {
   capacity: num

   init() {
	  capacity = 100
   }

   create_x() {
	  => X()
   }

   create_y() {
	  => Y()
   }
}

Apple {
   private weight: num
   
   orange: Orange

   init() {
	  weight = 100
   }

   get_weight() {
	  => weight
   }
}

Orange {
   private sugar_percent: num

   apple: Apple

   init() {
	  sugar_percent = 70
   }

   get_sugar_percent() {
	  => sugar_percent
   }
}

to{T}(input) {
   => input as T
}

init() {
   factory = Factory(Apple)
   apple = factory.create()

   large_factory = LargeFactory(Apple, Orange)
   large_apple = large_factory.create_x()
   large_orange = large_factory.create_y()

   large_apple.orange = large_orange
   large_apple.orange.apple = apple
   large_apple.orange.apple.orange = large_apple.orange

   apple_orange = to(Orange, apple)
   apple_orange.sugar_percent = -100

   => apple.get_weight() + large_apple.get_weight() + large_orange.get_sugar_percent()
}