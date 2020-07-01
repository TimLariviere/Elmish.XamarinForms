#!/bin/bash

dotnet tool restore
cd Fabulous.XamarinForms/samples/
dotnet paket restore
cd ../..