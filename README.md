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

### Fix `Dirty Read` problem
```
{begin}             {begin}
    read
    update
                        read(block)
    commit
                        read(actual read happens here)

{end}
```

### Read Committed (Non Repetable Read)

```
{begin}             {begin}
    read
                        read
    update
    commit
{end}
                        read // (NON REPETABLE READ) read1 and read2 are different
                    {end}
```
