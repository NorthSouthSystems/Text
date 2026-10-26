import 'dotnet.justfile'

format: (format-solution "NorthSouthSystems.Text.slnx")

integrate: (coverage-summary "Release")

deploy: (publish "https://api.nuget.org/v3/index.json")
