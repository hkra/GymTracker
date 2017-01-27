FROM microsoft/dotnet:runtime

COPY /dist /app
WORKDIR /app
EXPOSE 5000/tcp

CMD ["dotnet", "run", "--server.urls", "http://*:5000"]
