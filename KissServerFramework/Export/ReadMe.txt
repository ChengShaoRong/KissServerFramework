Folder '/Export/Server/' is for server code backup (We had copy '*.cs' to the path that same with the '*.ridl'), all files in that folder name suffix with '.cs.txt'.

Folder '/Export/Client/FullVersion' is for the C#Like FULL version, and folder '/Export/Client/FreeVersion' is for the C#Like FREE version.
You choose that according to your C#Like version . All files in their folder name suffix with '.cs.txt', please rename all the '*.cs.txt' to '*.cs' after you copy to your Unity project, because we don't want that code be identified as server code by KissServerFramework project.

You can copy that client code to your C#Like HotUpdate script folder in your Unity project, such as 'Assets\C#Like\HotUpdateScripts\NetObjects\'.

Strong recommend that set absolute path in KissGennerateRIDL.ini for copy the '*.cs' into your Unity project when compile server code. 
If your are using Full C#Like version, you can set 'KissGennerateRIDL.ini' like this 'folderClientFull = D:/MyProject/Assets/C#Like/HotUpdateScripts/NetObjects' 
If your are using Free C#Like version, you can set 'KissGennerateRIDL.ini' like this 'folderClientFree = D:/MyProject/Assets/C#Like/HotUpdateScripts/NetObjects'