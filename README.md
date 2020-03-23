# ArtNetTimecodeConsole v1.0
![.NET Core](https://github.com/hossimo/ArtNetTimecodeConsole/workflows/.NET%20Core/badge.svg)

![Example Image](Images/Terminal-Image.png)

 A simple .NET Core Console Application to send Art-Net Timecode

 ## Syntax
 `ArtNetTimecode [ip]`

 **ip** - (optional) if included ArtNetTimecode is unicast to the IP address, else ArtNetTimecode will be broadcast (255.255.255.255)

 ## Notes
 
 This application required the .NET Core Runtime in order to run. you can download it from Microsoft for Windows, Linux, macOS and Docker. choose your Platform and download the .NET Core Runtime or SDK depending on your platform.

 In this version the local computers *time of day* will be used for the ArtNetTimecode output.

 The Included macOS executable needs to be made executable due to an issue with the way github packages files. To make the file executable by both the user and group that own the file enter the following in the extracted folder `chmod ug+x ArtnetTimecode`. then you can run the application from terminal by running `./ArtNetTimecode`; However, because this application is not code signed or notarized, running from the terminal will likly fail the first time. In order to run the application after making it executable right (or option click) and choose Open then choose open again to allow the application to run.
