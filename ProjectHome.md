Pile - invented by Erez Elul - is a purely relationistic approach to information processing. A Pile does not store any data, but just relations. This has some interesting implications like logarithmic growth of memory consumption and limitless connectable information.

The Pile implementations published here are invitations to play with the Pile information model to find new areas of applications.

More information on Pile can be found here: http://weblogs.asp.net/ralfw/archive/tags/Pile/default.aspx

**Installation**

To recompile the sources for the persistent Pile engine on your machine, you need to download and install [VistaDb 3.x](http://download.vistadb.net/TrialA.aspx) first. A free trial edition is available.

After installation of VistaDb copy the _VistaDB.NET20.dll_ database runtime file into the the Pile.Engine.Persistent\lib folder. The VS.NET project references it from there.