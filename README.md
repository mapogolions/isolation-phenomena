### Isolation Phenomena

Demo of various isolation phenomena

#### How to use

- create Db using [Script](./script.sql)
- configure connection string
```sh
mkdir -p ~/.microsoft/usersecrets/2d57e6d1-388f-4169-bf7a-d0597639b88a
cd ~/.microsoft/usersecrets/2d57e6d1-388f-4169-bf7a-d0597639b88a
touch secrets.json
```

- update secrets.json
```
"ConnectionStrings": {
        "IsolationPhenomena": "<your-connection-string>"
    }
```
