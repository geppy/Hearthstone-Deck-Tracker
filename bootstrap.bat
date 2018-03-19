@echo off

nuget restore
git clone https://github.com/HearthSim/HearthSim.Common HearthSim.Common
cd HearthSim.Common
call bootstrap.bat
cd ..
git clone https://github.com/HearthSim/HDT-Localization HDT-Localization
xcopy /Y "HDT-Localization\*.resx" "Hearthstone Deck Tracker\Properties\"
./generate_resources.bat