Copy-Item -Path ".\bin\Release\net472" -Destination "C:\Program Files (x86)\BingWallpaper" -Recurse -Force
$action = New-ScheduledTaskAction -Execute "C:\Program Files (x86)\BingWallpaper\BingWallpaper.exe"
$trigger =  New-ScheduledTaskTrigger -Daily -At 8am
Register-ScheduledTask -Action $action -Trigger $trigger -TaskName "UpdateWallpaperFromBing" -Description "Update desktop wallpaper from bing"

