# 😈Viewer3D😈

**A program for rendering 3D objects in real time.**
Works only with .obj files, as it does not use materials or textures.
The most important thing is that you need an .obj file already with triangulated polygons.
Perhaps in the future we should expect updates and increased functionality.

## You can control camera movements using your mouse and keyboard:

	**Mouse Wheel		Changing zoom**
	**Mouse Movement 	Changing viewing direction**
	**W + A + S + D 	Forward Left Backwards Right**
	**Space 			Up**
	**Left Shift 		Down**
	**1-9				Speed change**
	**F12				Entering and exiting full screen mode**

## There is also a configuration file (/Settings/Settings.json)!

	**programName 				(string) 	I think everything is clear here from the title** 
		Example: 	"Viewer3D"
	**viewHeight and viewWidth	(int int) 	Window resolution when opening an application** 
		Example: 	1280 720
	**updateFrequency 			(int) 		Maximum refresh rate**
		Example: 	60 144
	**vsync 						(string) 	Turn Vsync on or off**
		Example: 	"On"  "Off"  "Adaptive"
	**windowState 				(string) 	Window state when opening an application**
		Example: 	"Normal"  "Fullscreen"  "Maximized"  "Minimized"
	**viewingDistance 			(float) 	Viewing distance**
		Example: 	100.0 1000.0
	**polygonMode 				(string) 	Polygon rendering view**
		Example: 	"Fill"  "Line"  "Point"
	**backgroundColor 			(string) 	Background color**
		Example: 	"#2e2e2e" - Only HEX color
	**linesScale 					(float) 	Line scale XYZ**
		Example: 	1.0 - On  0.0 - Off
	**cameraPosition 				(string) 	Initial camera position**
		Example: 	"0.0 7.0 10.0" - (float float float)
	**modelColor 					(string) 	3D model color**
		Example: 	"#f2f3f4" - Only HEX color
	**modelPosition 				(string) 	3D model position**
		Example: 	"0.0 0.0 0.0" - (float float float)
	**modelRotation 				(float) 	3D model auto-rotate speed**
		Example: 	20.0 - On  0.0 - Off
	**modelScale 					(float) 	3D model scale**
		Example: 	0.05  0.5  0.1
	**modelPath 					(string) 	Full path to the .obj file**
		Example: 	/IronMan.obj - Test .obj file in the root directory
		Example: 	"C:\\User\\Desktop\\IronMan.obj"
		Example: 	"C:/User/Desktop/IronMan.obj"
	
## I hope you like it, Good luck! ≧◠‿◠≦✌