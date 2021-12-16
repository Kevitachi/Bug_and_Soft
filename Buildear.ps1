$unityArguments = '-logFile log.txt -projectPath . -executeMethod Builds.build -quit -batchmode';
$unity = Start-Process -FilePath "C:\Program Files\Unity\Hub\Editor\2020.3.12f1\Editor\Unity.exe" -ArgumentList $unityArguments -PassThru; 
Start-Sleep -Seconds 3.0; Write-Host -NoNewLine 'Buildeando...'; while ((Get-WmiObject -Class Win32_Process | Where-Object {$_.ParentProcessID -eq $unity.Id -and $_.Name -ne 'VBCSCompiler.exe'}).count -gt 0) { Start-Sleep -Seconds 1.0; Write-Host -NoNewLine '.' }; if (!$unity.HasExited) { Wait-Process -Id $unity.Id };

$compress = @{
  Path = ".\Build\*"
  CompressionLevel = "Fastest"
  DestinationPath = ".\paco_an_adventure_begins.zip"
}
Compress-Archive @compress