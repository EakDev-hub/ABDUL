#!/bin/bash

# Test the accuracy endpoint
echo "Testing Accuracy Endpoint..."
echo ""

curl -X POST http://localhost:5001/api/accuracy/test \
  -H "Content-Type: application/json" \
  -d '{
    "question": "ลูกค้าอาขีพทหาร สามารถกู้แคมเปญพิเศษได้หรือไม่"
  }'

echo ""
echo ""
echo "Test completed!"
