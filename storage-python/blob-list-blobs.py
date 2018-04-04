#!/usr/bin/python
# -*- coding: utf-8 -*-

from azure.storage.blob import BlockBlobService

_STORAGE_ACCOUNT_NAME="<storage account name>"
_STORAGE_ACCOUNT_KEY="<storage account key>"
_CONTAINER_NAME="<container name>"

blob_service = BlockBlobService(
            account_name=_STORAGE_ACCOUNT_NAME,
            account_key=_STORAGE_ACCOUNT_KEY)

generator = blob_service.list_blobs(_CONTAINER_NAME)
for blob in generator:
    print("blob name => {}".format(blob.name))
