using Microsoft.EntityFrameworkCore;
using Simulation.Shared.Mocks;
using Simulation.Shared.Models;

namespace Simulation.Shared;

public class Repository : IRepository
{
    private readonly DatabaseContext _db;

    public Repository(DatabaseContext db)
    {
        _db = db;

        ProvinceJawaTimur();
        ProvinceJawaTengah();
    }


    private void ProvinceJawaTimur()
    {
        var province       = new MockProvinceBuilder().WithName("Jawa Timur").WithStatus().Build();
        var region         = new MockRegionBuilder().WithName("Region 1").WithStatus().Build();
        var regionProvince = new MockRegionProvinceBuilder(region, province).WithStatus().Build();

        var pa1 = new MockProvinceAssignmentBuilder(province)
            .WithAssignmentStatus().WithStatus(ProvinceAssignment.StatusInactive).Build();

        var pa2 = new MockProvinceAssignmentBuilder(province)
            .WithAssignmentStatus(ProvinceAssignment.AssignmentStatusApproved).WithStatus().Build();

        var r1 = new MockRegencyBuider(province).WithName("Kota Malang").WitCode("mlg").WithStatus().Build();
        var r2 = new MockRegencyBuider(province).WithName("Kota Surabaya").WitCode("sby").WithStatus().Build();

        var d1 = new MockDistrictBuilder(r1).WithName("Blimbing").WithStatus().Build();
        var d2 = new MockDistrictBuilder(r1).WithName("Klojen").WithStatus().Build();
        var d3 = new MockDistrictBuilder(r2).WithName("Genteng").WithStatus().Build();
        var d4 = new MockDistrictBuilder(r1).WithName("Tegalsari").WithStatus().Build();

        _db.Region.Add(region);
        _db.Province.Add(province);
        _db.RegionProvince.Add(regionProvince);
        _db.ProvinceAssignment.AddRange(pa1, pa2);
        _db.Regency.AddRange(r1, r2);
        _db.District.AddRange(d1, d2, d3, d4);
        _db.Village.AddRange(
            new MockVillageBuilder(d1).WithName("Dinoyo").WithPostalCode("65144").WithStatus().Build(),
            new MockVillageBuilder(d1).WithName("Arjosari").WithPostalCode("65126").WithStatus().Build(),
            new MockVillageBuilder(d2).WithName("Klojen").WithPostalCode("65111").WithStatus().Build(),
            new MockVillageBuilder(d2).WithName("Bareng").WithPostalCode("65119").WithStatus().Build(),
            new MockVillageBuilder(d3).WithName("Kaliasin").WithPostalCode("60271").WithStatus().Build(),
            new MockVillageBuilder(d3).WithName("Genteng").WithPostalCode("60275").WithStatus().Build(),
            new MockVillageBuilder(d4).WithName("Gubeng").WithPostalCode("60281").WithStatus().Build(),
            new MockVillageBuilder(d4).WithName("Tegalsari").WithPostalCode("60262").WithStatus().Build()
        );
        _db.SaveChanges();
    }

    private void ProvinceJawaTengah()
    {
        var province       = new MockProvinceBuilder().WithName("Jawa Tengah").WithStatus().Build();
        var region         = new MockRegionBuilder().WithName("Region 2").WithStatus().Build();
        var regionProvince = new MockRegionProvinceBuilder(region, province).WithStatus().Build();

        var pa1 = new MockProvinceAssignmentBuilder(province)
            .WithAssignmentStatus().WithStatus(ProvinceAssignment.StatusInactive).Build();

        var pa2 = new MockProvinceAssignmentBuilder(province)
            .WithAssignmentStatus(ProvinceAssignment.AssignmentStatusRejected).WithStatus().Build();

        var r1 = new MockRegencyBuider(province).WithName("Kota Semarang").WitCode("smg").WithStatus().Build();
        var r2 = new MockRegencyBuider(province).WithName("Kota Solo").WitCode("sol").WithStatus().Build();

        var d1 = new MockDistrictBuilder(r1).WithName("Semarang Tengah").WithStatus().Build();
        var d2 = new MockDistrictBuilder(r1).WithName("Candisari").WithStatus().Build();
        var d3 = new MockDistrictBuilder(r2).WithName("Pasar Kliwon").WithStatus().Build();
        var d4 = new MockDistrictBuilder(r1).WithName("Jebres").WithStatus().Build();

        _db.Region.Add(region);
        _db.Province.Add(province);
        _db.RegionProvince.Add(regionProvince);
        _db.ProvinceAssignment.AddRange(pa1, pa2);
        _db.Regency.AddRange(r1, r2);
        _db.District.AddRange(d1, d2, d3, d4);
        _db.Village.AddRange(
            new MockVillageBuilder(d1).WithName("Pleburan").WithPostalCode("50164").WithPopulation(1000).WithStatus().Build(),
            new MockVillageBuilder(d1).WithName("Pandean Lamper").WithPostalCode("50161").WithPopulation(2000).WithStatus().Build(),
            new MockVillageBuilder(d2).WithName("Candisari").WithPostalCode("50253").WithPopulation(3000).WithStatus().Build(),
            new MockVillageBuilder(d2).WithName("Sumurboto").WithPostalCode("50272").WithPopulation(4000).WithStatus().Build(),
            new MockVillageBuilder(d3).WithName("Pasar Kliwon").WithPostalCode("57131").WithPopulation(5000).WithStatus().Build(),
            new MockVillageBuilder(d3).WithName("Keprabon").WithPostalCode("57142").WithPopulation(6000).WithStatus().Build(),
            new MockVillageBuilder(d4).WithName("Jebres").WithPostalCode("57121").WithPopulation(7000).WithStatus().Build(),
            new MockVillageBuilder(d4).WithName("Manahan").WithPostalCode("57139").WithPopulation(8000).WithStatus().Build()
        );
        _db.SaveChanges();
    }

    public IQueryable<Province> Provinces()
    {
        return _db.Province
            .Include(_ => _.Regencies).ThenInclude(_ => _.Districts).ThenInclude(_ => _.Villages)
            .Include(_ => _.RegionProvinces).ThenInclude(_ => _.Region)
            .AsQueryable();
    }

    public IQueryable<Regency> Regencies()
    {
        return _db.Regency
            .Include(_ => _.Province).ThenInclude(_ => _.ProvinceAssignments)
            .Include(_ => _.Province).ThenInclude(_ => _.RegionProvinces).ThenInclude(_ => _.Region)
            .Include(_ => _.Districts)
            .ThenInclude(_ => _.Villages)
            .AsQueryable();
    }

    public IQueryable<District> Districts()
    {
        return _db.District
            .Include(_ => _.Regency)
            .ThenInclude(_ => _.Province)
            .ThenInclude(_ => _.RegionProvinces)
            .ThenInclude(_ => _.Region)
            .Include(_ => _.Villages)
            .AsQueryable();
    }
}