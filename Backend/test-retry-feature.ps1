# Test script for retry feature with temperature 0.7
# Tests both standard and batch endpoints with questions that might trigger retry

$baseUrl = "http://localhost:5001"

Write-Host "=== Testing Retry Feature with Temperature 0.7 ===" -ForegroundColor Cyan
Write-Host ""

# Test 1: Single question that might not have exact answer
Write-Host "Test 1: Single question (might trigger retry)" -ForegroundColor Yellow
$body1 = @{
    question = "ค่าธรรมเนียมการโอนเงินระหว่างธนาคารเท่าไหร่?"
} | ConvertTo-Json

try {
    $response1 = Invoke-RestMethod -Uri "$baseUrl/api/accuracy/test" `
        -Method Post `
        -ContentType "application/json" `
        -Body $body1
    
    Write-Host "Answer: $($response1.answer)" -ForegroundColor Green
    Write-Host "Is Suggestion: $($response1.isSuggestion)" -ForegroundColor $(if ($response1.isSuggestion) { "Magenta" } else { "White" })
    Write-Host "Temperature: $($response1.temperature)" -ForegroundColor $(if ($response1.temperature -eq 0.7) { "Magenta" } else { "White" })
    Write-Host ""
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
}

# Test 2: Question with exact answer (should not trigger retry)
Write-Host "Test 2: Question with exact answer (should NOT trigger retry)" -ForegroundColor Yellow
$body2 = @{
    question = "ดอกเบี้ยสินเชื่อรถยนต์เท่าไหร่?"
} | ConvertTo-Json

try {
    $response2 = Invoke-RestMethod -Uri "$baseUrl/api/accuracy/test" `
        -Method Post `
        -ContentType "application/json" `
        -Body $body2
    
    Write-Host "Answer: $($response2.answer)" -ForegroundColor Green
    Write-Host "Is Suggestion: $($response2.isSuggestion)" -ForegroundColor $(if ($response2.isSuggestion) { "Magenta" } else { "White" })
    Write-Host "Temperature: $($response2.temperature)" -ForegroundColor $(if ($response2.temperature -eq 0.7) { "Magenta" } else { "White" })
    Write-Host ""
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
}

# Test 3: Batch test with mixed questions
Write-Host "Test 3: Batch test (mixed questions)" -ForegroundColor Yellow
$body3 = @{
    questions = @(
        "ดอกเบี้ยสินเชื่อรถยนต์เท่าไหร่?",
        "ค่าธรรมเนียมการโอนเงินเท่าไหร่?",
        "วงเงินกู้สูงสุดเท่าไหร่?",
        "ค่าปรับชำระล่าช้าเท่าไหร่?"
    )
} | ConvertTo-Json

try {
    $response3 = Invoke-RestMethod -Uri "$baseUrl/api/accuracy/test-batch" `
        -Method Post `
        -ContentType "application/json" `
        -Body $body3
    
    Write-Host "Total Questions: $($response3.totalQuestions)" -ForegroundColor Cyan
    Write-Host "Success Count: $($response3.successCount)" -ForegroundColor Cyan
    Write-Host ""
    
    foreach ($result in $response3.results) {
        Write-Host "[$($result.index)] $($result.question)" -ForegroundColor White
        Write-Host "    Answer: $($result.answer.Substring(0, [Math]::Min(100, $result.answer.Length)))..." -ForegroundColor Green
        Write-Host "    Is Suggestion: $($result.isSuggestion)" -ForegroundColor $(if ($result.isSuggestion) { "Magenta" } else { "White" })
        Write-Host "    Temperature: $($result.temperature)" -ForegroundColor $(if ($result.temperature -eq 0.7) { "Magenta" } else { "White" })
        Write-Host ""
    }
    
    # Summary
    $suggestionCount = ($response3.results | Where-Object { $_.isSuggestion }).Count
    Write-Host "Summary:" -ForegroundColor Cyan
    Write-Host "  - Exact answers (temp=0.2): $($response3.totalQuestions - $suggestionCount)" -ForegroundColor White
    Write-Host "  - Suggestions (temp=0.7): $suggestionCount" -ForegroundColor Magenta
    
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Test Complete ===" -ForegroundColor Cyan
