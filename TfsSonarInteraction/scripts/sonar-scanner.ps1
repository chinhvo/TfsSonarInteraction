dotnet sonarscanner begin /k:"sonar1"   /d:sonar.login="f5bec4c9e3be0b7444abebed592d3471c6782b29" 
dotnet build ConsoleApp1/ConsoleApp1.sln
dotnet build ConsoleApp2/ConsoleApp2.sln 

dotnet sonarscanner end /d:sonar.login="f5bec4c9e3be0b7444abebed592d3471c6782b29" 
