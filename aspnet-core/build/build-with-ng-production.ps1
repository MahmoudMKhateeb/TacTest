
$todaysDate = $((Get-Date).ToString('yyyy-MM-dd'))
$buildPath = "c:\builds"
$outputFolder = Join-Path $buildPath $todaysDate
$buildFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $buildFolder "../"
$webHostFolder = Join-Path $slnFolder "src/TACHYON.Web.Host"
$ngFolder = Join-Path $buildFolder "../../angular"
$iisServerPath = "C:\inetpub\wwwroot\Tachyon"
$frontSiteName = "Default Web Site"
$backSiteName = "TachyonProdCore"



 
 function buildProject(){
      ## CLEAR ######################################################################

     Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
     New-Item -Path $outputFolder -ItemType Directory

     ## RESTORE NUGET PACKAGES #####################################################

     Set-Location $slnFolder
     dotnet restore

     ## PUBLISH WEB HOST PROJECT ###################################################

     Set-Location $webHostFolder
     dotnet publish /p:EnvironmentName=Development --output (Join-Path $outputFolder "Host")


     ########Build Front###################
     
      Set-Location $ngFolder
      & yarn
      & npm run publish
      Copy-Item (Join-Path $ngFolder "dist") (Join-Path $outputFolder "ng/") -Recurse | out-null

 }


 ## Move the Build From the C:/Build to IIS 
 ## and Check if its a complete Build Then Edit IIS Website Paths
 ## then Restart the Server
 function UpdateIISSettings(){
    ## Coppy The Build That Was Created In Todays Date To IIS 
    #Checking if outputFolder folder exists
    if ((Test-Path $outputFolder) -and (Test-Path (Join-Path $outputFolder "Host")) -and (Test-Path (Join-Path $outputFolder "ng" ))) {
        echo "Build Successed"
        Copy-Item -Path $outputFolder -Destination $iisServerPath -Recurse | out-null
        Import-Module WebAdministration
        #change IIS FrontEnd Path and BakEnd paths
        Set-ItemProperty IIS:\Sites\$frontSiteName -name physicalPath -value (Join-Path $iisServerPath "$todaysDate\ng" )
        Set-ItemProperty IIS:\Sites\$backSiteName -name physicalPath -value (Join-Path $iisServerPath "$todaysDate\Host" )
        Stop-WebSite $frontSiteName 
        Stop-WebSite $backSiteName
        Start-WebSite $frontSiteName 
        Start-WebSite $backSiteName
    }else {
    ##stop and Dont Procceed
    echo "[Error] - Cant Find Build Path \n"
    exit 1
    }
 
 }


 buildProject
 UpdateIISSettings

