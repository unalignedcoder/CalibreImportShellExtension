# Calibre Import Shell Extension

<img src="https://github.com/user-attachments/assets/89775627-822b-4d0e-baa1-615cd5598dc7" alt="CalibreImport Shell Extension" title="Download Calibre" align="left" style="height:100px" />

**Calibre Import** is a must-have shell extension for [Calibre](https://github.com/kovidgoyal/calibre "Calibre Github repository") on Windows 10 and 11. 

It allows to intelligently import one or multiple eBook files into Calibre directly from the Windows Explorer context menu. More significantly, it seamlessly enables the user to choose the target library for the eBooks before importing. 


## Usage
Simply right-click on any eBook, or group of eBooks, and select "Import to Calibre".  The entry submenu will offer a list of all Calibre libraries presently in the system. Select the one into which you want to import the books.

![image](https://github.com/user-attachments/assets/ecfdf0b3-12b1-4ed7-bf86-12003f958bb0)

The files can have any of the Calibre-supported eBook file types. Calibre Import will retrieve such list dynamically from the registry, and it usually corresponds to the following file extensions: 

`.epub, .pdf, .mobi, .azw, .azw3, .fb2, .djvu, .lrf, .rtf, .txt, .doc, .docx, .odt, .htm, .html, .cbz, .cbr, .pdb, .snb, .tcr, .zip, .rar`.

Alternatively, the user can use a dialog window rather than the submenu. The functionality is the same.

![image](https://github.com/user-attachments/assets/1598cf4a-f025-428a-806f-ed44d40578f2)

Once the import has concluded, the user will be prompted to start Calibre and directly open the library into which the ebooks have been imported.

## Settings
The Settings window sets some important options.

![image](https://github.com/user-attachments/assets/43552140-d9c9-491b-bc61-0b0b1a1ab3bd)

1) Hide libraries from the context submenu (this obviously doesn't affect the library visibility in Calibre!)
2) Path to the Calibre folder, if the user has it installed in a particular location;
3) How the import process should behave with duplicates. The following options are established by Calibre: `overwrite` (the new book will replace the old); `ignore` (the book will not be imported); `new_record` (a duplicate entry will be created). Such settings are valid for all import operations, and cannot be modified on a per-book basis.
4) Select language (in case the System picked the wrong language for the user;)
5) Whether to use a dialog rather than a submenu entry;
6) Whether to Log all books that are being imported into Calibre, and whether to log all events for debug purposes;
7) Whether to auto-kill Calibre or prompt the user about it (the import process cannot go through if Calibre or any Calibre-related app is running.)

## Localization
The app is translated in the following langauges:
  * Arabic
  * Chinese
  * Czech
  * English
  * French
  * German
  * Italian
  * Japanese
  * Korean
  * Polish
  * Portuguese
  * Russian
  * Spanish
  * Turkish

The Logging is always done in English.

## Requirements
This project has only been tested on x64 machines, with Windows 10/11 and calibre 7. It may or may not work on different Windows systems and older Calibre versions. It requires the [.Net Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48).

## Installation
 ### Setup executable
 1) Download the setup file from the latest release.
 2) Run the setup and follow instructions.

 ![image](https://github.com/user-attachments/assets/66c95ed0-48ac-4e8e-b0f4-36f3167c1f30)

 This method will install the extension on a per-user basis.

 ### Zipped folder
 If you prefer running the dll and config file from one folder, you can use the included PowerShell script to install in a "portable" mode. 

 1) Download the [latest release](https://github.com/unalignedcoder/CalibreImportShellExtension/releases).
 2) Extract the files to a folder of your choice.
 3) Run the `Setup.ps1` Powershell script. The script will register the Dll, add the necessary Registry entries and optionally restart Explorer, so as to make the new context menu entry immediately available. Read inside the Setup script for more instructions.

## Uninstallation
If you installed via Setup executable, just uninstall from the "Apps" settings, or "Programs and Features" Control panel entry.

If you installed via script, simply run the Setup script again with the `-u` parameter. 
All Registry entries will be deleted and the DLLs unregistered. All is left is to manually delete the folder with the files.


