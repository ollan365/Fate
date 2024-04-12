Welcome, welcome welcome and thank you for purchasing or otherwise aquiring Quirky's Easy Animatable Vignette.



Add a touch of atmosphere to your game in 2 simple steps!

1) Go to the Q_Vignette prefab folder and drag either Q_Vignette_Single or Q_Vignette_Split onto your canvas. If you'd like the Vignette to affect other things on the Canvas, place it right at the bottom - if not, right at the top.

2) Adjust the settings to your liking - voila :D You are now happy! Aren't you?




OK, I admit, there's other potential steps if you really want...

3) Animate the Vignette - using Unity's own animation system - it's mostly very simple. Mostly.

4) Make your own brilliant corner types and insert them in the inspector.

5) Put your feet on the desk, go on, you've earned it!




FAQ

Q. Why can't I animate between the Corner Types?
A. As with the demo animation, you can flick between corner types using 2 keyframes right next to each other, but trying to fade between them gradually will produce some unexpected results. For now, if you want to fade between Corner Types, you'll need to use 2x Vignettes and fade between them yourself. The reason for this is that your Corner Type selection is stored as an integer number so if you animate between corner type 1 and type 5 for instance, it'll slowly flip between types 1,2,3,4 and 5. I might add some convoluted system that makes it easier for you one day...

Q. What's the difference between Q_Vignette_Split and Q_Vignette_Single?
A. Q_Vignette_Split is useful if you want to create different effects between the sky and the ground.

Q. Is this Quirky's first Asset on the Asset Store?
A. Yes indeedy! I've got lots more to offer but I had to test the water with something nice and simples!

Q. Can't the vignette effect be acheived in post processing.
A. To a certain extent yes, however some are not animatable, and you can't just shove your own corners types in any old how! Also, using smaller scale settings, Quirky's Vignette may take less processing than regular post processing.