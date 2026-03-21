#!/usr/bin/env bash
set -Eeuo pipefail

REPO_DIR="${REPO_DIR:-/root/mikuBot/MikuSharp}"
DEPLOY_DIR="${DEPLOY_DIR:-/root/mikuBot/MikuSharp/MikuSharp/bin/Release/net9.0/linux-x64/publish}"
SERVICE_NAME="${SERVICE_NAME:-hatsune-miku}"
LAVALINK_SERVICE_NAME="${LAVALINK_SERVICE_NAME:-lavalink}"
OUTPUT_FILE="${OUTPUT_FILE:-}"

redact_stream() {
  sed -E \
    -e 's#(https?://)[^[:space:]"]*:[^[:space:]"]*@#\1***:***@#g' \
    -e 's#(Token|token|Password|password|Secret|secret|ApiKey|apiKey|Webhook|webhook|ConnectionString|connectionString)[[:space:]]*[:=][[:space:]]*[^[:space:]]+#\1=***REDACTED***#g' \
    -e 's#("?(discordToken|discordTokenDev|discordBotListToken|weebShToken|youtubeApiToken|ksoftSiToken|MotionBotListToken|token|password|clientSecret|clientId|connectionString|DbConnectString)"?[[:space:]]*:[[:space:]]*)"[^"]*"#\1"***REDACTED***"#g' \
    -e 's#(postgres(ql)?://)[^[:space:]"]+#\1***REDACTED***#g' \
    -e 's#(user_session=)[^;[:space:]]+#\1***REDACTED***#g'
}

section() {
  printf '\n===== %s =====\n' "$1"
}

run_cmd() {
  local title="$1"
  shift

  section "$title"
  printf '$ %s\n' "$*"
  if "$@" 2>&1 | redact_stream; then
    :
  else
    local exit_code=$?
    printf '[command exited with status %s]\n' "$exit_code"
  fi
}

run_shell() {
  local title="$1"
  local command="$2"

  section "$title"
  printf '$ %s\n' "$command"
  if bash -lc "$command" 2>&1 | redact_stream; then
    :
  else
    local exit_code=$?
    printf '[command exited with status %s]\n' "$exit_code"
  fi
}

emit_report() {
  section "Snapshot Meta"
  printf 'Timestamp: %s\n' "$(date -Iseconds)"
  printf 'Host: %s\n' "$(hostname)"
  printf 'Repo dir: %s\n' "$REPO_DIR"
  printf 'Deploy dir: %s\n' "$DEPLOY_DIR"
  printf 'Service: %s\n' "$SERVICE_NAME"
  printf 'Lavalink service: %s\n' "$LAVALINK_SERVICE_NAME"

  run_cmd "Identity" whoami
  run_cmd "Working Directory" pwd
  run_cmd "Kernel" uname -a
  run_cmd "Hostnamectl" hostnamectl

  run_cmd "Service Status" systemctl status "$SERVICE_NAME" --no-pager
  run_cmd "Service Unit" systemctl cat "$SERVICE_NAME"
  run_cmd "Recent Service Logs" journalctl -u "$SERVICE_NAME" -n 200 --no-pager

  run_cmd "Lavalink Status" systemctl status "$LAVALINK_SERVICE_NAME" --no-pager
  run_cmd "Recent Lavalink Logs" journalctl -u "$LAVALINK_SERVICE_NAME" -n 120 --no-pager

  run_shell "Relevant Processes" "ps -ef | grep -i -E 'miku|dotnet|lavalink|java' | grep -v grep"
  run_cmd "Dotnet Info" dotnet --info

  run_cmd "Memory" free -h
  run_cmd "Disk" df -h
  run_shell "Process Limits" "ulimit -a"
  run_shell "Top Snapshot" "top -b -n 1 | head -40"

  run_shell "Listening Ports" "ss -tulpn | grep -E '2333|80|443|dotnet|java|miku' || true"

  if [[ -d "$REPO_DIR" ]]; then
    run_shell "Repo Git Branch" "cd '$REPO_DIR' && git branch --show-current"
    run_shell "Repo Git HEAD" "cd '$REPO_DIR' && git rev-parse HEAD"
    run_shell "Repo Git Status" "cd '$REPO_DIR' && git status --short"
    run_shell "Repo Git Remotes" "cd '$REPO_DIR' && git remote -v"
    run_shell "Repo Recent Commits" "cd '$REPO_DIR' && git log --oneline --decorate -n 20"
    run_shell "Repo Top-Level Listing" "cd '$REPO_DIR' && ls -la"
    run_shell "Repo Important Files" "cd '$REPO_DIR' && find . -maxdepth 2 -type f | sort"
    run_shell "Repo Config Preview" "cd '$REPO_DIR' && if [[ -f MikuSharp/config.json ]]; then sed -n '1,220p' MikuSharp/config.json; elif [[ -f config.json ]]; then sed -n '1,220p' config.json; else echo 'config.json not found'; fi"
    run_shell "Repo Log Files" "cd '$REPO_DIR' && find . -maxdepth 3 -type f \\( -name 'miku_log*.txt' -o -name '*.log' \\) | sort"
    run_shell "Repo Log Tail" "cd '$REPO_DIR' && latest_log=\$(find . -maxdepth 3 -type f \\( -name 'miku_log*.txt' -o -name '*.log' \\) | sort | tail -n 1); if [[ -n \"\${latest_log:-}\" ]]; then tail -n 200 \"\$latest_log\"; else echo 'no log file found'; fi"
  else
    section "Repo Missing"
    printf 'Repo directory does not exist: %s\n' "$REPO_DIR"
  fi

  if [[ -d "$DEPLOY_DIR" ]]; then
    run_shell "Deploy Listing" "cd '$DEPLOY_DIR' && ls -la"
    run_shell "Deploy File Tree" "cd '$DEPLOY_DIR' && find . -maxdepth 2 -type f | sort"
  else
    section "Deploy Missing"
    printf 'Deploy directory does not exist: %s\n' "$DEPLOY_DIR"
  fi
}

if [[ -n "$OUTPUT_FILE" ]]; then
  emit_report | tee "$OUTPUT_FILE"
else
  emit_report
fi
