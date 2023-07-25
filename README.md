### Isolation Phenomena

Demo of various isolation phenomena

- [Dirty Read](./Phenomena/DirtyReadPhenomenon.cs)
- [Non-Repeatable Read](./Phenomena/NonRepeatableReadPhenomenon.cs)
- [Phantom Read](./Phenomena/PhantomReadPhenomenon.cs)

#### How to use

- create database with [script](./script.sql)
- configure connection string
    - create a folder using info from `AssemblyInfo.cs`
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
