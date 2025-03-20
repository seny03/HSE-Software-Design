#!/bin/bash
cd "$(dirname "$0")"
dotnet run << EOF
add-account Test_Account 1000
add-category Salary Income
add-category Food Expense
add-operation Income Test_Account 500 Salary Salary_for_March
add-operation Expense Test_Account 200 Food Groceries
edit-account Test_Account Updated_Account 1500
edit-category Salary Updated_Salary Income
edit-operation Income Updated_Account 500 Updated_Salary Income 600 Updated_description
report-category Updated_Account
delete-operation Expense Updated_Account 200 Food
delete-category Updated_Salary
delete-account Updated_Account
exit
EOF
