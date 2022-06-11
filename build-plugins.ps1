Remove-Item "./publish/plugins" -Force -Recurse -ErrorAction SilentlyContinue

Get-ChildItem -Path '.' -Filter 'manifest.json' -Depth 1 | ForEach-Object {
    $dirName = $_.Directory.Name
    "Build plugin into ../publish/plugins/${dirName}"
    Set-Location $_.DirectoryName
    dotnet restore --ignore-failed-sources
    dotnet publish -c Release -o "../publish/plugins/${dirName}" --no-restore
    Set-Location .. 
}