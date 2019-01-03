
Outside of the Box 
------------------

Link to video: https://www.youtube.com/watch?v=lj52geM-z0A&t=4s

------------------

Michael Schumacher

------------------

Play as a box that has lost its way after it falls off the back of a transport truck.
In a race against time you need to make your way back to the box so you can continue
to your destination! Roll, jump, and slide toward you goal before the timer runs out
to fulfill your destiny. After you've reached your goal explore the randomly generated
and endless landscape in fly mode, in which the box can zip around the map. Change the 
options of the map generation to make each playthrough unique

------------------

The game begins with a start menu which allows the user to select if they would like
to play the original game, enter fly mode, change the map settings, or leave the game.
The player is able to change all of the variables of the map generation, including:
	
	- Uniform Scale......... Size of each chunk
	- Height Multiplier..... Max height of the map
	- Seed.................. Used for noise generation
	- X and Y offsets....... For start location
	- Noise Scale........... Effectively "zooming" in or out of the noise
	- Octaves............... Controls the difference in frequencies
	- Normalization Mode.... Global normalization makes smooth transitions for chunks
				 (this is the recommended setting for gameplay)
	                         Local normalization creates more realistic landscapes 
				 (but with mal-aligned chunks)
	- Persistence........... Affects the frequency of the noise
	- Lacunarity............ Additional change to the frequency of the noise

If the player applies these changes in the options menu they are compiled immediately
and the scene is reloaded to apply the changes to the new map. Though the map is randomly
generated, it is persistent from playthrough to playthrough if the settings are kept the
same, so leaving a chunk and returning to it later in the game will result in the original
chunk being re-spawned.

Each chunk is stored as its own object and has its own copy of its properties (e.g. its
mesh, vertices, etc.). Each chunk also has a Level of Detail modifier to change the 
complexity of its mesh based on how far away the player is from that chunk. This helped
with some in-game lagging issues when chunks would be spawned / de-spawned, though there
is still some noticeable spikes when chunks are created.

The texture of the landscape is based on the height of vertices. There are cutoffs for
each texture. Though I do not allow for players to change the height cutoffs, they can
be changed by modifying the values in the Default Text" object in the Terrain Assets folder.
Each texture also comes with a tint that can be strengthened / weakened in the same object
mentioned above. This allows you to replace the texture (or supplement it) with a solid color
at that height.

In fly mode the player is able to change their altitude and move quickly around the 
map in order to explore and see how their changes have affected the map. It is worth
noting that leaving fly mode does not change any of the map parameters, so a player
could explore the map and then play on it afterwards. When the user begins fly mode the 
Culling Distance set by the Unity Engine is changed to be as large as possible.
However, the map is not updated until the player moves a set distance in the X or Z
directions. So, after moving this distance, the level of detail of distance terrain
chunks are increased to be as high as possible so that the player can see as far as
possible. This does come with a hit in performance, however. It is possible to fly
high enough to see the edge of the generated terrain, and therefore to see chunks
being generated and being de-spawned. 

Once the player enters the standard game mode they are locked in place for 10 seconds
while they read the controls and their objective. During this time the truck is still
moving away from the player. Additionally, every 7 seconds the truck turns by a random
amount in the range (-60 degrees, 60 degrees) so that it isn't going in a straight
line away from the player (that would be boring!). To follow the truck a box is dropped
off of the back every 5 seconds to act as a sort of trail for the player to follow. 

After the 10 seconds the player gains control and the timer starts ticking. The player
may really choose what they would like to do, be it to follow the truck or not. The 
player moves using WASD and an appropriate force is applied to the rigidbody of the box.
The player can also jump using SPACE, which was particularly helpful when trying to
scale the side of a mountain.

The game is ended when the player either collides with the truck, resulting in victory,
or when the timer ends or if the player falls into water, both resulting in a loss. 

There are two known issues:

	- Truck / player spawning:
		Because of the variation in the height of the starting point on the map
		there are some combinations which cause the player and the truck to 
		spawn under the map, causing them to free fall forever. This is
		an issue especially when options are changed to make the map very 
		tall or when uniform scale is larger than 5.

	- Terrain Collision Detection:
		There have been some incidences when the player hits the terrain at
		a certain angle or speed which causes the player to clip through the 
		map, sending them on an endless free fall. This is likely due to the 
		"Level of Detail" setting applied to the terrain, which attempts to
		cut down on the number of colliders in the terrain mesh to save
		computational effort. I have set the Level of Detail to be as high as
		possible for the chunk on which the player currently is. Despite this
		the issue can still be replicated in some circumstances.

	- Truck movement (sort of)
		Originally the truck was able to flip upside down while driving, which
		isn't something trucks should really do. However, after having friends
		test the game out they seemed to like when the truck would make huge
		jumps and flip upside down (sometimes followed by a sudden change of 
		direction), so I decided to keep it in the final game. I wanted to list
		it here, however, so it does not seem like it was just ignored.

------------------

This project was completed individually.

------------------

Box Asset: 
	
	I used the basic model for the boxes in the game, which came complete with
	rigidbodies. I added box colliders to the prefab so that I could detect
	player collisions. The box did not come with any controller scripts so those
	were written by myself.
	
	https://assetstore.unity.com/packages/3d/props/pbr-cardboard-box-110635

Truck Asset:

	I used the truck asset, which came with a rigidbody. I added a collider so that
	the truck may move across the terrain, and I placed the box prefabs on the 
	flatbed of the truck. It did not come with any scripts so the controller for
	the truck was written by myself.

	https://assetstore.unity.com/packages/3d/vehicles/land/mini-cargo-truck-68663

Landscape:

	Sebastian Lague has an incredibly helpful youtube series on procedural landmass
	generation based on Perlin Noise. I followed along many of his videos to go from
	generating Perlin Noise to projecting the noise upward into a terrain to creating
	meshes and finally applying textures based on vertex height. There are many places
	that I modified the code to better suit my game, including the ability of the player
	script and the truck script to access all of the information of the landscape to create
	better start positions. Additionally I modified the accessibility of the data to allow
	the players to modify the parameters of the landscape to generate what they'd like.

	I really cannot thank Sebastian enough, his tutorials were absurdly easy to follow and
	they helped to produce a great project!

	https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ?&ab_channel=SebastianLague

Soundtrack:

	The background music for the game was found on a Royalty Free music website called
	freemusicpublicdomain.com. The track name is Robot Coupe by Lost European.

Sound Effects:
	
	The sound for the truck was also found on a public domain / royalty free website
	called soundbible.com. The sound name is "Tank Sound".

------------------

CMSC425 - Professor David Mount - Fall 2018

