  The application consists of one solutiton which has two projects inside of it. One project is Console application and the other one is Grpc Server.
Console application works as a consumer. In general, the workflow is as follows. Console application listens for specific folder within the system, path of which
can be specified in the appsettings.json file for the console app project. It listens for the created event, meaning in case in the folder specified a new file is being uploaded
it will process that file in terms of reading its data line by line and sending that object to the server for storing it there, successfully transported files will then be deleted
on the consumer side. Console app listens only for .JSON extension files.
  Server on its side has three service methods. One listens for the incoming stream which was specified above and returns success result back. This call is one directional, client stream approach.
There two more calls additionally. The second method is just an unary call that will return all the types of files as a string array. No parameters have to be applied.
And lastly there is a method which returns all the objects that have the type specified as a server side stream. 

In order to run the application you will need to have .NET6 sdk installed. 
It is built using Visual Studio so if it will be opened inside that IDE than it should work fine.
This implementation is for GAMETEQ and the way it is done is the way I understood it. 
 
