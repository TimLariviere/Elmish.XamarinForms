dotnet tool restore
dotnet paket restore
cd docs
cp ../paket-files/docs/ionide/Fornax/src/Fornax.Template/_lib/Fornax.Core.dll _lib/Fornax.Core.dll
cp ../paket-files/docs/ionide/Fornax/src/Fornax.Template/_lib/Markdig.dll _lib/Markdig.dll
dotnet fornax watch