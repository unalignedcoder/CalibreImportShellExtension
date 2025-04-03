; PREPROCESSOR DEFINITIONS
#define ReleaseFilesPath "..\ReleaseFiles\" 
#define ResourcesFilesPath "..\Resources\"  
#define AppName "CalibreImport"             
#define MainDll "CalibreImport.dll"         
#define AppIcon "MainAppIcon.ico"           

[Setup]
; Application metadata
AppName={#AppName}
AppVersion=0.0.7.6
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
UninstallDisplayIcon={app}\{#MainDll}
SetupIconFile={#ResourcesFilesPath}{#AppIcon}
UsedUserAreasWarning=no

; Output settings
OutputDir={#ReleaseFilesPath}
OutputBaseFilename={#AppName}Setup

; Compression settings
Compression=lzma
SolidCompression=yes

; System requirements
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

; Wizard appearance
WizardImageFile={#ResourcesFilesPath}Wizardlarge.bmp
WizardSmallImageFile={#ResourcesFilesPath}Wizard.bmp
WizardStyle=modern

; Installation behavior
RestartIfNeededByRun=False
CloseApplications=no

; Version information for the setup executable
VersionInfoCompany=Tuscoss
VersionInfoCopyright=Copyright © 2025 Tuscoss
VersionInfoProductName={#AppName} Setup
VersionInfoProductVersion=1.0.0.2
VersionInfoVersion=1.0.0.2

[Files]
Source: "{#ReleaseFilesPath}*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs; Check: Is64BitInstallMode
Source: "{#ReleaseFilesPath}*.config"; DestDir: "{userappdata}\{#AppName}"; Flags: ignoreversion; Check: DirExists(ExpandConstant('{userappdata}\{#AppName}')) or ForceDirectories(ExpandConstant('{userappdata}\{#AppName}'))
Source: "{#ResourcesFilesPath}Wizard.bmp"; DestDir: "{tmp}"; Flags: dontcopy
Source: "{#ResourcesFilesPath}WizardLarge.bmp"; DestDir: "{tmp}"; Flags: dontcopy

[UninstallRun]
; Unregister Main DLL
Filename: "{win}\Microsoft.NET\Framework64\v4.0.30319\regasm.exe"; \
    Parameters: "/unregister ""{app}\{#MainDll}"""; \
    Flags: runhidden; \
    RunOnceId: "Unregister_{#MainDll}"; \
    Check: Is64BitInstallMode

; Unregister SharpShell DLL
Filename: "{win}\Microsoft.NET\Framework64\v4.0.30319\regasm.exe"; \
    Parameters: "/unregister ""{app}\SharpShell.dll"""; \
    Flags: runhidden; \
    RunOnceId: "Unregister_SharpShell"; \
    Check: Is64BitInstallMode

[Code]
var
  CustomDir: string;
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
      else if RetryCount = 2 then
      begin
        MsgBox('Registration failed after multiple attempts.' + #13#10 +
               'Please make sure no applications are using the DLL and try again.', 
               mbError, MB_OK);
      end;
    end;
    
    Sleep(2000);
    RetryCount := RetryCount + 1;
  end;
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  AppDataPath: string;
begin
  AppDataPath := ExpandConstant('{userappdata}\{#AppName}');
  
  Result :=
  NewLine +
    Space + 'The installation will:' + NewLine + NewLine +
    Space + Space + '• Install the necessary DLL files to: ' + CustomDir + ';' + NewLine +
    Space + Space + '• Place the configuration file in: ' + AppDataPath + ';'  + NewLine +
    Space + Space + '• Register the DLLs in system registry.' + NewLine + '.'  + NewLine +    
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

  InfoLabel := TNewStaticText.Create(WizardForm);
  InfoLabel.Parent := WizardForm.FinishedPage;
  InfoLabel.Left := ScaleX(20);
  InfoLabel.Top := WizardForm.FinishedLabel.Top + WizardForm.FinishedLabel.Height + ScaleY(16);
  InfoLabel.Width := WizardForm.FinishedPage.ClientWidth - ScaleX(40);
  InfoLabel.Height := ScaleY(100);
  InfoLabel.AutoSize := False;
  InfoLabel.WordWrap := True;
  InfoLabel.Caption :=
    'You can now right-click on any ebook file to start using {#AppName}.' + #13#10#13#10 + #13#10#13#10 +
    'Note: If the context menu entry is not visible yet:' + #13#10 +
    '• Restart Windows Explorer (explorer.exe)' + #13#10 +
    '• Or log off and back on to your Windows session';
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  AppDataPath: string;
begin
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
    
    AppDataPath := ExpandConstant('{userappdata}\{#AppName}');
    if not DirExists(AppDataPath) then
      ForceDirectories(AppDataPath);
  end;
  Result := True;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    if not RegisterMainDLL then
    begin
      InfoLabel.Caption := InfoLabel.Caption + #13#10#13#10 +
        'Warning: DLL registration failed.' + #13#10 +
        'For manual registration, run as Administrator:' + #13#10 +
        '"%windir%\Microsoft.NET\Framework64\v4.0.30319\regasm.exe" "' + 
        ExpandConstant('{app}\{#MainDll}') + '" /codebase';
        
      InfoLabel.Height := InfoLabel.Height + ScaleY(60);
    end;
  end;
end;


procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  RegAsmPath: String;
  ResultCode: Integer;
begin
  if CurUninstallStep = usUninstall then
  begin
    RegAsmPath := ExpandConstant('{win}\Microsoft.NET\Framework64\v4.0.30319\regasm.exe');
    
    // Unregister Main DLL
    if FileExists(RegAsmPath) and FileExists(ExpandConstant('{app}\{#MainDll}')) then
    begin
      if not Exec(RegAsmPath, ExpandConstant('/unregister "{app}\{#MainDll}"'), '', 
         SW_HIDE, ewWaitUntilTerminated, ResultCode) or (ResultCode <> 0) then
      begin
        MsgBox('Failed to unregister main DLL. You may need to manually unregister it using:' + #13#10 +
               'regasm.exe /unregister "' + ExpandConstant('{app}\{#MainDll}') + '"', 
               mbError, MB_OK);
      end;
    end;
    
    // Unregister SharpShell DLL
    if FileExists(RegAsmPath) and FileExists(ExpandConstant('{app}\SharpShell.dll')) then
    begin
      if not Exec(RegAsmPath, ExpandConstant('/unregister "{app}\SharpShell.dll"'), '', 
         SW_HIDE, ewWaitUntilTerminated, ResultCode) or (ResultCode <> 0) then
      begin
        MsgBox('Failed to unregister SharpShell DLL. You may need to manually unregister it using:' + #13#10 +
               'regasm.exe /unregister "' + ExpandConstant('{app}\SharpShell.dll') + '"', 
               mbInformation, MB_OK);
      end;
    end;
  end;
end;
