<h1 align="center">
<a href="https://github.com/code-tesseract/dotnet-api-component">
    <img src="https://i.imgur.com/qSIOgVy.png" alt="DotnetApiComponent" width="300">
</a>
<br>
<b>Dotnet API Action Base</b>
</h1>

<h2>Example Simulation</h4>
<h4>Entity List</h4>

* Region
* Province
* RegionProvince
* ProvinceAssignment
* Regency
* District
* Village

<h3>How implement allowed property ?</h3>
<small>**District** as Parent Class</small>

<small>Filter by <code>**District.Name**</code></small>

```csharp
var allowedFilterProperty = new List<AllowedFilterProperty>
{
    new()
    {
        Key = nameof(District.Name)		
    }
};
```

<small>Filter by <code>**District.Name**</code> with custom key</small>

```csharp
var allowedFilterProperty = new List<AllowedFilterProperty>
{
    new()
    {
        Key = "DistrictName",
        FilterProperty = nameof(District.Name)		
    }
};
```

<small>Filter by <code>**Regency.Name**</code> with one to one relation</small>

```csharp
var allowedFilterProperty = new List<AllowedFilterProperty>
{
    new()
    {
        Key = "RegencyName",
        RelationProperty = new[] { nameof(District.Regency) },
        FilterProperty = nameof(Regency.Name)
    }
};
```

<small>Filter by <code>**Village.Name**</code> with one to many relation</small>

```csharp
var allowedFilterProperty = new List<AllowedFilterProperty>
{
    new()
    {
        Key = "VillageName",
        RelationProperty = new[] { nameof(District.Villages) },
        FilterProperty = nameof(Village.Name)
    }
};
```