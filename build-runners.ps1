Remove-Item "./publish/runners" -Force -Recurse -ErrorAction SilentlyContinue

Get-ChildItem -Path '.' -Filter 'runner.manifest.json' -Depth 1 | ForEach-Object {
    $dirName = $_.Directory.Name
    "Build runner into ../publish/runners/${dirName}"
    Set-Location $_.DirectoryName
    dotnet restore --ignore-failed-sources
    dotnet publish -c Release -o "../publish/runners/${dirName}" --no-restore
    Set-Location .. 
}