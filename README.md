# PSRunner
A Windows Run-like dialog for PowerShell.

![PSRunner Screenshot](https://raw.githubusercontent.com/qJake/PSRunner/master/Assets/PSRunner-Screenshot1.png)

# Releases

Current release: [Version 2.2.0](https://github.com/qJake/PSRunner/releases/tag/2.2.0) (2016/06/21)

[View all releases](https://github.com/qJake/PSRunner/releases)

## Changelog

* Changed global hotkey to Win + N, due to a recent Insider build of Windows 10 stealing the Win + F shortcut.
* Added the ability for the hotkey to be overridden via the command-line - pass in an enum value from the [`Keys` enumeration](https://msdn.microsoft.com/en-us/library/system.windows.forms.keys(v=vs.110).aspx) as the only command-line argument, for example: `PSRunner.exe Y` will use the Win + Y binding instead of the default.
* If no command is entered (the command box is left empty), a new, empty PowerShell session will be opened.

# Contributing

Report bugs, request enhancements, fork it, and send me pull requests via this repo.

Not sure where to start? [Search the code for "TODO:"](https://github.com/qJake/PSRunner/search?q=TODO%3A) and do it! :smile:

# License

Licensed under the [MIT License](https://opensource.org/licenses/MIT).
