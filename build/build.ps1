<#
�ļ���build.ps1
��;�����ڴ����������nuget������
#>
$path = Get-Location

$baseDate=[datetime]"03/28/2019"
$currentDate=$(Get-Date)
$interval=New-TimeSpan -Start $baseDate -End $currentDate
$days=$interval.Days
$hours=$interval.Hours
#�汾������
#$version="$currentDate.Year-$days"
#$version="$(Get-Date -Format 'yyyy.M').1"
$version="0.2.0.$days" #"2022.2.3"
#��common.props�ļ�������

Write-Host -Object $days

foreach($line in Get-Content .\projects.txt) {
    $projectName= "..\$line\$line.csproj"
    Write-Host $projectName

    <# ������Ŀ #>
    if (Test-Path "..\$line\bin")
    {
        rm -Recurse -Force "..\$line\bin"
    }
    if (Test-Path "..\$line\obj")
    {
        rm -Recurse -Force "..\$line\obj"
    }

    <# ��� #>
    # $targetPath= "..\$line\bin\nupkgs"

    $targetPath="..\..\..\..\Packages\$line"  
    dotnet pack --configuration Release --output $targetPath  $projectName /p:Version=$version

    <# ������nuget������ #>
    # dotnet nuget push "$targetPath\$line.$version.nupkg"  -s https://api.nuget.org/v3/index.json --skip-duplicate 

    <# ������Ŀ #>
    if (Test-Path "..\$line\bin")
    {
        rm -Recurse -Force "..\$line\bin"
    }
    if (Test-Path "..\$line\obj")
    {
        rm -Recurse -Force "..\$line\obj"
    }
}
