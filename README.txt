Generate server certs:

1. makecert -r -pe -n "CN=hostname" -sky exchange -sv C:\server.pvk C:\server.cer

2. pvk2pfx.exe -pvk C:\server.pvk -spc C:\server.cer -pfx C:\server.pfx

3. Then place .pfx file in same directory with exe.
