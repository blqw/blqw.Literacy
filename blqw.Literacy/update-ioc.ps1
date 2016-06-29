 $x = $x = Split-Path -Parent $MyInvocation.MyCommand.Definition
 wget https://raw.githubusercontent.com/blqw/blqw.IOC/master/blqw.IOC.Lite/MEFLite.cs -outfile  $x/IOC/MEFLite.cs
 wget https://raw.githubusercontent.com/blqw/blqw.IOC/master/blqw.IOC/nuget-pack.bat -outfile  $x/nuget-pack.bat