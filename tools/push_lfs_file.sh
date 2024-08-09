#!/usr/bin/bash

# This script manually pushes the contents of a file to a remote repository
# as an LFS object. This can be used to fix 'object does not exist' LFS smudge errors
# caused by LFS objects not being pushed alongside commits that reference them.
#
# Usage: push_lfs_file.sh <file path> [remote]
# Target remote defaults to 'origin' if not given.

REMOTE=${2:-origin}

# 'git lfs clean' reads large file data from stdin, adds it to the LFS object database,
# and writes an LFS pointer file for that object to stdout,
# which we then cut up with awk to get the ID of the new object.
#
# This command is normally run by the 'clean' git hook installed by LFS,
# and AFAIK it's the only way to actually add new LFS objects to the database.
# (It also takes the path of the file to be added for some reason not mentioned in the man page.)
OID=$(git lfs clean "$1" <$1 | awk -F ':' '/^oid sha256/ {print $2}')

echo "Pushing LFS object $OID to $REMOTE..."

# We could technically use the --stdin option here to do the whole operation in one line,
# but I'll split it for the sake of clarity.
git lfs push $REMOTE --object-id $OID
