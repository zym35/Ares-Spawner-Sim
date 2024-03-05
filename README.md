# Ares Spawner Sim
King Zhou 2024.03.04

## Implemented features
- 4 spawners, each as a prefab variant, for spawning a specific type of moving cubes.
- 4 types of moving cubes (color-coded). After spawning, the cube will find a random location inside the map, and move towards it until reaching it. After that, the cube finds a new location and loops.
	- Specific cube data (speed, color, scale) are stored in `ScriptableObjects` in the Resources folder. This is dynamically loaded at runtime, and can be easily modified to quickly create different cube behaviors. Also compatible with hot reloading.
  - The random locations are provided by a singelton `WaypointProvider`, which pre-generates a fixed amount (default to 300) of random points within the map area (defined by two transforms), then returns a random one when needed.
  - The cube will reflect its direction when hitting the wall.
- Object pool `CubePool` is implemented from scratch (based on `List<>`). It handles object pooling, disposing, dynamic resizing, etc. It is implemented as a singleton.
	- The pool has a maximum capacity. Upon reaching max capacity, no new objects are generated, and any following logic is skipped.
- Upon same-type collision, a new cube of the same type is generated in the middle.
	- This will trigger all three cubes to enter a "no new collision" mode for a period of time, in order to prevent a burst of new objects spawning within a very short time window.
		- The "no new collision" time duration has a base value of 1s. As there are more objects in the scene, this duration would increase slightly, to prevent very fast growth of object amount.
		- Any newly spawned cubes will also have this "no new collision" protection.
  	- The cube will turn red when in this mode.
  - To prevent two cubes spawning together, as the collision event trigger for both sides, a primary spawning event handling cube is determined by comparing their `GetInstanceID()`s.
- Upon different-type collision, both are destroyed (returned back to pool).
- Basic UI implemented for showing the current active cube amount.
- Basic slider UI implemented for dynamically adjusting the spawning rate for each spawner. Default to 1/s.

## Metrics
- On my computer in edit mode, the fps at max (1000) objects are around 100 fps.
- Reading from the profiler, the major performance cost is physics simulation (5-8ms). Monobehavior update takes around 1-1.5ms.

## Possible future improvements
- Use a more efficient object pooling with unmanaged collection types, or use Unity's `ObjectPool<>`.
- Use ECS and burst compiler. This is projected to drastically improve the performance from my previous experiences.
- Some optimizations on rendering. URP is currently used.
