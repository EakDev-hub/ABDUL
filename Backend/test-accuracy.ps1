# Test the accuracy endpoint
Write-Host "Testing Accuracy Endpoint..." -ForegroundColor Green
Write-Host ""

$body = @{
    question = "ลูกค้าอาขีพทหาร สามารถกู้แคมเปญพิเศษได้หรือไม่"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5001/api/accuracy/test" `
        -Method POST `
        -ContentType "application/json" `
        -Body $body

    Write-Host "Response:" -ForegroundColor Cyan
    $response | ConvertTo-Json -Depth 10
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "Test completed!" -ForegroundColor Green
