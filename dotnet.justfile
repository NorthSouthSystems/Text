[windows]
set shell := ["pwsh.exe", "-NoLogo", "-NoProfile", "-Command"]

[windows]
set script-interpreter := ["pwsh.exe", "-NoLogo", "-NoProfile", "-File"]

[unix]
set shell := ["bash", "-O", "inherit_errexit", "-euo", "pipefail", "-c"]

[unix]
set script-interpreter := ["bash", "-euo", "pipefail"]

# This is used for local development only.
[windows, script]
update-dotnet-sdk:
    $major = (dotnet --list-sdks | Select-Object -Last 1).Split('.')[0]
    winget install --exact --id "Microsoft.DotNet.SDK.$major" --source winget

# This is used for local development only.
[unix, script]
update-dotnet-sdk:
    major=$(dotnet --list-sdks | tail -n 1 | cut -d. -f1)
    curl -fsSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel "$major.0" --quality GA

# This is intended to be "explicitly" invoked; we are not calling it from other recipes.
tools:
    dotnet tool restore

# This is intended to be "explicitly" invoked; we are not calling it from other recipes.
certs:
    dotnet dev-certs https --trust
    dotnet dev-certs https --check --trust

# This is intended to be "explicitly" invoked; we are not calling it from other recipes.
update-package-versions:
    dotnet run --file scripts/UpdatePackageVersions.cs

restore:
    dotnet restore --locked-mode

restore-force:
    dotnet restore --force-evaluate

format-solution solution:
    dotnet jb cleanupcode "{{solution}}" \
        --profile="Built-in: Reformat & Apply Syntax Style" \
        --no-updates \
        --verbosity=ERROR

build configuration="Debug": restore
    dotnet build \
        --no-restore \
        --configuration {{configuration}}

test configuration="Debug": (build configuration)
    dotnet test \
        --no-build \
        --configuration {{configuration}}

[windows]
clean-coverage:
    foreach ($path in @('artifacts/coverage', 'artifacts/coverage-report')) { \
        if (Test-Path -LiteralPath $path) { \
            Remove-Item -LiteralPath $path -Recurse -Force -ErrorAction Stop \
        } \
    }

[unix]
clean-coverage:
    rm -rf -- \
        artifacts/coverage \
        artifacts/coverage-report

coverage configuration="Debug": clean-coverage (build configuration)
    dotnet test \
        --no-build \
        --configuration {{configuration}} \
        --coverage \
        --coverage-output-format cobertura \
        --results-directory artifacts/coverage
    dotnet reportgenerator \
        "-reports:artifacts/coverage/**/*.cobertura.xml" \
        "-targetdir:artifacts/coverage-report" \
        "-reporttypes:TextSummary;MarkdownSummaryGithub"

[windows]
coverage-summary configuration="Debug": (coverage configuration)
    Get-Content artifacts/coverage-report/Summary.txt

[unix]
coverage-summary configuration="Debug": (coverage configuration)
    cat artifacts/coverage-report/Summary.txt

[windows]
clean-pack:
    if (Test-Path -LiteralPath 'artifacts/packages') { \
        Remove-Item -LiteralPath 'artifacts/packages' -Recurse -Force -ErrorAction Stop \
    }

[unix]
clean-pack:
    rm -rf -- artifacts/packages

pack: clean-pack (test "Release")
    dotnet pack \
        --no-build \
        --configuration Release \
        --output artifacts/packages

publish source: pack
    dotnet nuget push \
        "artifacts/packages/*.nupkg" \
        --source "{{source}}"
