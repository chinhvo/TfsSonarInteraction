Start-Job -Name Job1 -ScriptBlock { 
	[String]$sourcebranch = "$env:BUILD_SOURCEBRANCH"
	$execFile = "C:\Users\chinh\Downloads\vsts-agent-win7-x64-2.122.1\_work\1\s\TfsSonarInteraction\TfsSonarInteraction.exe";
	$pullrequest = "refs/pull/+(?<pullnumber>\w+?)/merge+"
	if($sourcebranch -match $pullrequest){    
		& $execFile $Matches.pullnumber, "sonar1"
	}
	else { write-host "Cannot find pull request ID" }
}
Wait-Job -Name Job1


#OR

function Await-Task {
    param (
        [Parameter(ValueFromPipeline=$true, Mandatory=$true)]
        $task
    )

    process {
        while (-not $task.AsyncWaitHandle.WaitOne(200)) { }
        $task.GetAwaiter().GetResult()
    }
}

[String]$sourcebranch = "$env:BUILD_SOURCEBRANCH"
$execFile = "C:\Users\chinh\Downloads\vsts-agent-win7-x64-2.122.1\_work\1\s\TfsSonarInteraction\TfsSonarInteraction.exe";
$pullrequest = "refs/pull/+(?<pullnumber>\w+?)/merge+"
if($sourcebranch -match $pullrequest){     
	& $execFile $Matches.pullnumber, "sonar1"
}
else { write-host "Cannot find pull request ID" }
