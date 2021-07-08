#!/usr/bin/env bash

echo 'Deploy to DeployGate'

baseSelfUrl=https://tlf93.synology.me:6060

data=$(curl -m 5 "${baseSelfUrl}/data")

if [ "${data}" == '' ]; then
  echo 'Fetch timed out'
  curl \
    -m 5 \
    -X POST "${baseSelfUrl}/notify" \
    -d '{"type":"timeout"}'
  exit 0
fi

token=$(echo "${data}" | jq -r .token)
dist_key=$(echo "${data}" | jq -r .dist_key)
owner=$(echo "${data}" | jq -r .owner)

result=$(curl \
  -H "Authorization: token ${token}" \
  -F "file=@$2/build.ipa" \
  -F "message=Auto upload from Unity Cloud Build" \
  -F "distribution_key=${dist_key}" \
  "https://deploygate.com/api/users/${owner}/apps" | jq .)

is_error=$(echo "${result}" | jq -r .error)

if [ "$is_error" == 'false' ]; then
  version=$(echo "${result}" | jq -r .results.version_code)
  curl \
    -m 5 \
    -X POST "${baseSelfUrl}/notify" \
    -d "{\"type\":\"success\",\"version\":\"${version}\"}"
else
  message=$(echo "${result}" | jq -r .message)
  curl \
    -m 5 \
    -X POST "${baseSelfUrl}/notify" \
    -d "{\"type\":\"failed\",\"message\":\"${message}\"}"
fi
