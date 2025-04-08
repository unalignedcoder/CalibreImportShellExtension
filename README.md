# Calibre Import shell extension

<img src="https://github.com/user-attachments/assets/89775627-822b-4d0e-baa1-615cd5598dc7" alt="CalibreImport Shell Extension" title="Download Calibre" align="left" style="height:100px" />

**Calibre Import** is a must-have shell extension for [Calibre](https://github.com/kovidgoyal/calibre "Calibre Github repository") on Windows 10 and 11. 

It allows to intelligently import one or multiple eBook files into Calibre directly from the Windows Explorer context menu. More significantly, it seamlessly enables the user to choose the target library for the eBooks before importing. 

&nbsp;

<p align=center>Why, thank you for asking!<br />👉 You can donate to this project <a href="https://www.buymeacoffee.com/unalignedcoder" target="_blank" title="buymeacoffee.com">here</a></p>

## Usage
Simply right-click on any eBook, or group of eBooks, and select "Import to Calibre".  The entry submenu will offer a list of all Calibre libraries presently in the system. Select the one into which you want to import the books.

![image](https://github.com/user-attachments/assets/cc6b9394-14ff-4110-93e3-70d715ddf9aa)

Alternatively, you can use a dialog window rather than the submenu. The functionality is the same.

![image](https://github.com/user-attachments/assets/1598cf4a-f025-428a-806f-ed44d40578f2)

The selected files can have any of the Calibre-supported eBook file types. Calibre Import will retrieve such list dynamically from the registry, and it usually corresponds to the following file extensions: 

`.epub, .pdf, .mobi, .azw, .azw3, .fb2, .djvu, .lrf, .rtf, .txt, .doc, .docx, .odt, .htm, .html, .cbz, .cbr, .pdb, .snb, .tcr, .zip, .rar`.

Once the import has concluded, the user will be prompted to start Calibre and directly open the library into which the ebooks have been imported.

## Settings

A number of options can be set to suit your needs. In particular: 

* Hide libraries from the context submenu (this obviously doesn't affect the library visibility in Calibre!)
* How the import process should behave with duplicates. The following options are established by Calibre: `overwrite` (the new book will replace the old); `ignore` (the book will not be imported); `new_record` (a duplicate entry will be created). Such settings are valid for all import operations, and cannot be modified on a per-book basis.
* Whether to use a dialog or a submenu entry.
* ... and more.

![image](https://github.com/user-attachments/assets/16919cec-1763-4d21-943f-193034114e81)

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

## Requirements
This project has only been tested on x64 machines, with Windows 10/11 and calibre 7/8. It may or may not work on different Windows systems and older Calibre versions. It requires the [.Net Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48).

## Installation
 ### Setup executable
 1) Download the setup file from the [latest release](https://github.com/unalignedcoder/CalibreImportShellExtension/releases).
 2) Run the setup and follow instructions.

 ![image](https://github.com/user-attachments/assets/d57d78c2-7ff3-4b7e-b3d2-defbfed3522d)

 This method will install the extension on a per-user basis. The setup wizard was created using InnoSetup, and you can investigate what it does by looking into the CalibreImportSetup.iss file here on Github.

 ### Zipped folder
 If you prefer running the dll and config file from the same folder, you can use the included PowerShell script to install in a "portable" mode (it's not really portable since it still needs to register the dll in the registry, but you know what I mean). 

 1) Download the [latest release](https://github.com/unalignedcoder/CalibreImportShellExtension/releases).
 2) Extract the files to a folder of your choice.
 3) Run the `Setup.ps1` Powershell script. The script will register the Dll, add the necessary Registry entries and optionally restart Explorer, so as to make the new context menu entry immediately available. Read inside the Setup script for more instructions.

&nbsp;

<p align=center>👉 You can donate to this project <a href="https://www.buymeacoffee.com/unalignedcoder" target="_blank" title="buymeacoffee.com">here</a></p>

## Uninstallation
If you installed via Setup executable, just uninstall from the "Apps" settings, or "Programs and Features" Control panel entry.

If you installed via script, simply run the Setup script again with the `-u` parameter. 
All Registry entries will be deleted and the DLLs unregistered. All is left is to manually delete the folder with the files.


