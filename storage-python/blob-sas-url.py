#!/usr/bin/python
# -*- coding: utf-8 -*-

from datetime import datetime, timedelta
from azure.storage.blob import (
    BlockBlobService,
    ContainerPermissions,
    BlobPermissions,
    PublicAccess,
)

_STORAGE_ACCOUNT_NAME="<storage account name>"
_STORAGE_ACCOUNT_KEY="<storage account key>"
_CONTAINER_NAME="<container name>"
_BLOB_NAME = "<blob object name>"

blob_service = BlockBlobService(account_name=_STORAGE_ACCOUNT_NAME, account_key=_STORAGE_ACCOUNT_KEY)
generator = blob_service.list_blobs(_CONTAINER_NAME)
for blob in generator:
    print(blob.name)

token = blob_service.generate_blob_shared_access_signature(
            _CONTAINER_NAME,
            _BLOB_NAME,
            BlobPermissions.READ,
            datetime.utcnow() + timedelta(hours=1),
        )
print("token={}".format(token))
blob_url = "https://{0}.blob.core.windows.net/{1}/{2}?{3}".format(_STORAGE_ACCOUNT_NAME,_CONTAINER_NAME,_BLOB_NAME,token)
print("blob_url={}".format(blob_url))
