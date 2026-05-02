$in = "E:\SMT\1.mp4"      # 👈 change to your actual file name
$out = "E:\SMT\chunks1\chunk"  # 👈 output folder + prefix
$size = 100MB

# create chunks folder if not exists
if (!(Test-Path "E:\SMT\chunks")) {
    New-Item -ItemType Directory -Path "E:\SMT\chunks1"
}

$fs = [System.IO.File]::OpenRead($in)
$buffer = New-Object byte[] $size
$i = 1

while(($read = $fs.Read($buffer,0,$size)) -gt 0){
    [System.IO.File]::WriteAllBytes("$out$i.part",$buffer[0..($read-1)])
    $i++
}

$fs.Close()