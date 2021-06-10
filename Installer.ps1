$taskName = "UpdateWallpaperFromBing"
$description = "Update desktop wallpaper from bing"
$release = ".\bin\Release\net5.0\win-x64\publish"
$destination = "C:\Program Files\BingWallpaper"
$command = "Start-Process -FilePath '$destination\BingWallpaper.exe' -WindowStyle Hidden"

# Copy items
if ((Test-Path $destination) -eq $false) {
    New-Item $destination -ItemType "directory"
}
else
{
    Remove-Item "$destination\*"
}

Get-ChildItem -Path $release | Copy-Item -Destination $destination -Recurse -Force

# Unregister existed task
$TaskExists = Get-ScheduledTask | Where-Object {$_.TaskName -like $taskName}
if ($TaskExists) {
    Unregister-ScheduledTask -TaskName $taskName -Confirm:$false
}

# Register scheduledTask
$action = New-ScheduledTaskAction -Execute "powershell.exe" `
                                  -Argument "-Command `"$command`""

$trigger =  New-ScheduledTaskTrigger -Daily -At 10am

Register-ScheduledTask -Action $action `
                       -Trigger $trigger `
                       -TaskName $taskName `
                       -Description $description `
