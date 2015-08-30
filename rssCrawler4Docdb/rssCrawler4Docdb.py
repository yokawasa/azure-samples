#!/usr/bin/env python
# -*- coding: utf-8 -*-

import feedparser
import pydocumentdb.documents as documents
import pydocumentdb.document_client as document_client
import pydocumentdb.errors as errors
import pydocumentdb.http_constants as http_constants

Docdb_masterKey = 'Tl1+ikQtnExUisJ+BXwbbaC8NtUqYVE9kUDXCNust5aYBduhui29Xtxz3DLP88PayjtgtnARc1PW+2wlA6jCJw=='
Docdb_host = 'https://yoichikademo.documents.azure.com:443/'
Docdb_dbname = 'feeddb'
Docdb_colname = 'article_collection'

feedurl='http://blogs.msdn.com/b/windowsazurej/atom.aspx'

def rsscrawling():
    # create documentDb client instance
    client = document_client.DocumentClient(Docdb_host,
                                 {'masterKey': Docdb_masterKey})
    # create a database if needed
    database_definition = {'id': Docdb_dbname }
    databases = list(client.QueryDatabases({
            'query': 'SELECT * FROM root r WHERE r.id=@id',
            'parameters': [
                { 'name':'@id', 'value': database_definition['id'] }
            ]
        }))

    if ( len(databases) > 0 ):
        feeddb = databases[0]
    else:
        print "database is created:%s" % Docdb_dbname 
        feeddb = client.CreateDatabase(database_definition)

    # create a collection if needed
    collection_definition = { 'id': Docdb_colname }
    collections = list(client.QueryCollections(
        feeddb['_self'],
        {
            'query': 'SELECT * FROM root r WHERE r.id=@id',
            'parameters': [
                { 'name':'@id', 'value': collection_definition['id'] }
            ]
        }))
    if ( len(collections) > 0 ):
        collection = collections[0]
    else:
        print "collection is created:%s" % Docdb_colname
        collection = client.CreateCollection(
                    feeddb['_self'], collection_definition)
 
    # request & parse rss feed via feedparser
    feed=feedparser.parse(feedurl)
    for entry in feed[ 'entries' ]:
        document_definition = { 'title':entry[ 'title'],
                                'content':entry['description'],
                                'permalink':entry[ 'link' ],
                                'postdate':entry['date'] }

        #duplicate check
        documents = list(client.QueryDocuments(
            collection['_self'],
            {
                'query': 'SELECT * FROM root r WHERE r.permalink=@permalink',
                'parameters': [
                    { 'name':'@permalink', 'value':document_definition['permalink'] }
                ]
            }))
        if (len(documents) < 1):
            # only create if it's fully new document
            print "document is added:title:%s" % entry['title']
            created_document = client.CreateDocument(
                    collection['_self'], document_definition)


if __name__ == '__main__':
    rsscrawling()
