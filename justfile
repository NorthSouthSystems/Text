import 'dotnet.justfile'

format: (format-solution "NorthSouthSystems.Text.slnx")

deploy: tools (publish "https://api.nuget.org/v3/index.json")
