Port of Nvidia's C++ NvTriStrip library to C#.

Features:
* Generates strips from arbitrary geometry.
* Flexibly optimizes for post TnL vertex caches (16 on GeForce1/2, 24 on GeForce3).
* Can stitch together strips using degenerate triangles, or not.
* Can output lists instead of strips.
* Can optionally throw excessively small strips into a list instead.
* Can remap indices to improve spatial locality in your vertex buffers.

On cache sizes:

Note that it's better to UNDERESTIMATE the cache size instead of OVERESTIMATING.
So, if you're targetting GeForce1, 2, and 3, be conservative and use the GeForce1_2 cache 
size, NOT the GeForce3 cache size.

This will make sure you don't "blow" the cache of the GeForce1 and 2.
Also note that the cache size you specify is the "actual" cache size, not the "effective"
cache size you may have heard about.  This is 16 for GeForce1 and 2, and 24 for GeForce3.

Credit goes to Curtis Beeson and Joe Demers for the basis for this stripifier and to Jason Regier and 
Jon Stone at Blizzard for providing a much cleaner version of CreateStrips().