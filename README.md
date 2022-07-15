# ems-vr-wall-haptics
My HCI lab project extension for giving haptics to walls and weights/resistance to objects in VR via EMS actuation to the wrist, biceps, triceps, and shoulders with RehaStim 2. The technology is demonstrated in the form of a escape room Unity experience.

2 different types of haptics of varying intensity and duration are programmed for the walls, including repulsion (for discrete shocks and pushbacks) and soft (for weights and resistances).

## Instructions
1. Run this project and then open the MuscleDeck scene in Unity
2. Hook up the RehaStim 2 to the computer noting which USB port was used. Update this port name in the inspector view of the RehaStim script (it should be near top of hierarchy)
3. Check that each cable connected to the 8 channels work (use very long cables). If not, note which channels should be disabled. Map the channels and the corresponding muscle groups that they're actuating in the inspector of RehaStim script
4. Connect Quest 2 to computer using Link cable
5. Run the game once you're ready with all conductive pads attached. Turn on RehaStim and science mode. Beating the game is easy enough.
* 2 pads for each wrist, bicep, tricep, and shoulder blades, ensuring there is enough spacing between the pads. Otherwise, RehaStim will freeze.'

## Walkthrough
