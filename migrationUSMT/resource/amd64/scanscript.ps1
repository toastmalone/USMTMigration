	[string]$user
	./scanstate C:\store /localonly /o /c /ui:TMCCADMN\user /ue:* /ue:%computername%* /i:MigUser.xml /v:13 /vsc /progress:prog.log /listfiles:filelist.txt

