import glfwInit(): num
import glfwCreateWindow(width: num, height: num, title: link, monitor: link, share: link): link
import glfwTerminate()
import glfwMakeContextCurrent(window: link)
import glfwWindowShouldClose(window: link): num
import glClear(flags: num)
import glClearColor(r: decimal, g: decimal, b: decimal, a: decimal)
import glfwSwapBuffers(window: link)
import glfwPollEvents()
import internal_cos(angle: decimal): decimal
import internal_sin(angle: decimal): decimal
import glBegin(mode: num)
import glColor3d(r: decimal, g: decimal, b: decimal)
import glVertex2d(x: decimal, y: decimal)
import glEnd()
import glFlush()

cos(angle: decimal) {
	=> internal_cos(angle) as decimal
}

sin(angle: decimal) {
	=> internal_sin(angle) as decimal
}

GL_COLOR_BUFFER_BIT = 16384

TOP_LEFT_X = -0.5
TOP_LEFT_Y = 0.5

TOP_RIGHT_X = 0.5
TOP_RIGHT_Y = 0.5

BOTTOM_LEFT_X = -0.5
BOTTOM_LEFT_Y = -0.5

BOTTOM_RIGHT_X = 0.5
BOTTOM_RIGHT_Y = -0.5

PI1 = 1.57079
PI2 = 3.14159
PI3 = 4.71238

GL_QUADS = 7

false = 0
true = 1
none = 0

init() {
	if glfwInit() == false {
		=> 1
	}

	# Create a windowed mode window and its OpenGL context
	window = glfwCreateWindow(1280, 720, 'Hello World', 0 as link, 0 as link)

	if window == none {
		glfwTerminate()
		=> 1
	}

	# Make the window's context current
	glfwMakeContextCurrent(window)

	r = 0.0
	angle = 0.0

	# Loop until the user closes the window
	loop (glfwWindowShouldClose(window) == false) {
		
		r += 0.001
		angle += 0.001
			
		# Render here
		glClear(GL_COLOR_BUFFER_BIT)

		glBegin(GL_QUADS)
		glColor3d(1.0, 0.0, 1.0)
		glVertex2d(cos(angle), sin(angle))
		glVertex2d(cos(angle + PI1), sin(angle + PI1))
		glVertex2d(cos(angle + PI2), sin(angle + PI2))
		glVertex2d(cos(angle + PI3), sin(angle + PI3))
		glEnd()
		glFlush()

		# Swap front and back buffers
		glfwSwapBuffers(window)

		# Poll for and process events
		glfwPollEvents()
	}

	glfwTerminate()
	=> 0
}