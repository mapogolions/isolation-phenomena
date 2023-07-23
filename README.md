### Isolation Phenomena

Demo of various isolation phenomena

#### How to use

1) Create Db using [Script](./script.sql)
2) Configure connection string
```sh
mkdir -p ~/.microsoft/usersecrets/2d57e6d1-388f-4169-bf7a-d0597639b88a
cd ~/.microsoft/usersecrets/2d57e6d1-388f-4169-bf7a-d0597639b88a
touch secrets.json
```

secrets.json
```
"ConnectionStrings": {
        "IsolationPhenomena": "<your-connection-string>"
    }
```
