SystemTrayModder
Original Script: http://xtremeconsulting.com/blog/windows-7-notification-area-automation-falling-back-down-the-binary-registry-rabbit-hole/
Original Author: Micah Rowland
C# Port Author: Timothy Lester
Last Updated: 2015-05-28

This application is a command line application that allows the user to set a system tray icon to always be visible.

Usage:  SystemTrayModder.exe <ProgramName> <Setting>
  ProgramName - The name of the executable that is running the system tray icon. Example... rcgui.exe
  Setting - The type of visibility needed. (2 - Always visible, 1 - Updates only, 0 - Hidden in system tray such as is default)
