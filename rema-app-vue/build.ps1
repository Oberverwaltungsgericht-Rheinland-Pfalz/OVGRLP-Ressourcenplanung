npm run build
cd..
dotnet publish -p:PublishProfile=Rema.WebApi\Properties\PublishProfiles\FolderProfile.pubxml -c Release
MD .\rema-app-vue\dist\api\ -Force
Copy-Item -Path Rema.WebApi\bin\Release\net5.0\publish\*.* -Destination .\rema-app-vue\dist\api\
Rename-Item -Path .\rema-app-vue\dist\api\appsettings.json -NewName "appsettings.template.json"
Rename-Item -Path .\rema-app-vue\dist\api\web.config -NewName "web.template.config"

$version = (Get-Content rema-app-vue\package.json) -join "`n" | ConvertFrom-Json | Select -ExpandProperty "version"
$source = (Get-Location).Path + "\rema-app-vue\dist\"
$destination = (Get-Location).Path + "\Rema_Release_" + $version + ".app.zip"
 If(Test-path $destination) {Remove-item $destination}

Add-Type -assembly "system.io.compression.filesystem"
[io.compression.zipfile]::CreateFromDirectory($Source, $destination) 

