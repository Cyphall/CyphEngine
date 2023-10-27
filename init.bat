@echo off
set VCPKG_DEFAULT_TRIPLET=x64-windows-static
cd CyphEngineNative
git clone https://github.com/Microsoft/vcpkg
cd vcpkg
git checkout a5d6d145164e82e67fbf91a4a30f98699d30de63
call bootstrap-vcpkg.bat
vcpkg install stb
rmdir /s /q .git
rmdir /s /q buildtrees
rmdir /s /q downloads
rmdir /s /q packages
cd ..
cd ..
pause