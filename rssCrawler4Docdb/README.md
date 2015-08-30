# Python Crawler - rssCrawler4Docdb
RSS Crawler does Crawling and parsing data with feedparser and storing results into Azure DocumentDB with pydocumentdb. 

## Configurations
```python
Docdb_masterKey = '<your documentdb master key>'
Docdb_host = 'https://<documentdb account>.documents.azure.com:443/'
Docdb_dbname = '<documentdb database name>'
Docdb_colname = '<documentdb collection name>'
feedurl='<atom feed url>'
```
Sample Configuration:
```python
Docdb_masterKey = 'Tl1+iasdfnExUisJ+BXwbbaC8NtUqYVE9kUDXCNust5aYBduhui29Xtxz3DLP88PayjtgtnARc1PW+2wlA6jCJw=='
Docdb_host = 'https://yoichikademo.documents.azure.com:443/'
Docdb_dbname = 'feeddb'
Docdb_colname = 'article_collection'
feedurl='http://blogs.msdn.com/b/windowsazurej/atom.aspx'
```

## See also
http://unofficialism.info/posts/crawler-with-documentdb-python-sdk-and-feedparser/

