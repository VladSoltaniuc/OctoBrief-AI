#!/bin/bash

# Test all topic+country combinations for source selection
# This script uses a short timeout and just checks if sources are found

BASE_URL="http://localhost:5000/api/brief/preview"
TIMEOUT=120  # 2 minutes per request

countries=("global" "usa" "uk" "canada" "germany" "france" "italy" "spain" "romania" "poland")
topics=("all" "ai" "media" "health" "climate" "politics")

echo "=== OctoBrief API Source Validation ==="
echo "Testing ${#countries[@]} countries × ${#topics[@]} topics = $((${#countries[@]} * ${#topics[@]})) combinations"
echo "Timeout per request: ${TIMEOUT}s"
echo ""

results_file="/tmp/octobrief_test_results.txt"
> "$results_file"

for country in "${countries[@]}"; do
    echo ""
    echo "============================================"
    echo "COUNTRY: $country"
    echo "============================================"
    
    for topic in "${topics[@]}"; do
        echo -n "  $topic: "
        
        # Make API request and capture response
        response=$(curl -s --max-time $TIMEOUT "$BASE_URL" \
            -H "Content-Type: application/json" \
            -d "{\"topic\":\"$topic\",\"country\":\"$country\"}" 2>&1)
        
        # Check for curl errors
        if [[ $? -ne 0 ]]; then
            echo "❌ TIMEOUT/ERROR"
            echo "$country|$topic|TIMEOUT" >> "$results_file"
            continue
        fi
        
        # Check if response contains "Found X sources"
        sources_found=$(echo "$response" | grep -oP 'Found \d+ sources' | head -1)
        
        if [[ -z "$sources_found" ]]; then
            # Check for news count
            news_count=$(echo "$response" | grep -oP '"newsCount":\s*\d+' | grep -oP '\d+' | head -1)
            
            if [[ -n "$news_count" ]]; then
                echo "✓ $news_count news items"
                echo "$country|$topic|SUCCESS|$news_count" >> "$results_file"
            else
                echo "⚠️  Response issue (check logs)"
                echo "$country|$topic|ISSUE" >> "$results_file"
            fi
        else
            echo "✓ $sources_found"
            echo "$country|$topic|SUCCESS" >> "$results_file"
        fi
    done
done

echo ""
echo "=== SUMMARY ==="
echo "Results saved to: $results_file"
echo ""
echo "Success count: $(grep -c 'SUCCESS' $results_file)"
echo "Timeout count: $(grep -c 'TIMEOUT' $results_file)"
echo "Issue count: $(grep -c 'ISSUE' $results_file)"
