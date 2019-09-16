# escape=`
FROM microsoft/windowsservercore AS downloader
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

ENV ES_VERSION="EventStore-OSS-Win-v5.0.2" `
    ES_HOME="C:\eventstore"

RUN Invoke-WebRequest "https://eventstore.org/downloads/EventStore-OSS-Win-v$($env:ES_VERSION).zip" -OutFile 'eventstore.zip' -UseBasicParsing; `
    Expand-Archive eventstore.zip -DestinationPath $env:ES_HOME ;

# windowsservercore
FROM microsoft/windowsservercore
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop';"]

ENV ES_VERSION="EventStore-OSS-Win-v5.0.2" `
    ES_HOME="C:\eventstore"

EXPOSE 1113 2113

# Data Directory
RUN New-Item -Path Data -ItemType Directory ; `
    New-Item -Path Logs -ItemType Directory

WORKDIR $ES_HOME
COPY --from=downloader C:\eventstore\ .

# Run Service
SHELL ["cmd", "/S", "/C"]
CMD C:\eventstore\EventStore.ClusterNode.exe --db /Data --log /Logs --ext-ip 0.0.0.0 --ext-http-prefixes "http://*:2113/"