#!/bin/bash

echo Test

if [ $# -eq 0 ]; then
    fake build
else
    fake run build.fsx -t $@
fi
