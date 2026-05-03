Set-Location -Path $PSScriptRoot
$name = 'czasowy - kontrola Windows'
$name2 = 'wnowa'
$data = powercfg /list | ForEach-Object {
    if ($_ -match 'GUID schematu zasilania: (.*) \((.*)\)' -or $_ -match 'Power Scheme GUID: (.*) \((.*)\)') {
        New-Object PSObject -Property @{
            GUID = $Matches[1]
            Name = $Matches[2]
        }
    }
}
$toSet = $data | Where-Object { $_.Name -like "*$name2*" }
if ($toSet -ne $null) {
    $restoreCommand = "powercfg -S " + $toSet.GUID
    Invoke-Expression -Command $restoreCommand
}
$toRemove = $data | Where-Object { $_.Name -like "*$name*" }
foreach ($scheme in $toRemove) {
    Write-Output "Usuwam plan zasilania o GUID: $($scheme.GUID)"
    $deleteCommand = "powercfg -D " + $scheme.GUID
    Invoke-Expression -Command $deleteCommand
}