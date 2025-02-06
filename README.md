Game Board - Collapse/Blast Mechanic Game

Overview

This is a tile-matching game implemented in Unity as part of a game development case study. The game features a collapse/blast mechanic, where players can click on groups of the same-colored blocks to remove them. Blocks above the removed group fall down to fill empty spaces, and new blocks appear from the top. The game automatically detects deadlocks and shuffles the board accordingly.

Features

Tile-Matching Mechanic: Click on groups of two or more same-colored blocks to remove them.

Gravity Mechanic: Blocks above a removed group fall down, and new blocks appear at the top.

Dynamic Icon System: Blocks display different icons based on the size of their group:

Default icon for groups of less than 5 blocks.

First icon for groups of 5-7 blocks.

Second icon for groups of 8-9 blocks.

Third icon for groups of 10 or more blocks.

Deadlock Detection: Detects situations where no moves are available and automatically reshuffles the board efficiently.

Smooth Animations: Block movements and shuffling include smooth transitions.

Scalable Board: Supports different grid sizes (2x2 to 10x10) and up to six different colors.

Installation

Ensure you have Unity installed (Recommended version: 2021 or later).

Clone or download the project.

Open the project in Unity Hub.

Set up the scene by placing the BoardManager script on an empty GameObject.

Assign the required UI elements and sprites in the Unity Inspector.

Click Play to start the game.

How to Play

Click on any group of 2 or more same-colored blocks to remove them.

Blocks above the removed group will fall into empty spaces.

New blocks will spawn from the top to refill the board.

If there are no valid moves, the game will automatically shuffle the board.

Try to make larger groups to see different icons appear!

File Structure

BoardManager.cs - Main game logic, including board generation, tile matching, and deadlock handling.

CameraManager.cs - Handles the camera and board scaling.

Block.cs - Defines block properties and behaviors.

UI Elements - Displays messages such as "Shuffling..." during board shuffling.

Future Improvements

Scoring System: Implement a points system based on group sizes.

Level Progression: Add different board layouts and challenges.

Sound Effects: Include audio feedback for actions like matching and shuffling.

UI Enhancements: Improve animations and visual effects.



License

This project is for educational and portfolio purposes. Not for commercial use.

