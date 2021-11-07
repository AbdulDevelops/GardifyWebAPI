frontendPath="GardifyFrontend/dist/gardify/"
backendPath="GardifyWebAPINew/bin/Release/GardifyWebAPI_Upload/"
adminPath="PflanzenApp/bin/Release/PflanzenApp_Upload"
releasesPath="Releases/"

cp.exe -r $frontendPath $releasesPath
echo -e 'Copied frontend.'

cp.exe -r $backendPath $releasesPath
echo -e 'Copied backend.'

cp.exe -r $adminPath $releasesPath
echo -e 'Copied admin area.'

cd Releases && zip.exe -r Gardify_Release.zip .
echo -e 'Finished zipping.'

mv.exe Gardify_Release.zip ../
rm -r ./*
echo -e 'Done.'