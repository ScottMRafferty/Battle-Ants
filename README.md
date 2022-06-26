**Simple unity remake of the Ants demo from Greenfoot Java package.**

Created as an educational example of how to create a roughly similar concept in unity for
my children.

This example has 2 creature scripts and has 2 different flavours of Ants which each have
a default HP (which can be set in the ant prefab in the unity interface).

The original unity Ants use the Creature script which is a reasonably faithful construction
of the Greenfoot Ant script.  CreatureEnhanced relies on RigidBody2D for movement (direction 
and force) rather than actually setting the position.

The two movement scripts may be of generic use elsewhere.

A slider has been added to increase/decrease simulation speed but as CreatureEnhanced movement
does not rely on x,y placement setting the turn speed to some values may cause the ant to do
some weird stuff.

The sprites are mostly scaled so that a larger resolution will just result in a larger play area
and not larger sprites. 

If you know the Greenfoot Ants demo you will recognise that the original sprites have been used
for the red team.

Check out the prefabs directly (models folder) to see which settings can be adjusted per prefab.

Oh - and I added a beetle.  No reason.

As this is a Unity project in GIT I think you'll just need to create a new project and just copy
everything under Assets into the Assets folder in your new project.  Unity may rebuild the
whole project and Library files from everything in this repo but I'm not convinced.
