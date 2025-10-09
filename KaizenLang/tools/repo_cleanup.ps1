<#
Safe repo cleanup script (PowerShell)
Run this from the root of your repository (the folder that contains .git)

What it does:
- Stages and commits the updated .gitignore (already edited by me)
- Stops tracking common build artifacts (bin/, obj/, tools/*/bin, tools/*/obj) while keeping files on disk
- Removes tracked tool run outputs (tools/**/out.txt, err.txt, run-output.txt)
- Commits the removals with a clear message

IMPORTANT: This script does NOT rewrite git history. If you want to purge large files from history
use the BFG or git-filter-repo instructions at the end of this script (manual step; backup required).
#>

param(
    [switch]$DoIt,
    [switch]$Verbose
)

function Write-Info($msg) { Write-Host $msg }
function Write-DebugMsg($msg) { if ($Verbose) { Write-Host "[DBG] $msg" } }

Write-Info "Repo cleanup helper"
Write-Info "Dry run (preview) mode unless you pass -DoIt"

# 1) Preview tracked build artifacts
# Ensure we're inside a git repository
$gitInside = & git rev-parse --is-inside-work-tree 2>$null
if ($LASTEXITCODE -ne 0 -or $gitInside -ne 'true') {
    Write-Info "Error: This script must be run from inside a git repository root."
    exit 1
}

# Collect tracked files and make matches robust across path separators
$allFiles = & git ls-files 2>$null

Write-Info "\nPreview: tracked 'bin/' entries (matches up to first 200):"
$binMatches = $allFiles | Where-Object { $_ -match '(?:/|\\)bin(?:/|\\)' } | Select-Object -First 200
if ($binMatches) { $binMatches | ForEach-Object { Write-Info "  $_" } } else { Write-Info "  (none)" }

Write-Info "\nPreview: tracked 'obj/' entries (matches up to first 200):"
$objMatches = $allFiles | Where-Object { $_ -match '(?:/|\\)obj(?:/|\\)' } | Select-Object -First 200
if ($objMatches) { $objMatches | ForEach-Object { Write-Info "  $_" } } else { Write-Info "  (none)" }

Write-Info "\nPreview: tracked tool output files (out/err/run-output.txt) (first 200):"
$toolOut = $allFiles | Where-Object { $_ -match 'tools(?:/|\\).*(?:/|\\)(?:out|err|run-output)\.txt$' } | Select-Object -First 200
if ($toolOut) { $toolOut | ForEach-Object { Write-Info "  $_" } } else { Write-Info "  (none)" }

if (-not $DoIt) {
    Write-Info "\nRun with -DoIt to perform the untracking and commit. Example:`n    .\\tools\\repo_cleanup.ps1 -DoIt"
    exit 0
}

Write-Host "\n-- Executing: untrack build artifacts and tool outputs --"

try {
    # Stage .gitignore first (if present)
    if (Test-Path .gitignore) { Write-DebugMsg "Staging .gitignore"; & git add .gitignore }

    Write-DebugMsg "Untracking common build artifact folders (cached)..."
    & git rm -r --cached --ignore-unmatch bin obj 2>$null | Out-Null
    & git rm -r --cached --ignore-unmatch "src/*/bin" "src/*/obj" 2>$null | Out-Null
    & git rm -r --cached --ignore-unmatch "tools/*/bin" "tools/*/obj" 2>$null | Out-Null

    Write-DebugMsg "Untracking known tool output files..."
    & git rm --cached --ignore-unmatch "tools/**/out.txt" "tools/**/err.txt" "tools/**/run-output.txt" 2>$null | Out-Null

    # Check if there are staged changes to commit
    $staged = & git diff --cached --name-only
    if ($staged) {
        Write-Info "Staged changes detected (will commit):"
        $staged | ForEach-Object { Write-Info "  $_" }
        & git commit -m "chore: stop tracking build artifacts and tool outputs; update .gitignore"
        Write-Info "Committed changes to stop tracking artifacts."
    } else {
        Write-Info "No tracked build artifacts or tool outputs found to untrack. Nothing to commit."
    }
}
catch {
    Write-Host "Error while running git commands: $_"
    exit 2
}

Write-Output "Committed. If you want to purge these files from git history (optional), follow the BFG/git-filter-repo instructions below."
Write-Output "Committed. If you want to purge these files from git history (optional), follow the BFG/git-filter-repo instructions below."

$purgeHelp = @'
== Optional: purge files from history (DANGEROUS, rewrites history) ==
1) Backup your repo or ensure you have a clone elsewhere.
2) Install BFG Repo-Cleaner (https://rtyley.github.io/bfg-repo-cleaner/) or git-filter-repo.
3) Example BFG steps (run from outside your repo):
    git clone --mirror <repo-url> repo-mirror.git
    java -jar bfg.jar --delete-folders bin --delete-folders obj --delete-files out.txt --delete-files err.txt repo-mirror.git
    cd repo-mirror.git
    git reflog expire --expire=now --all
    git gc --prune=now --aggressive
    git push --force

Or use git-filter-repo with similar patterns. Do not proceed unless you understand force-push and its impact.
'@

Write-Output $purgeHelp

Write-Output "Repo cleanup script completed"
