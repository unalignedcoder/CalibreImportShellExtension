#define ReleaseFilesPath "..\ReleaseFiles\"
#define ResourcesFilesPath "..\Resources\"
#define AppName "CalibreImport"
#define MainDll "CalibreImport.dll"
#define AppIcon "MainAppIcon.ico" 

[Setup]
AppName={#AppName}
AppVersion=1.0.5.4
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
UninstallDisplayIcon={app}\{#MainDll}
SetupIconFile={#ResourcesFilesPath}{#AppIcon}
OutputDir={#ReleaseFilesPath}
OutputBaseFilename=CalibreImportSetup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
WizardImageFile={#ResourcesFilesPath}Wizardlarge.bmp
WizardSmallImageFile={#ResourcesFilesPath}Wizard.bmp
WizardStyle=modern
RestartIfNeededByRun=False
CloseApplications=no
// details of the Setup executable:
VersionInfoCompany=Tuscoss
VersionInfoCopyright=Copyright © 2025 Tuscoss
VersionInfoProductName={#AppName}
VersionInfoProductVersion=1.0.0.1
VersionInfoVersion=1.0.0.1

[Files]
Source: "{#ReleaseFilesPath}*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Check: Is64BitInstallMode
Source: "{#ReleaseFilesPath}*.config"; DestDir: "{autoappdata}\{#AppName}"; Flags: ignoreversion
Source: "{#ResourcesFilesPath}Wizard.bmp"; DestDir: "{tmp}"; Flags: dontcopy
Source: "{#ResourcesFilesPath}WizardLarge.bmp"; DestDir: "{tmp}"; Flags: dontcopy

[Code]
var
  CustomDir: string;
  FinishedPage: TWizardPage;
  InfoLabel: TNewStaticText;

function IsDotNet48Installed: Boolean;
var
  ReleaseVersion: Cardinal;
begin
  Result := False;
  if RegKeyExists(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full') then
  begin
    if RegQueryDWordValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', ReleaseVersion) then
    begin
      Result := (ReleaseVersion >= 528040);
    end;
  end;
end;

function RegisterMainDLL: Boolean;
var
  RegAsmPath: String;
  ResultCode: Integer;
  RetryCount: Integer;
begin
  Result := False;
  RegAsmPath := ExpandConstant('{win}\Microsoft.NET\Framework64\v4.0.30319\regasm.exe');
  
  if not FileExists(RegAsmPath) then
  begin
    MsgBox('64-bit .NET Framework 4.8 not found. Please install it first.', mbError, MB_OK);
    Exit;
  end;

  // Try registration up to 3 times with delays
  RetryCount := 0;
  while RetryCount < 3 do
  begin
    if Exec(RegAsmPath, ExpandConstant('"{app}\{#MainDll}" /codebase'), '', 
       SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      if ResultCode = 0 then
      begin
        Result := True;
        Break;
      end
      else if RetryCount = 2 then  // Last attempt failed
      begin
        MsgBox('Registration failed after multiple attempts.' + #13#10 +
               'Please make sure no applications are using the DLL and try again.', 
               mbError, MB_OK);
      end;
    end;
    
    Sleep(2000);  // Wait 2 seconds before retrying
    RetryCount := RetryCount + 1;
  end;
end;

function InitializeSetup(): Boolean;
begin
  // Skip the "close applications" prompt entirely
  Result := True;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  AppDataPath: string;
begin
  AppDataPath := ExpandConstant('{userappdata}\{#AppName}');
  
  Result :=
  NewLine +
    Space + 'The installation now will:' + NewLine + NewLine +
    Space + Space + '• Install {#MainDll} to: ' + CustomDir + ';' + NewLine +
    Space + Space + '• Place the configuration file in: ' + AppDataPath + ';'  + NewLine +
    Space + Space + '• Register the DLL in system registry' + NewLine + '.'  + NewLine +
    
    Space + 'Click Install to proceed.';
end;


procedure InitializeWizard;
begin
  if not Is64BitInstallMode then
  begin
    MsgBox('This application requires a 64-bit Windows system.', mbError, MB_OK);
    Abort;
  end;

  if not IsDotNet48Installed then
  begin
    MsgBox('This application requires 64-bit .NET Framework 4.8. Please install it first.', mbError, MB_OK);
    Abort;
  end;

  // Create custom finished page
  FinishedPage := CreateCustomPage(wpFinished, 'Installation Complete', '');

  // Add information label
  InfoLabel := TNewStaticText.Create(FinishedPage);
  with InfoLabel do
  begin
    Parent := FinishedPage.Surface;
    Caption :=
      '' + #13#10#13#10 +
      'You can now right-click on any ebook file to start using {#AppName}.' + #13#10#13#10 +
      'Note: If the context menu entry is not visible yet:' + #13#10 +
      '' + #13#10#13#10 +
      '• Restart Windows Explorer (explorer.exe)' + #13#10 +
      '• Or log off and back on to your Windows session';
    Left := ScaleX(0);
    Top := ScaleY(0);
    Width := FinishedPage.SurfaceWidth;
    Height := ScaleY(120);
    AutoSize := False;
    WordWrap := True;
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  // This now handles the default directory page
  if CurPageID = wpSelectDir then
  begin
    if WizardForm.DirEdit.Text = '' then
    begin
      MsgBox('You must select an installation directory.', mbError, MB_OK);
      Result := False;
      Exit;
    end;
    
    CustomDir := WizardForm.DirEdit.Text;
    ForceDirectories(CustomDir);
  end;
  Result := True;
end;


procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    if not RegisterMainDLL then
    begin
      // Add registration failure note to the finished page
      InfoLabel.Caption := InfoLabel.Caption + #13#10#13#10 +
        'Warning: DLL registration failed.' + #13#10 +
        'For manual registration, run as Administrator:' + #13#10 +
        '"%windir%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" "' + 
        ExpandConstant('{app}\{#MainDll}') + '" /codebase';
    end;
  end;
end;
