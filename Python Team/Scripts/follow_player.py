print Game.time
enemy1.Target = player.Position
enemy2.Target = player.Position

if Game.time > 15:
    enemy1.Sleep( 10 )
    enemy2.Sleep( 10 )
    Game.time = -15