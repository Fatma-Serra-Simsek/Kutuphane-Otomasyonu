# Renders all Mermaid (.mmd) diagrams in this folder to SVG and PNG under ./out
$ErrorActionPreference = "Stop"
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$out = Join-Path $here "out"
$config = Join-Path $here "puppeteer-config.json"
New-Item -ItemType Directory -Force -Path $out | Out-Null

$files = Get-ChildItem -Path $here -Filter "*.mmd"
if ($files.Count -eq 0) {
    Write-Error "No .mmd files found in $here"
}

foreach ($f in $files) {
    $base = [System.IO.Path]::GetFileNameWithoutExtension($f.Name)
    $svg = Join-Path $out "$base.svg"
    $png = Join-Path $out "$base.png"
    Write-Host "Rendering $($f.Name) -> svg, png"
    npx -y @mermaid-js/mermaid-cli -p $config -i $f.FullName -o $svg
    if ($LASTEXITCODE -ne 0) { throw "SVG render failed for $($f.Name)" }
    npx -y @mermaid-js/mermaid-cli -p $config -i $f.FullName -o $png
    if ($LASTEXITCODE -ne 0) { throw "PNG render failed for $($f.Name)" }
}

Write-Host "Done. Output: $out"
Get-ChildItem $out | Select-Object Name, Length
