```sql
create database IsoLevelsAdoNet;

use IsoLevelsAdoNet;

create table Album (
    Id int Identity PRIMARY KEY,
    Title varchar(128) not null,
    Artist varchar(255) not null,
    Price decimal(5, 2) not null
);

insert into Album
    (Title, Artist, Price)
values
    ('Blue Train', 'John Coltrane', 56.99),
    ('Giant Steps', 'John Coltrane', 63.99),
    ('Jeru', 'Gerry Mulligan', 17.99),
    ('Sarah Vaughan', 'Sarah Vaughan', 34.98)
```

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
