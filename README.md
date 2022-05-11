# unity-step-system
A step system for Unity using TreeView and ScriptableObject.

Warning: Work in progress!

# Requirements
- Unity 2020.3.20f1

# Idea
The main focus of this project is to create a step system (circuit break) integrated with Unity that has a friendly UI and various options for each step.
Each step will derive from a Scriptable Object and will have its own logic to it: Some are necessary for the game (not optionals), some are asynchronous, some must be done after others, some steps can be divided into smaller steps, etc.

# Example
When developing a game you have to load and make some calculations at the start of the gameplay, like setting up the main character, setting up a server connection, downloading game store files, asking the server about other player's positions, etc.

Here is a simple imaginary example of how you can arrange some initialization steps as a tree:

![Window Example](https://user-images.githubusercontent.com/20073691/167965443-f15a9aba-9f72-4bb2-9fe9-e91d2e382b0e.png)

# Next Steps (ha!)
The features that will be implemented in order to deploy the version 1.0.0 are marked with a star (*).

## General
- Unit tests (*).

## UI Improvements
- Display other lists that are inside the list you are currently seeing.

## More Step Types
Suggestions are great!
- DecisionStep: Splits the flow between various other steps, choosing one or multiple to be executed.

## Nice To Have
- Support [UniTasks](https://github.com/Cysharp/UniTask);
- The ability to cancel an AsyncStep using a [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=net-6.0);
- Change the background color of an element ever so slightly based on its type;
- A way to add icons using the UI.

# Cool Things
[TreeView](https://docs.unity3d.com/Manual/TreeViewAPI.html) is a Unity class you can use in your own projects.
