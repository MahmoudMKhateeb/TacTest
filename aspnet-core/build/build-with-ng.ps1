# COMMON PATHS

$buildFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $buildFolder "../"
$outputFolder = "C:\inetpub\wwwroot\Tachyon\outputs"
$webHostFolder = Join-Path $slnFolder "src/TACHYON.Web.Host"
#$webPublicFolder = Join-Path $slnFolder "src/TACHYON.Web.Public"
$ngFolder = Join-Path $buildFolder "../../angular"

## CLEAR ######################################################################

Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
New-Item -Path $outputFolder -ItemType Directory

## RESTORE NUGET PACKAGES #####################################################

Set-Location $slnFolder
dotnet restore

## PUBLISH WEB HOST PROJECT ###################################################

Set-Location $webHostFolder
dotnet publish /p:EnvironmentName=Development --output (Join-Path $outputFolder "Host")

## PUBLISH WEB PUBLIC PROJECT ###################################################

#Set-Location $webPublicFolder
#dotnet publish --output (Join-Path $outputFolder "Public") --configuration Release

# Change Public configuration
#$publicConfigPath = Join-Path $outputFolder "Public/appsettings.Staging.json"
#(Get-Content $publicConfigPath) -replace "9903", "9902" | Set-Content $publicConfigPath

## PUBLISH ANGULAR UI PROJECT #################################################

Set-Location $ngFolder
& yarn
& npm run publish
Copy-Item (Join-Path $ngFolder "dist") (Join-Path $outputFolder "ng/") -Recurse
#Copy-Item (Join-Path $ngFolder "Dockerfile") (Join-Path $outputFolder "ng")

# Change UI configuration
$ngConfigPath = Join-Path $outputFolder "ng/assets/appconfig.production.json"
$ngConfigPathBackup = "C:/Users/Islam/Desktop/iis_application-files-dev/appconfig.production.json"
$ngWbfile = "C:/Users/Islam/Desktop/iis_application-files-dev/web.config"

$hostConfigPath = Join-Path $outputFolder "Host/appsettings.json"
$hostConfigPathBackup = "C:/Users/Islam/Desktop/iis_application-files-dev/appsettings.json"

Remove-Item $hostConfigPath -Force -Recurse -ErrorAction Ignore
Copy-Item ($hostConfigPathBackup) (Join-Path $outputFolder "Host") -Recurse


Remove-Item $ngConfigPath -Force -Recurse -ErrorAction Ignore
Copy-Item ($ngConfigPathBackup) (Join-Path $outputFolder "ng/assets") -Recurse
Copy-Item ($ngWbfile) (Join-Path $outputFolder "ng/") -Recurse

#(Get-Content $ngConfigPath) -replace "22742", "9945" | Set-Content $ngConfigPath
#(Get-Content $ngConfigPath) -replace "4200", "80" | Set-Content $ngConfigPath
#(Get-Content $ngConfigPath) -replace "http", "https" | Set-Content $ngConfigPath
#(Get-Content $ngConfigPath) -replace "localhost", "dev.tachyonhub.com" | Set-Content $ngConfigPath

## CREATE DOCKER IMAGES #######################################################

# Host
#Set-Location (Join-Path $outputFolder "Host")

#docker rmi zero/host -f
#docker build -t zero/host .

# Public
#Set-Location (Join-Path $outputFolder "Public")

#docker rmi zero/public -f
#docker build -t zero/public .

# Angular UI
#Set-Location (Join-Path $outputFolder "ng")

#docker rmi zero/ng -f
#docker build -t zero/ng .

## DOCKER COMPOSE FILES #######################################################

#Copy-Item (Join-Path $slnFolder "docker/ng/*.*") $outputFolder

## FINALIZE ###################################################################

#Set-Location $outputFolder