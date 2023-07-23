### Dirty Read (ReadUncommitted)
```
{begin}             {begin}
    update
                        read (DIRTY READ)
    rollback
{end}
```

### Non Repeatable Read (ReadCommitted)

```
{begin}             {begin}
    read
                        read (val_1)
    update
    commit
                        read (val_2) // (NON REPEATABLE READ) val_1 != val_2
{end}               {end}
```
