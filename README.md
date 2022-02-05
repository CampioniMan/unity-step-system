# unity-step-system
A step system for Unity using TreeView and Coroutines.
Warning: Work in progress!

# Requirements
- Unity 2020.3.20f1

# Idea
The main focus of this project is to create a step system (circuit break) integrated with Unity that has a friendly UI and various options for each step.
Each step will derive from a Scriptable Object and will have its own logic to it: Some are necessary for the game (not optionals), some are asynchronous, some must be done after others, some steps can be divided into smaller steps, etc.

# Example
When developing a game you have to load and make some calculations at the start of the gameplay, like setting up the main character, setting up a server connection, downloading game store files, asking the server about other player's positions, etc.
Here is a simple imaginary example of how you can arange some init steps as a tree where the parent must be successfully completed so all the children can execute:

![image](https://user-images.githubusercontent.com/20073691/152297739-43f96ecd-46ef-48f0-a3ab-fdead2a34503.png)

# Next Steps (ha!)
Here are some features that will be implemented in the following weeks in order to deploy the version 1.0.0:

## UI Improvements
- Add/Delete buttons in the UI;
- Proper "Icon" column;
- Display other lists that are inside the list you are currently seeing.

## More Step Types
Suggestions are great!
- DecisionStep: A step to split the flow between various other trees, choosing one or multiple to be executed;

## Nice To Have
- The ability to cancel an AsyncStep using a [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=net-6.0);
- The optional ability to change the background color of an element ever so slightly;

# Cool Things
TreeView is a Unity class you can use in your own projects: https://docs.google.com/document/d/1B14wBRUiURm-LD6cISC5N0nvPngjglgL2T7vpsw1YSQ/edit?usp=sharing
