Get-ChildItem -Path '.' -Filter 'Dockerfile' -Depth 1 | ForEach-Object {
    $imageName = $_.Directory.Name.Replace(".", "_").ToLower()
    $fullPath = $_.Directory.FullName;
    $parentPath = $_.Directory.Parent.FullName;
    "Build docker image ${imageName}" 
    "Build Path ${fullPath}"
    
    docker build -f "${fullPath}\Dockerfile" --force-rm -t $imageName "$parentPath"
    #Set-Location $_.DirectoryName
    #docker build -f "C:\Users\susch\source\repos\WarFabrik.Clone\BotMaster.YouTube\Dockerfile" --force-rm -t botmaster_twitch "C:\Users\susch\source\repos\WarFabrik.Clone"
    #dotnet restore --ignore-failed-sources
    #dotnet publish -c Release -o "../publish/plugins/${dirName}" --no-restore
    #Set-Location .. 
}