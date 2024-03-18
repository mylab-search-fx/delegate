echo off

IF [%1]==[] goto noparam

echo "Build image '%1-sdk' ..."
docker build --progress plain --build-arg DOTNET_BASE=sdk -f ./Dockerfile -t ghcr.io/mylab-search-fx/searcher:%1-sdk ../src

echo "Publish image '%1-sdk' ..."
docker push ghcr.io/mylab-search-fx/searcher:%1-sdk

goto done

:noparam
echo "Please specify image version"
goto done

:done
echo "Done!"