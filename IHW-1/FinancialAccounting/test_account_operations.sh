#!/bin/bash
cd "$(dirname "$0")"
dotnet run << EOF
time
add-account Main_Account 1000
add-account Savings_Account 5000.50
add-category Salary Income
add-category Groceries Expense
add-category Entertainment Expense
add-operation Income Main_Account 2000 Salary Salary_for_March
add-operation Expense Main_Account 150 Groceries Grocery_shopping
add-operation Expense Main_Account 50 Entertainment Cinema_ticket
add-operation Expense Savings_Account 500 Entertainment Vacation_booking
time
report-balance Main_Account 2025-01-01 2025-03-31
time
export json data/data.json
exit
EOF
