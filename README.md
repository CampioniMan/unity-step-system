# unity-init-step
A init step configuration for Unity using TreeView and Coroutines.
Warning: Work in progress!

# Requirements
- Unity 2020.3.20f1

# Idea
The main focus of this project is to create an step system (circuit break) integrated with Unity that has a friendly UI and many options for each step.
Each step will derive from a Scriptable Object and will have its own logic to it: Some are necessary for the game (not optionals), some are asynchronous, some must be done after others, etc.

# Example
When developing a game you have to load and make some calculations at the start of the gameplay, like setting up the main character, setting up a server connection, downloading game store files, asking the server about other player's positions, etc.
Here is a simple imaginary example of how you can arange some init steps as a tree where the parent must be successfully completed so all the children can execute:

![image](https://user-images.githubusercontent.com/20073691/152297739-43f96ecd-46ef-48f0-a3ab-fdead2a34503.png)
