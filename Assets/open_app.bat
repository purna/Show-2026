@echo on

echo Logging started at %DATE% %TIME% >> log.txt

echo Passed arguments: %* >> log.txt
echo This is the log message >> log.txt
dir >> log.txt

echo Logging finished at  %DATE% %TIME% >> log.txt

if "%~1" == "" (
    echo No parameter specified. Usage : open_app.bat [application_number]
    exit /b
)

setlocal enabledelayedexpansion


set "application[0]=C:\Users\nigel.morris\Show-2026\Portfolio/Abbie Kilby/Abbie KilbyFMP.exe"
set "application[1]=C:\Users\nigel.morris\Show-2026\Portfolio/Aleena Akhtar/FMP Gameplay Showcase.mp4"
set "application[2]=C:\Users\nigel.morris\Show-2026\Portfolio/Ben Spayne/Ben Spayne - The Backrooms/Build/Assignment 4.exe"
set "application[3]=C:\Users\nigel.morris\Show-2026\Portfolio/Ben Spayne/Ben Spayne - Terminal Horizon/Ben Spayne FMPYear2.exe"
set "application[4]=C:\Users\nigel.morris\Show-2026\Portfolio/Callum Stoor/Callum Stoor FMP.exeCallumStoor_s FMP/CallumStoorFMP.exe"
set "application[5]=C:\Users\nigel.morris\Show-2026\Portfolio/Charlie Hale/"
set "application[6]=C:\Users\nigel.morris\Show-2026\Portfolio/Charlie MacDonald/Charlie MacDonald_s FMP/Charlie MacDonald's FMP.exe"
set "application[7]=C:\Users\nigel.morris\Show-2026\Portfolio/Chris Collington/Chris Collington - GameFmp_Data/GameFmp.exe"
set "application[8]=C:\Users\nigel.morris\Show-2026\Portfolio/Eddie Hill/FMP_Y1/ColinBB.exe"
set "application[9]=C:\Users\nigel.morris\Show-2026\Portfolio/Eddie Hill/MinigameCollection By Eddie Hill/MinigameCollection.exe"
set "application[10]=C:\Users\nigel.morris\Show-2026\Portfolio/Elliot Fayose/Dungeon/Builds/NightTime.exe"
set "application[11]=C:\Users\nigel.morris\Show-2026\Portfolio/Emila Collyer/Emila Collyer BeeRPG_Data/Emila Collyer BeeRPG.exe"
set "application[12]=C:\Users\nigel.morris\Show-2026\Portfolio/Fyhren Parry/Fyhren parry - Depth Defender 3D/Depth Defenders.exe"
set "application[13]=C:\Users\nigel.morris\Show-2026\Portfolio/Fyhren Parry"
set "application[14]=C:\Users\nigel.morris\Show-2026\Portfolio/George Lundin/Final build-20260615T123744Z-3-001/Final build/Year 2 assignment 1.exe"
set "application[15]=C:\Users\nigel.morris\Show-2026\Portfolio/Imogen Devlin/Imogen Devlin FMP/Pixel.exe"
set "application[16]=C:\Users\nigel.morris\Show-2026\Portfolio/James Barrow/McDragon Made by James/McDragon.exe"
set "application[17]=C:\Users\nigel.morris\Show-2026\Portfolio/Joshie Dixon/FMP-joshie/FMP-joshie/Brotato-rouguelike.exe"
set "application[18]=C:\Users\nigel.morris\Show-2026\Portfolio/Luca Rampling/Luca Rampling -King of the Hill/King of the Hill.exe"
set "application[19]=C:\Users\nigel.morris\Show-2026\Portfolio/Luca Rampling"
set "application[20]=C:\Users\nigel.morris\Show-2026\Portfolio/Luke Tanner/Luke Tanner Vampire Shooter/FMP Vampire Shooter.exe"
set "application[21]=C:\Users\nigel.morris\Show-2026\Portfolio/Nicholas Animations/First Attempt At StanleyP Animation 2.mp4"
set "application[22]=C:\Users\nigel.morris\Show-2026\Portfolio/Nicholas Animations/Second Attempt At StanleyP Animation 1.mp4"
set "application[22]=C:\Users\nigel.morris\Show-2026\Portfolio/Owen Smith/Expendable/Expendable/FMP.exe"
set "application[23]=C:\Users\nigel.morris\Show-2026\Portfolio/Will Mitchell/Video Project.mp4"



rem Check if the specified application number exists
if not defined application[%1] (
    echo Invalid application number.
    exit /b
)

echo Opening application %1: !application[%1]!
start "" /max "!application[%1]!"
