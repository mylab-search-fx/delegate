echo off

IF [%1]==[] goto noparam

echo "Build image '%1' and 'latest'..."
docker build -f ./Dockerfile -t ghcr.io/mylab-search-fx/searcher:%1 -t ghcr.io/mylab-search-fx/searcher:latest ../src

echo "Publish image '%1' ..."
docker push ghcr.io/mylab-search-fx/searcher:%1

echo "Publish image 'latest' ..."
docker push ghcr.io/mylab-search-fx/searcher:latest

goto done

:noparam
echo "Please specify image version"
goto done

:done
echo "Done!"