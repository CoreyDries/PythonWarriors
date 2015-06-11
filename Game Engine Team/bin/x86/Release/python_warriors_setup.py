# This file contains the setup logic for your script.
# Declare any persistent variables or functions you may need in this file.
snake1.Patrol = [ Game.waypoints[1].Position, Game.waypoints[2].Position, Game.waypoints[3].Position, Game.waypoints[0].Position ]
snaketwo.Patrol = [ Game.waypoints[0].Position, Game.waypoints[1].Position, Game.waypoints[2].Position, Game.waypoints[3].Position ]

firedemon1.Patrol = [ Game.waypoints[2].Position ]

def func( sender ):
    if sender == snake1:
        snaketwo.Sleep( 2 )
    else:
        snake1.Sleep( 2 )


snake1.Death += func
snaketwo.Death += func
