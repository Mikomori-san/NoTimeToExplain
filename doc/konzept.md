# No Time To Explain

Normalerweise heißt es ja Gut gegen Böse, doch hier wird der Spieß mal umgedreht.
In No Time To Explain spielst du einen kleinen, aber mächtigen Zeitdämon, der seine Macht nie missbraucht oder auch nur eingesetzt hat. Dennoch fürchten dich Gott und Luzifer so sehr, dass sie dich in die tiefsten Ebenen von Luzifers Schloss in der Hölle verbannen und dir deine Kräfte zu rauben versuchen. Das gelingt ihnen aber nur so halbwegs, weshalb um dir jetzt komische Zeit-Zustände herrschen und du nicht mehr lange zu leben hast.
In dir entbrennt eine fürchterliche Wut, angetrieben von Unverständnis und Hass. Das ist der Moment, in dem du zum „Bösen“ wirst. Du hast nichts anderes mehr im Kopf als Rache, weswegen du in der Hölle einen Amoklauf startest. Jetzt liegt es an dir, so viele Seelen wie möglich zu verschlingen und auszulöschen, um die Hölle in Furcht und Chaos zu versetzen, bevor deine Lebenszeit abläuft. Time’s ticking…

## Movement
Das Spielfeld, indem man sich bewegt, besteht aus verschiedenen Feldern (Tiles). Es gibt genau 2 Arten von Feldern: Wege (Paths/Movables) auf denen man sich bewegen kann und Hindernisse (Obstacles) die einem den Weg blockieren. 
Man bewegt sich mit WASD um jeweils 1 Tile in eine Richtung.
**W = Oben**
**A = Unten**
**S = Links**
**D = Rechts**
Da spezielle hierbei: Immer wenn sich der Spieler bewegt, bewegen sich auch die Gegner. Das heißt, das Movement ist sozusagen in Zügen unterteilt.  Wenn der Spieler es schafft, in seinem Zug auf einen Gegner zu springen, tötet er diesen.

## Enemies
Es gibt 4 verschiedene Gegnertypen: 
**Stone Golem**
**Broken Stone Golem**
**Base Stone Golem**
**Lava Golem**
Jeder Gegner hat sein eigenes Angriffspattern. Wenn der Spieler in Angriffsreichweite ist, bereitet der jeweilige Gegner seinen Angriff vor. Wenn der Spieler es nicht schafft, aus der Reichweite zu kommen, springt der Gegner auf den Spieler und es ist Game Over. Sollte der Spieler es doch schaffen, springt der Gegner auf den Tile, auf dem der Spieler das letzte mal gestanden ist.

## Level
Ein Level besteht aus mehreren Räumen. Es gibt insgesamt 12 verschiedene Presets and Räumen. 
Der erste ist immer der Spawn-Room. Dieser Raum beinhaltet das blaue Pentagram, auf dem der Spieler spawnt. Der letzte Raum ist immer der Teleportation-Room. Dieser beinhaltet das Portal, das zum nächsten Level führt. 
Zwischen den beiden Räumen werden immer zwischen 0 und 4 Räume aus den restlichen 10 Räumen generiert.
Um in den nächsten Raum zu gelangen, muss der Spieler auf das nach links gerichtete Stairs-Tile hüpfen. Sobald dies erledigt wurde, wird der nächste Raum generiert. Um wieder zum vorherigen Raum zu gelangen, muss man auf das nach rechts gerichtete Stairs-Tile hüpfen, damit dieser wieder re-generiert wird.

## Ziel / Score
Das Ziel des Spiels ist es, vom Spawn-Room zum Teleporter-Room zu gelangen, um in das nächste Level zu laden. Während dem Level muss man Gegner töten, um deren Seele zu bekommen. Die bereits entnommenen Seelen werden links oben am Bildschirm angezeigt.
Am Anfang des Levels erscheint ein Countdown rechts oben, der von 120 Sekunden herunterzählt. Wenn der Timer 0 erreicht hat, stirbt der Spieler. Jeder neu entdeckte Raum bringt 30 Sekunden. Am Schluss werden beim Laden des nächsten Levels dann die entnommenen Seelen und die restliche Zeit des Levels in den tatsächlichen Game Score umgewandelt. 
Die Rechnung hierfür lautet: SCORE = SCORE + SOULS x 10 + REMAINING_TIME x 3
Das heißt, wenn der Spieler im 1. Level bereits stirbt, ohne das Portal zu erreichen, bekommt er keinen Game-Score.
Wenn der Spieler beispielsweise im 2. Level bereits 30 Seelen gesammelt hat und dann stirbt, werden diese nicht in den Game-Score miteingerechnet. Der Game Score wird also immer nur nach abschließen eines Levels raufgezählt.