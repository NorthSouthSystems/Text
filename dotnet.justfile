[windows]
set shell := ["pwsh.exe", "-NoLogo", "-NoProfile", "-Command"]

[unix]
set shell := ["bash", "-O", "inherit_errexit", "-euo", "pipefail", "-c"]

tools:
    dotnet tool restore

restore:
    dotnet restore --locked-mode

restore-force:
    dotnet restore --force-evaluate

format-solution solution: tools
    dotnet jb cleanupcode "{{solution}}" \
        --profile="Built-in: Reformat & Apply Syntax Style" \
        --no-updates \
        --verbosity=ERROR

build: restore
    dotnet build --no-restore

test: build
    dotnet test --no-build

coverage: tools build && coverage-summary
    dotnet test \
        --no-build \
        --coverage \
        --coverage-output-format cobertura \
        --results-directory artifacts/coverage
    dotnet reportgenerator \
        "-reports:artifacts/coverage/**/*.cobertura.xml" \
        "-targetdir:artifacts/coverage-report" \
        "-reporttypes:TextSummary"

[windows]
coverage-summary:
    Get-Content artifacts/coverage-report/Summary.txt

[unix]
coverage-summary:
    cat artifacts/coverage-report/Summary.txt
