#!/bin/bash
cd "$(dirname "$0")"
dotnet run << EOF
import json data/data.json
export yaml data/data.yaml
import yaml data/data.yaml
export csv data/data.csv
exit
EOF
