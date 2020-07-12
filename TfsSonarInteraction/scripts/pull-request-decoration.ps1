[String]$sourcebranch = "$env:BUILD_SOURCEBRANCH"
$execFile = "C:\Users\chinh\Downloads\vsts-agent-win7-x64-2.122.1\_work\1\s\TfsSonarInteraction\TfsSonarInteraction.exe";
$pullrequest = "refs/pull/+(?<pullnumber>\w+?)/merge+"
if($sourcebranch -match $pullrequest){        
   Start-Process -FilePath $execFile $Matches.pullnumber, "sonar1" ;
}
else { write-host "Cannot find pull request ID" }