on = 1
off = 0

Car {
   cost: num
   weight: normal

private:

   owner: String
   status: tiny = off

   start() 
	  => status = on

public static:

   registered = 0

   register(car, owner) {
	  ++registered

	  car.owner = owner
	  car.start()
   }
}

init() {
   car = Car()
   car.cost = 7000
   car.weight = 2000
   
   Car.register(car, String('John'))
}