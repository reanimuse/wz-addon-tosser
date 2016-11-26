#Increments the Build Number

$root = $PSScriptRoot

pushd

Set-Location $root
Set-Location ".."

$content = Get-Content "versionInfo.cs"

$newContent = $content | % { 
    $curLine = [string] $_

    if ($curLine.IndexOf("BuildVersion") -lt 0) {
        $curLine
    } else {
        $seg = $curLine.Substring(0, $curline.LastIndexOf(".") + 1);
        $ver = $curLine.Substring($seg.Length).trim(';', '"', ' ')

        if ($ver -eq "*") { $ver = "-1" }

        $ver = [int32]::Parse($ver) + 1;

        write-host "Setting Build Number to: $ver"

        $newLine = [string]::Format("{0}{1}"";", $seg, $ver);
        $newLine
    }
}

$newContent | Set-Content "versionInfo.cs"-Encoding UTF8 
popd

