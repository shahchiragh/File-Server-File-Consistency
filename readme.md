Name: Chirag Shah

# File Server File Consistency

Programming Language: C# Windows Forms Application

Objectives:
1. Exposure to Consistency Models.
2. Experience with Push/Pull.
3. Experience with Invalidation Protocols.
4. Further exposure to Sockets and the Uploading and Downloading of Files.

Project Specification:
The Project is built on tops of File-Server project (https://github.com/shahchiragh/File-Server). This project consists of a server and three clients. 
The general procedure is as follows:
1. All clients will begin with a single, identical .txt file loaded into a pre-determined directory.
2. A simple text editing program (e.g., Notepad) can be used to modify the contents of that .txt file on any individual client. The copies of the .txt file across the system are now said to be inconsistent.
3. The client will automatically detect that the file has been updated and Push those updates to the file server.
4. When the server receives a file update from a client, it will issue Invalidation Notices to the remaining clients.
5. Upon receipt of the Invalidation Notice, the clients will Pull the updated file from the server and update their local copy. The copies of the .txt file across the system are now said to be consistent.
6. The program will repeat this update process, beginning with any individual client, as many times as necessary until the program is killed.

## This program will maintain the consistency of only a single text file and files and files will not be updated concurrently. Further component specifications are as follows:

## Client:

Each client process will connect to the server over a socket connection and provide a unique user name to be displayed at the server. 
Upon connecting to the server, the client will automatically Pull any file updates.
The client will utilize a simple GUI to provide state information about that client. 
The user will be informed of the state of the client as appropriate, for instance: “Client connected,” “File change detected,” “Invalidation notice received,” et cetera.

## Server:

The server will utilize a simple GUI to provide state information about the server, including which clients are presently connected. 
The server will handle multiple socket connections simultaneously. The server will not identify, or correct, write conflicts.
The file exchange may be performed using whatever mechanism is most convenient. 

Code Structure:
1. FileServerApplication is Multi-threaded TCP Server Application. The folder consists of FileServerApplication.sln file which
can be opened with a Visual Studio 2013+ versions. Also, it consists FileServeApplication folder which has Application
configurations, form properties, etc.
2. FileServerClient is TCP Client Application. The folder consists of FileServeClient.sln file which
can be opened with a Visual Studio 2013+ versions. Also, it consists FileServeApplication folder which has Application
configurations, form properties, etc.
3. Both applications has usage of Sockets to accomplish TCP connection and does not use and HTTP tags or mechanisms.
4. The FileServerApplication uses a hard coded path "E:\\FT" for getting the files uploaded. (E:// is a root source in my project, you can tweak this path according your folder structure)
5. The FileServerClient Application uses a hard coded path "E:\Client1-FT" or "E:\Client2-FT" or "E:\Client3-FT" for downloading the files at
   3 clients location resp.
6. In this particular assignment it is assumed that we are focusing changes only one text file and that is ChiragShah.txt file.
7. For above both applications, the path names can be changed at receivedPath for both the applications respectively inside Form1.cs. 



Run Code:
1. To directly run Server, go to path
..\FileServerApplication\FileServerApplication\bin\Debug open FileServerApplication.exe

2.  To directly run Client, go to path
..\FileServerApplication\FileServerClient\bin\Debug open FileServerClient.exe

3. To directly run the execution, open FileServerClient.exe from Client1, Client2 and Client3 folder and FileServerApplication.exe from Server
   folder.

3. If exe's are not permitted to be executed then .sln files can be imported to Visual Studio and project solutions can be built 
and can be executed from Visual Studio.

