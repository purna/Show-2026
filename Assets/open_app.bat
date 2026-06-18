```bat
@echo on

echo Logging started at %DATE% %TIME% >> log.txt
echo Passed arguments: %* >> log.txt
echo This is the log message >> log.txt
dir >> log.txt
echo Logging finished at %DATE% %TIME% >> log.txt

if "%~1"=="" (
    echo No parameter specified. Usage: open_app.bat [application_number]
    exit /b 1
)

setlocal enabledelayedexpansion

rem ==========================
rem Year 1 Projects
rem ==========================

set "application[0]=C:\Users\nigel.morris\Show-2026\Portfolio\Abbie Kilby\Abbie KilbyFMP.exe"
set "application[1]=C:\Users\nigel.morris\Show-2026\Portfolio\Aleena Akhtar\FMP Gameplay Showcase.mp4"
set "application[2]=C:\Users\nigel.morris\Show-2026\Portfolio\Callum Stoor\CallumStoor_s FMP\CallumStoorFMP.exe"
set "application[3]=C:\Users\nigel.morris\Show-2026\Portfolio\Charlie MacDonald\Charlie MacDonald_s FMP\Charlie MacDonald's FMP.exe"
set "application[4]=C:\Users\nigel.morris\Show-2026\Portfolio\Elliot Fayose\Dungeon\Builds\NightTime.exe"
set "application[5]=C:\Users\nigel.morris\Show-2026\Portfolio\Emila Collyer\Emila Collyer BeeRPG_Data\Emila Collyer BeeRPG.exe"
set "application[6]=C:\Users\nigel.morris\Show-2026\Portfolio\James Barrow\McDragon Made by James\McDragon.exe"
set "application[7]=C:\Users\nigel.morris\Show-2026\Portfolio\Joshie Dixon\FMP-joshie\Brotato-rouguelike.exe"
set "application[8]=C:\Users\nigel.morris\Show-2026\Portfolio\Owen Smith\Expendable\Expendable\FMP.exe"
set "application[9]=C:\Users\nigel.morris\Show-2026\Portfolio\Will Mitchell\Video Project.mp4"

rem ==========================
rem Year 2 Projects
rem ==========================

set "application[10]=C:\Users\nigel.morris\Show-2026\Portfolio\Ashton Stammers\FMP-Y1\PandorasBox.exe"
set "application[11]=C:\Users\nigel.morris\Show-2026\Portfolio\Ashton Stammers\fmp-y2.mp4"
set "application[12]=C:\Users\nigel.morris\Show-2026\Portfolio\Ben Spayne\Ben Spayne - The Backrooms\Build\Assignment 4.exe"
set "application[13]=C:\Users\nigel.morris\Show-2026\Portfolio\Ben Spayne\Ben Spayne - Terminal Horizon\Ben Spayne FMPYear2.exe"
set "application[14]=C:\Users\nigel.morris\Show-2026\Portfolio\Charlie Hale\FMP1\Assignment 5 - FMP.exe"
set "application[15]=C:\Users\nigel.morris\Show-2026\Portfolio\Charlie Hale\FMP2\Assignment 5 - FMP.exe"
set "application[16]=C:\Users\nigel.morris\Show-2026\Portfolio\Chris Collington\FMP-Y1\FMP Game.exe"
set "application[17]=C:\Users\nigel.morris\Show-2026\Portfolio\Chris Collington\Chris Collington - GameFmp_Data\GameFmp.exe"
set "application[18]=C:\Users\nigel.morris\Show-2026\Portfolio\Eddie Hill\FMP_Y1\ColinBB.set"
set "application[19]=C:\Users\nigel.morris\Show-2026\Portfolio\Eddie Hill\MinigameCollection By Eddie Hill\MinigameCollection.exe"
set "application[20]=C:\Users\nigel.morris\Show-2026\Portfolio\Fyhren Parry\FMP Y1\Nerror.exe"
set "application[21]=C:\Users\nigel.morris\Show-2026\Portfolio\Fyhren Parry\Fyhren parry - Depth Defender 3D\Depth Defenders.exe"
set "application[22]=C:\Users\nigel.morris\Show-2026\Portfolio\George Lundin\FMP Y1\George-s-FMP-Game.exe"
set "application[23]=C:\Users\nigel.morris\Show-2026\Portfolio\George Lundin\FMP Y2\RPG 3D projecct.exe"
set "application[24]=C:\Users\nigel.morris\Show-2026\Portfolio\Imogen Devlin\FMP Y1P\My project (23).exe"
set "application[25]=C:\Users\nigel.morris\Show-2026\Portfolio\Imogen Devlin\Imogen Devlin FMP\Pixel.exe"
set "application[26]=C:\Users\nigel.morris\Show-2026\Portfolio\Luca Rampling\Luca Rampling -King of the Hill\King of the Hill.exe"
set "application[27]=C:\Users\nigel.morris\Show-2026\Portfolio\Luca Ramplin\Light-Souls-master\Light Souls.exe"
set "application[28]=C:\Users\nigel.morris\Show-2026\Portfolio\Luke Tanner\FMP Y1\FMP-assignment5.exe"
set "application[29]=C:\Users\nigel.morris\Show-2026\Portfolio\Luke Tanner\Luke Tanner Vampire Shooter\FMP Vampire Shooter.exe"
set "application[30]=C:\Users\nigel.morris\Show-2026\Portfolio\Nicholas Robertson\FMP Y1\Swword Game.exe"
set "application[31]=C:\Users\nigel.morris\Show-2026\Portfolio\Nicholas Robertson\First Attempt At StanleyP Animation 2.mp4"
set "application[32]=C:\Users\nigel.morris\Show-2026\Portfolio\Yannik Absolom\FMP Y1\"
set "application[33]=C:\Users\nigel.morris\Show-2026\Portfolio\Yannik Absolom\FMP Y2\"


rem Check if the specified application number exists
if not defined application[%1] (
    echo Invalid application number: %1
    exit /b 1
)

set "target=!application[%1]!"

echo Opening application %1: !target!

if /i "!target:~0,4!"=="http" (
    rem Open webpage in default browser
    start "" "!target!"
) else if /i "!target:~-4!"==".mp4" (
    rem Open video in default player
    start "" "!target!"
) else (
    rem Launch executable maximized
    start "" /max "!target!"
)

endlocal
exit /b 0
```
