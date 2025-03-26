# Calibre Import Shell Extension

<img src="https://github.com/user-attachments/assets/89775627-822b-4d0e-baa1-615cd5598dc7" alt="CalibreImport Shell Extension" title="Download Calibre" align="left" style="height:100px" />

**Calibre Import** is a must-have shell extension for [Calibre](https://github.com/kovidgoyal/calibre "Calibre Github repository") on Windows 10 and 11. 

It allows to intelligently import one or multiple eBook files into Calibre directly from the Windows Explorer context menu. More significantly, it seamlessly enables the user to choose the target library for the eBooks before importing. 


## Usage
The user can right-click on any eBook in their File Explorer, and select "Import to Calibre". By "eBook" we mean the files currently supported by Calibre. This app will retrieve such list dynamically from the registry, and it usually corresponds to the following file extensions: 

`.epub, .pdf, .mobi, .azw, .azw3, .fb2, .djvu, .lrf, .rtf, .txt, .doc, .docx, .odt, .htm, .html, .cbz, .cbr, .pdb, .snb, .tcr, .zip, .rar`.

![image](https://github.com/user-attachments/assets/07c6ea38-aa46-42d4-8a3e-7f6f9f6d88db)

The user selection can include one file or multiple files. Such multiple files can have any of the supported extensions. The entry submenu will offer a list of all Calibre libraries presently in the system.

Alternatively, the user can use a dialog window rather than the submenu. The functionality is the same.

<img src="https://github.com/user-attachments/assets/8b9679bb-0943-44a6-ace3-8145c8a0b661"  width=60% />

Once the import has concluded, the user will be prompted to start Calibre and directly open the library into which the ebooks have been imported.

## Settings
The Settings window sets some important options.

![image](https://github.com/user-attachments/assets/a8dedadc-63ac-4406-9a3f-8d4e679d1d48)

1) Hide libraries from the context submenu (this obviously doesn't affect the library visibility in Calibre!)
2) Path to the Calibre folder, if the user has it installed in a particular location;
3) How the import process should behave with duplicates. The following options are established by Calibre: `overwrite` (the new book will replace the old); `ignore` (the book will not be imported); `new_record` (a duplicate entry will be created). Such settings are valid for all import operations, and cannot be modified on a per-book basis.
4) Whether to use a dialog rather than a submenu entry;
5) Whether to Log all books that are being imported into Calibre, and whether to log all events for debug purposes;
6) Whether to auto-kill Calibre or prompt the user about it (the import process cannot go through if Calibre or any Calibre-related app is running;)
7) Select the language of the app (in case the System picked the wrong language for the user.)

## Localization
The app is AI-translated in the following langauges:
  * Chinese
  * Czech
  * English
  * French
  * German
  * Italian
  * Japanese
  * Korean
  * Polish
  * Portoguese
  * Russian
  * Spanish
  * Turkish

The Logging is always done in English.

## Requirements
This project has only been tested on x64 machines, with Windows 10/11 and calibre 7. It may or may not work on different Windows systems and older Calibre versions. It requires the [.Net Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48).

## Installation
Rather than providing a Setup executable, the installation process is done through a PowerShell script. The installation is "portable", since the user can place the necessary DLLs and launch the Setup script from any folder.

1) Download the [latest release](https://github.com/unalignedcoder/CalibreImportShellExtension/releases).
2) Extract the files to a folder of your choice.
3) Run the `Setup.ps1` Powershell script. The script will register the Dll, add the necessary Registry entries and optionally restart Explorer, so as to make the new context menu entry immediately available. Read inside the Setup script for more instructions.

## Uninstallation
Removing this Shell extension is just as easy. The user can simply run the Setup script with the `-u` parameter. All Registry entries will be deleted and the DLLs unregistered. All is left is to manually delete the folder with the files.


