# Test parallel KB endpoint 3 times and compare answers
# Usage: .\test-parallel-comparison.ps1

$baseUrl = "http://localhost:5001"
$endpoint = "$baseUrl/api/accuracy/test-parallel"

# Test question
$question = "ดอกเบี้ยสินเชื่อรถยนต์เท่าไหร่?"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Parallel KB Test - 3 Runs Comparison" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Question: $question" -ForegroundColor Yellow
Write-Host ""

$results = @()

# Run 3 times
for ($i = 1; $i -le 3; $i++) {
    Write-Host "Run #$i" -ForegroundColor Green
    Write-Host "--------------------------------------" -ForegroundColor Gray
    
    $body = @{
        question = $question
    } | ConvertTo-Json
    
    try {
        $startTime = Get-Date
        $response = Invoke-RestMethod -Uri $endpoint `
            -Method Post `
            -ContentType "application/json" `
            -Body $body
        $endTime = Get-Date
        $duration = ($endTime - $startTime).TotalMilliseconds
        
        Write-Host "✓ Success" -ForegroundColor Green
        Write-Host "Response Time: $([math]::Round($duration, 0))ms" -ForegroundColor Cyan
        Write-Host "Selected Strategy: $($response.selectedStrategy)" -ForegroundColor Cyan
        Write-Host "Selection Score: $($response.selectionScore)" -ForegroundColor Cyan
        Write-Host "Candidates Evaluated: $($response.candidatesEvaluated)" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Answer:" -ForegroundColor Yellow
        Write-Host $response.answer -ForegroundColor White
        Write-Host ""
        
        $results += @{
            Run = $i
            Success = $true
            Answer = $response.answer
            Strategy = $response.selectedStrategy
            Score = $response.selectionScore
            Candidates = $response.candidatesEvaluated
            Duration = [math]::Round($duration, 0)
        }
    }
    catch {
        Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        
        $results += @{
            Run = $i
            Success = $false
            Error = $_.Exception.Message
        }
    }
    
    if ($i -lt 3) {
        Write-Host ""
    }
}

# Comparison Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "COMPARISON SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if all answers are the same
$successfulResults = $results | Where-Object { $_.Success -eq $true }

if ($successfulResults.Count -eq 3) {
    $answer1 = $successfulResults[0].Answer
    $answer2 = $successfulResults[1].Answer
    $answer3 = $successfulResults[2].Answer
    
    $allSame = ($answer1 -eq $answer2) -and ($answer2 -eq $answer3)
    
    if ($allSame) {
        Write-Host "✓ All 3 answers are IDENTICAL" -ForegroundColor Green
    }
    else {
        Write-Host "⚠ Answers are DIFFERENT" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Differences:" -ForegroundColor Yellow
        
        if ($answer1 -ne $answer2) {
            Write-Host "  • Run 1 vs Run 2: Different" -ForegroundColor Red
        }
        if ($answer1 -ne $answer3) {
            Write-Host "  • Run 1 vs Run 3: Different" -ForegroundColor Red
        }
        if ($answer2 -ne $answer3) {
            Write-Host "  • Run 2 vs Run 3: Different" -ForegroundColor Red
        }
    }
    
    Write-Host ""
    Write-Host "Strategy Distribution:" -ForegroundColor Cyan
    $strategyGroups = $successfulResults | Group-Object -Property Strategy
    foreach ($group in $strategyGroups) {
        Write-Host "  • $($group.Name): $($group.Count) times" -ForegroundColor White
    }
    
    Write-Host ""
    Write-Host "Score Statistics:" -ForegroundColor Cyan
    $avgScore = ($successfulResults | Measure-Object -Property Score -Average).Average
    $minScore = ($successfulResults | Measure-Object -Property Score -Minimum).Minimum
    $maxScore = ($successfulResults | Measure-Object -Property Score -Maximum).Maximum
    Write-Host "  • Average: $([math]::Round($avgScore, 2))" -ForegroundColor White
    Write-Host "  • Min: $minScore" -ForegroundColor White
    Write-Host "  • Max: $maxScore" -ForegroundColor White
    
    Write-Host ""
    Write-Host "Response Time Statistics:" -ForegroundColor Cyan
    $avgTime = ($successfulResults | Measure-Object -Property Duration -Average).Average
    $minTime = ($successfulResults | Measure-Object -Property Duration -Minimum).Minimum
    $maxTime = ($successfulResults | Measure-Object -Property Duration -Maximum).Maximum
    Write-Host "  • Average: $([math]::Round($avgTime, 0))ms" -ForegroundColor White
    Write-Host "  • Min: $($minTime)ms" -ForegroundColor White
    Write-Host "  • Max: $($maxTime)ms" -ForegroundColor White
}
else {
    Write-Host "⚠ Some tests failed" -ForegroundColor Yellow
    Write-Host "Successful: $($successfulResults.Count)/3" -ForegroundColor White
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
