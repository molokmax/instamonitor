cd ../../src/InstaMonitor.Api
dotnet publish -c release --self-contained -r linux-arm
# linux-arm64 linux-arm linux-x64
cd ../../tools/webapi
