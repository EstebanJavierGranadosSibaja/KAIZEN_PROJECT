Repo cleanup workflow

Run the helper script `tools/repo_cleanup.ps1` to untrack compiled artifacts and tool outputs. This is the safe option and will not rewrite git history.

Recommended steps:

1. Review preview output:
   .\tools\repo_cleanup.ps1

2. Execute the untracking and commit:
   .\tools\repo_cleanup.ps1 -DoIt

3. Push the commit:
   git push origin HEAD

Optional: To purge artifacts from git history (reduces repo size) use BFG or git-filter-repo. This rewrites history and requires force-push and coordination.

Note: I removed some helper scripts from `tools/backups/` that appeared to be simple local helpers. If you want to keep them, restore from your local copy or let me move them to `docs/` instead.
