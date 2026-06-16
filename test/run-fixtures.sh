#!/usr/bin/env bash
#
# Run hbs against each fixture in test/fixtures/ and diff against the committed
# golden output in expected/. Pass --regenerate to overwrite goldens after a
# deliberate template or env change.
#
# HBS_BIN must point to the binary to test (defaults to the local Release build).
set -uo pipefail

regenerate=false
if [[ "${1:-}" == "--regenerate" ]]; then
  regenerate=true
fi

repo_root=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)
HBS_BIN="${HBS_BIN:-$repo_root/src/Handlebars.conf/bin/Release/net10.0/hbs}"

# Allow pointing HBS_BIN at the managed dll for local runs where the apphost
# can't locate a .NET 10 runtime (e.g. macOS with system dotnet pinned to 8).
if [[ "$HBS_BIN" == *.dll ]]; then
  run_hbs=(dotnet "$HBS_BIN")
else
  if [[ ! -x "$HBS_BIN" ]]; then
    echo "hbs binary not found or not executable: $HBS_BIN" >&2
    echo "Build first (e.g. dotnet publish -c Release -r linux-x64 ...) or set HBS_BIN." >&2
    exit 1
  fi
  run_hbs=("$HBS_BIN")
fi

fail=0
for fixture in "$repo_root"/test/fixtures/*/; do
  name=$(basename "$fixture")
  echo "### $name"
  if ! (
    cd "$fixture"
    rm -rf out
    mkdir -p out
    set -a
    # shellcheck disable=SC1091
    source ./env.sh
    set +a
    "${run_hbs[@]}" --config config.yaml
    if $regenerate; then
      rm -rf expected
      mv out expected
      echo "  regenerated goldens"
    else
      diff -ruN expected out
    fi
  ); then
    echo "FAIL: $name" >&2
    fail=1
  else
    $regenerate || echo "OK: $name"
  fi
done

exit $fail
