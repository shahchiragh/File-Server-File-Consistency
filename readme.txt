Name: Shah Chirag Hareshkumar
UID: 1001558824

Project: Lab-2 File Consistency

Programming Language: C# Windows Forms Application

Code Structure:
1. FileServerApplication is Multithreaded TCP Server Application. The folder consists of FileServerApplication.sln file which
can be opened with a Visual Studio 2013+ versions. Also, it consists FileServeApplication folder which has Application
configurations, form properties, etc.
2. FileServerClient is TCP Client Application. The folder consists of FileServeClient.sln file which
can be opened with a Visual Studio 2013+ versions. Also, it consists FileServeApplication folder which has Application
configurations, form properties, etc.
3. Both applications has usage of Sockets to accomplish TCP connection and does not use and HTTP tags or mechanisms.
4. The FileServerApplication uses a hardcoded path "E:\\FT" for getting the files uploaded.
5. The FileServerClient Application uses a hardcoded path "E:\Client1-FT" or "E:\Client2-FT" or "E:\Client3-FT" for downloading the files at
   3 clients location resp.
6. In this particular assignment it is assumed that we are focusing changes only one text file and that is ChiragShah.txt file.
7. For above both applications, the path names can be changed at receivedPath for both the applications respecively inside Form1.cs. 




Run Code:
1. To directly run Server, go to path
..\shah_chs8824\FileServerApplication\FileServerApplication\bin\Debug open FileServerApplication.exe

2.  To directly run Client, go to path
..\shah_chs8824\FileServerApplication\FileServerClient\bin\Debug open FileServerClient.exe

3. To directly run the execution, open FileServerClient.exe from Client1, Client2 and Client3 folder and FileServerApplication.exe from Server
   folder.

3. If exe's are not permitted to be executed then .sln files can be imported to Visual Studio and project solutions can be builded 
and can be executed from Visual Studio.

