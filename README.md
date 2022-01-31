# unity-init-step
A init step configuration for Unity using TreeView and Coroutines.
Warning: Work in progress!

# Requirements
- Unity 2020.3.20f1

# Idea
The main focus of this project is to create an init step (circuit break) system integrated with Unity that has a friendly UI and many options for each step.
Each step will derive from a Scriptable Object and will have its own logic to it: Some are necessary for the game (blockers), some are asynchronous, some must be done after others, etc.

# Example
When developing a game you have to load and make some calculations at the start of the gameplay, like setting up the main character, setting up a server connection, downloading game store files, asking the server about other player's positions, etc.
Here is a simple imaginary example of how you can arange some init steps as a tree where the parent must be successfully completed so all the chilrend can execute:

	Set server connection 					[blocker-async]
		Load spawn area					[blocker]
			Preload nearby areas	 		[async]
		Download game store files 			[async]
		Get remote players' infos  			[blocker-async]
			Init remote entity system   		[blocker]
		Get/Update local player's infos 		[blocker-async]
			Init pet system 			[blocker]
			Init outfit system 			[blocker]
			Init vehicle system 			[blocker]
			Init house system 			[blocker]
		Get ranking data from server 			[async]
			Init ranking system
