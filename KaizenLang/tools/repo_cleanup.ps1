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
    [switch]$DoIt
)

Write-Host "Repo cleanup helper"
Write-Host "Dry run mode (no changes) unless you pass -DoIt"

# 1) Preview tracked build artifacts
Write-Host "\nTracked bin/ or obj/ entries (preview):"
git ls-files --full-name | Select-String -Pattern '\b(bin|obj)\\' | Select-Object -First 200

Write-Host "\nTracked tool output files (preview):"
git ls-files | Select-String -Pattern 'tools/.*/(out|err|run-output)\.txt' | Select-Object -First 200

if (-not $DoIt) {
    Write-Host "\nRun with -DoIt to perform the untracking and commit. Example:`n    .\\tools\\repo_cleanup.ps1 -DoIt`"
    exit 0
}

Write-Host "\n-- Executing: untrack build artifacts and tool outputs --"

# Stage .gitignore first (if present)
git add .gitignore

# Remove tracked artifacts from index but keep files on disk
git rm -r --cached --ignore-unmatch bin obj
# Also untrack project-level bin/obj
git rm -r --cached --ignore-unmatch src/*/bin src/*/obj
# Tools build outputs
git rm -r --cached --ignore-unmatch tools/*/bin tools/*/obj

# Untrack specific tool outputs if previously committed
git rm --cached --ignore-unmatch tools/**/out.txt tools/**/err.txt tools/**/run-output.txt

# Commit changes
git commit -m "chore: stop tracking build artifacts and tool outputs; update .gitignore"

Write-Host "Committed. If you want to purge these files from git history (optional), follow the BFG/git-filter-repo instructions below."
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

Write-Output "Repo cleanup script completed."
