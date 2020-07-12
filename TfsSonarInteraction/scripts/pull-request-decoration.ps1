$job = Start-Job { 
	[String]$sourcebranch = "$env:BUILD_SOURCEBRANCH"
	$execFile = "C:\Users\chinh\Downloads\vsts-agent-win7-x64-2.122.1\_work\1\s\TfsSonarInteraction\TfsSonarInteraction.exe";
	$pullrequest = "refs/pull/+(?<pullnumber>\w+?)/merge+"
	if($sourcebranch -match $pullrequest){     
		-Process -FilePath $execFile $Matches.pullnumber, "sonar1" -NoNewWindow -Wait; 
	}
	else { write-host "Cannot find pull request ID" }
}


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

$execFile = "C:\Users\chinh\Downloads\vsts-agent-win7-x64-2.122.1\_work\1\s\TfsSonarInteraction";
$results = Start-Process -FilePath  $execFile 2, "sonar1" -WindowStyle Hidden | Await-Task
