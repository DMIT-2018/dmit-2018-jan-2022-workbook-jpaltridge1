<Query Kind="Expression">
  <Connection>
    <ID>a100a46e-4920-47d8-8174-c63b817e89b4</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>Chinook</Database>
  </Connection>
</Query>

// Jeff Paltridge Jan. 18, 2022
//List all albums showing the Title, Artist name, Year and decade of
// releases (oldies, 70s, 80s, 90s, or modern
// Order by decade

//understand problem
// collection: albums
// selective data set : anonymous data set
// ordering: decade
// decade: either 70s , 80s , 90s , modern or oldies


//design
//	Albums
// .Orderby : decade
// .Select (new{})
// using nav property Artist Name
// assigning the decade using ternary operator


Albums

	.Select(x => new
	{
		Decade =  (x.ReleaseYear < 1970) ? "Oldies" : (x.ReleaseYear > 1969 && x.ReleaseYear < 1980) ? "70s" : (x.ReleaseYear > 1979 && x.ReleaseYear < 1990) ? "80s" 
		: (x.ReleaseYear > 1989 && x.ReleaseYear < 2000) ? "90s" : "Modern",
		Title = x.Title,
		Artist = x.Artist.Name,
		Year = x.ReleaseYear

	})

	.OrderBy(x => x.Year)