import sleep(seconds: large)

init() {
   socket = Socket()

   loop (socket.connect_to('127.0.0.1', 7777) == false) {}

   loop() {
	  socket.send_to('Hola', 5)
	  sleep(1)
   }
}