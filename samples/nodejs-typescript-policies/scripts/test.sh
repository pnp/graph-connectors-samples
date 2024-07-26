#!/bin/bash

ARGS=()

while [[ $# -gt 0 ]]; do
  case $1 in
    --env)
      ENV="$2"
      shift # past argument
      shift # past value
      ;;
    --secret)
      SECRET="$2"
      shift # past argument
      shift # past value
      ;;
    *)
      ARGS+=("$1") # save positional arg
      shift # past argument
      ;;
  esac
done

val=$(npx ts-node EnsureConnection.ts  --env $ENV --secret $SECRET)
printf $val
if [[ $val = "1" ]] ; then 
  exit 1 
fi

exit 0
#val=$(npx ts-node EnsureSchema.ts  --env $ENV --secret $SECRET)

#if [[ $val -eq 1 ]]; then
#  exit 1
#fi

#val=$(npx ts-node IngestContent.ts  --env $ENV --secret $SECRET)

#if [[ $val -eq 1 ]]; then
  #exit 1
#fi

#exit 0