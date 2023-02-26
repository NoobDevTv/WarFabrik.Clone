$jobs = @()

Get-ChildItem -Path '.' -Filter 'Dockerfile' -Depth 1 | ForEach-Object {
    $imageName = $_.Directory.Name.Replace(".", "_").ToLower()
    $fullPath = $_.Directory.FullName;
    $parentPath = $_.Directory.Parent.FullName;
    "Build docker image ${imageName}" 
    "Build Path ${fullPath}"
    #$job = Start-Job -ScriptBlock {
    
        docker build -f "${fullPath}\Dockerfile" --force-rm -t $imageName "$parentPath"
    #}
    #$jobs += $job
    #Set-Location $_.DirectoryName
    #docker build -f "C:\Users\susch\source\repos\WarFabrik.Clone\BotMaster.YouTube\Dockerfile" --force-rm -t botmaster_twitch "C:\Users\susch\source\repos\WarFabrik.Clone"
    #dotnet restore --ignore-failed-sources
    #dotnet publish -c Release -o "../publish/plugins/${dirName}" --no-restore
    #Set-Location .. 
}
# Warte auf das Ende aller Jobs
while ($jobs.State -contains "Running") {
    Start-Sleep -Milliseconds 100
}
# Sammle die Ergebnisse der Jobs
$results = foreach ($job in $jobs) {
    Receive-Job $job
}
