#!/bin/bash
cd "$(dirname "$0")"
dotnet run << EOF
add-account Test_Account 1000
add-category Salary Income
add-category Food Expense
add-operation Income Test_Account 500 Salary
add-operation Expense Test_Account 200 Food
export csv data/data_test.csv
import csv data/data_test.csv
exit
EOF
