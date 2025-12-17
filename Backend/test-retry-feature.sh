#!/bin/bash
# Test script for retry feature with temperature 0.7
# Tests both standard and batch endpoints with questions that might trigger retry

BASE_URL="http://localhost:5001"

echo "=== Testing Retry Feature with Temperature 0.7 ==="
echo ""

# Test 1: Single question that might not have exact answer
echo "Test 1: Single question (might trigger retry)"
curl -s -X POST "$BASE_URL/api/accuracy/test" \
  -H "Content-Type: application/json" \
  -d '{"question":"ค่าธรรมเนียมการโอนเงินระหว่างธนาคารเท่าไหร่?"}' | jq '.'
echo ""

# Test 2: Question with exact answer (should not trigger retry)
echo "Test 2: Question with exact answer (should NOT trigger retry)"
curl -s -X POST "$BASE_URL/api/accuracy/test" \
  -H "Content-Type: application/json" \
  -d '{"question":"ดอกเบี้ยสินเชื่อรถยนต์เท่าไหร่?"}' | jq '.'
echo ""

# Test 3: Batch test with mixed questions
echo "Test 3: Batch test (mixed questions)"
curl -s -X POST "$BASE_URL/api/accuracy/test-batch" \
  -H "Content-Type: application/json" \
  -d '{
    "questions": [
      "ดอกเบี้ยสินเชื่อรถยนต์เท่าไหร่?",
      "ค่าธรรมเนียมการโอนเงินเท่าไหร่?",
      "วงเงินกู้สูงสุดเท่าไหร่?",
      "ค่าปรับชำระล่าช้าเท่าไหร่?"
    ]
  }' | jq '.'
echo ""

echo "=== Test Complete ==="
