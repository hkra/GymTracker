FROM microsoft/dotnet:runtime

COPY ./bin/release/netcoreapp1.1/publish /app
WORKDIR /app
EXPOSE 5000/tcp

CMD ["dotnet", "GymTracker.dll"]
