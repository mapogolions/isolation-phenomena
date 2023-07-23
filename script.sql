create database IsolationPhenomena;

use IsolationPhenomena;

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
