ARG PARENT_VERSION=1.7.0-dotnet8.0

# Development
FROM defradigital/dotnetcore-development:${PARENT_VERSION} AS development
ARG PARENT_VERSION
LABEL uk.gov.defra.ffc.parent-image=defradigital/dotnetcore-development:${PARENT_VERSION}

EXPOSE 80
EXPOSE 443

RUN mkdir -p /home/dotnet/src/AiFindDotnetDemoApi/
WORKDIR /home/dotnet/src
COPY --chown=dotnet:dotnet ./AiFindDotnetDemoApi/*.csproj ./AiFindDotnetDemoApi/
RUN dotnet restore "./AiFindDotnetDemoApi/AiFindDotnetDemoApi.csproj"

COPY --chown=dotnet:dotnet ./AiFindDotnetDemoApi/ ./AiFindDotnetDemoApi/

RUN chown -R dotnet:dotnet /home/dotnet

WORKDIR /home/dotnet/src/AiFindDotnetDemoApi
RUN dotnet publish -c Release -o /home/dotnet/out

ARG PORT=8085
ENV PORT ${PORT}
EXPOSE ${PORT}
ENTRYPOINT dotnet watch --project AiFindDotnetDemoApi.csproj run --urls "http://*:${PORT}" --non-interactive

FROM defradigital/dotnetcore:${PARENT_VERSION} AS production
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ARG PARENT_VERSION
LABEL uk.gov.defra.ffc.parent-image=defradigital/dotnetcore:${PARENT_VERSION}
COPY --from=development /home/dotnet/out/ ./
ARG PORT=8085
ENV ASPNETCORE_URLS http://*:${PORT}
EXPOSE ${PORT}

ENTRYPOINT ["dotnet", "AiFindDotnetDemoApi.dll"]
