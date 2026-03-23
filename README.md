# Unity-InteractionSystem
A simple interaction system for Unity that uses distance checks to determine which interactable object is closest to the player, and enabling that interactable object's interaction Blip. The system has a built in input handling system using the new unity input system. Interactable objects use an inheritable script, and the manager does the rest.

Add the Interaction Manager to a gameobject

Add an interactable script to an item (Interactable_FenceGate)

When interact is pressed, it will run the Interact() method, which is inherited from the base script, "Interactable".
