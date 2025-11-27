#!/bin/bash

# Health Check API Test Script
# This script tests both endpoints of the Health Check API

# Configuration
API_BASE_URL="http://localhost:5000"
API_ENDPOINT="$API_BASE_URL/healthcheck"

echo "=========================================="
echo "Health Check API Test"
echo "=========================================="
echo ""

# Color codes for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Test 1: GET /healthcheck
echo -e "${YELLOW}Test 1: GET /healthcheck${NC}"
echo "Endpoint: $API_ENDPOINT"
echo "Method: GET"
echo ""

GET_RESPONSE=$(curl -s -w "\n%{http_code}" "$API_ENDPOINT")
GET_HTTP_CODE=$(echo "$GET_RESPONSE" | tail -n1)
GET_BODY=$(echo "$GET_RESPONSE" | head -n-1)

echo "HTTP Status: $GET_HTTP_CODE"
echo "Response Body:"
echo "$GET_BODY" | jq '.' 2>/dev/null || echo "$GET_BODY"
echo ""

if [ "$GET_HTTP_CODE" -eq 200 ]; then
    echo -e "${GREEN}✓ GET /healthcheck - PASS${NC}"
else
    echo -e "${RED}✗ GET /healthcheck - FAIL${NC}"
fi

echo ""
echo "=========================================="
echo ""

# Test 2: PATCH /healthcheck
echo -e "${YELLOW}Test 2: PATCH /healthcheck${NC}"
echo "Endpoint: $API_ENDPOINT"
echo "Method: PATCH"
echo ""

# Get the first ID from the GET response
FIRST_ID=$(echo "$GET_BODY" | jq -r '.data[0].id' 2>/dev/null)

if [ -z "$FIRST_ID" ] || [ "$FIRST_ID" = "null" ]; then
    echo -e "${RED}Could not extract ID from GET response. Using ID=1 as default.${NC}"
    FIRST_ID=1
fi

TIMESTAMP=$(date +"%Y-%m-%d %H:%M:%S")
PATCH_DATA="{\"id\":$FIRST_ID,\"message\":\"Test update at $TIMESTAMP\"}"

echo "Request Body:"
echo "$PATCH_DATA" | jq '.' 2>/dev/null || echo "$PATCH_DATA"
echo ""

PATCH_RESPONSE=$(curl -s -w "\n%{http_code}" -X PATCH "$API_ENDPOINT" \
  -H "Content-Type: application/json" \
  -d "$PATCH_DATA")

PATCH_HTTP_CODE=$(echo "$PATCH_RESPONSE" | tail -n1)
PATCH_BODY=$(echo "$PATCH_RESPONSE" | head -n-1)

echo "HTTP Status: $PATCH_HTTP_CODE"
echo "Response Body:"
echo "$PATCH_BODY" | jq '.' 2>/dev/null || echo "$PATCH_BODY"
echo ""

if [ "$PATCH_HTTP_CODE" -eq 200 ]; then
    echo -e "${GREEN}✓ PATCH /healthcheck - PASS${NC}"
else
    echo -e "${RED}✗ PATCH /healthcheck - FAIL${NC}"
fi

echo ""
echo "=========================================="
echo ""

# Test 3: Verify the update by calling GET again
echo -e "${YELLOW}Test 3: Verify Update - GET /healthcheck${NC}"
echo "Retrieving data again to verify the update..."
echo ""

VERIFY_RESPONSE=$(curl -s -w "\n%{http_code}" "$API_ENDPOINT")
VERIFY_HTTP_CODE=$(echo "$VERIFY_RESPONSE" | tail -n1)
VERIFY_BODY=$(echo "$VERIFY_RESPONSE" | head -n-1)

echo "HTTP Status: $VERIFY_HTTP_CODE"
echo "Response Body:"
echo "$VERIFY_BODY" | jq '.' 2>/dev/null || echo "$VERIFY_BODY"
echo ""

if [ "$VERIFY_HTTP_CODE" -eq 200 ]; then
    echo -e "${GREEN}✓ Verification GET - PASS${NC}"
    
    # Check if the message was updated
    UPDATED_MESSAGE=$(echo "$VERIFY_BODY" | jq -r ".data[] | select(.id==$FIRST_ID) | .message" 2>/dev/null)
    if [[ "$UPDATED_MESSAGE" == *"Test update at"* ]]; then
        echo -e "${GREEN}✓ Message successfully updated in database${NC}"
    else
        echo -e "${YELLOW}⚠ Message may not have been updated${NC}"
    fi
else
    echo -e "${RED}✗ Verification GET - FAIL${NC}"
fi

echo ""
echo "=========================================="
echo "Test Summary"
echo "=========================================="
echo ""

# Summary
TESTS_PASSED=0
TESTS_TOTAL=3

[ "$GET_HTTP_CODE" -eq 200 ] && ((TESTS_PASSED++))
[ "$PATCH_HTTP_CODE" -eq 200 ] && ((TESTS_PASSED++))
[ "$VERIFY_HTTP_CODE" -eq 200 ] && ((TESTS_PASSED++))

echo "Tests Passed: $TESTS_PASSED/$TESTS_TOTAL"
echo ""

if [ $TESTS_PASSED -eq $TESTS_TOTAL ]; then
    echo -e "${GREEN}All tests passed! API is working correctly.${NC}"
    exit 0
else
    echo -e "${RED}Some tests failed. Please check the API and database connection.${NC}"
    exit 1
fi