using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tsp.Models;
using tsp.Repositories;
using tsp.Services;

namespace tsp.Tests;

[TestClass]
public class VFileServiceTests
{
    [TestMethod]
    public async Task SaveFileAsync_ShouldAddFileToRepository()
    {
        var repo = new FakeVFileRepository();
        var service = new VFileService(repo);

        await service.SaveFileAsync("test.txt", new byte[] { 1, 2, 3 }, accountId: 1);

        Assert.AreEqual(1, repo.AddedFiles.Count);
        Assert.AreEqual("test.txt", repo.AddedFiles[0].FileName);
        Assert.AreEqual(1, repo.AddedFiles[0].AccountId);
    }

    [TestMethod]
    public async Task GetFilesAsync_ShouldReturnOnlyFilesForAccount()
    {
        var repo = new FakeVFileRepository();
        repo.AddedFiles.Add(new VFile { FileId = 1, FileName = "a.txt", AccountId = 1 });
        repo.AddedFiles.Add(new VFile { FileId = 2, FileName = "b.txt", AccountId = 2 });
        var service = new VFileService(repo);

        var files = await service.GetFilesAsync(1);

        Assert.AreEqual(1, files.Count());
        Assert.AreEqual(1, files.First().AccountId);
    }

    [TestMethod]
    public async Task DeleteFileAsync_ShouldRemoveExistingFile()
    {
        var repo = new FakeVFileRepository();
        var file = new VFile { FileId = 1, FileName = "delete.txt", AccountId = 1 };
        repo.AddedFiles.Add(file);
        var service = new VFileService(repo);

        await service.DeleteFileAsync(1, 1);

        Assert.AreEqual(0, repo.AddedFiles.Count);
        Assert.IsTrue(repo.DeleteCalled);
    }

    [TestMethod]
    public void PasswordHashService_VerifyValidPassword_ReturnsTrue()
    {
        var password = "StrongPassword123!";
        var hash = PasswordHashService.Hash(password);

        Assert.IsTrue(PasswordHashService.Verify(password, hash));
    }

    [TestMethod]
    public void PasswordHashService_VerifyInvalidPassword_ReturnsFalse()
    {
        var password = "Password1";
        var hash = PasswordHashService.Hash(password);

        Assert.IsFalse(PasswordHashService.Verify("WrongPassword", hash));
    }
}

internal class FakeVFileRepository : IVFileRepository
{
    public List<VFile> AddedFiles { get; } = new();
    public bool DeleteCalled { get; private set; }

    public Task AddAsync(VFile file)
    {
        AddedFiles.Add(file);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(VFile file)
    {
        DeleteCalled = true;
        AddedFiles.RemoveAll(f => f.FileId == file.FileId && f.AccountId == file.AccountId);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<VFile>> GetAllAsync(int accountId) =>
        Task.FromResult(AddedFiles.Where(f => f.AccountId == accountId).AsEnumerable());

    public Task<VFile?> GetAsync(int id, int accountId) =>
        Task.FromResult(AddedFiles.FirstOrDefault(f => f.FileId == id && f.AccountId == accountId));

    public Task<IEnumerable<VFile>> GetByNameAsync(string name, int accountId) =>
        Task.FromResult(AddedFiles.Where(f => f.AccountId == accountId && f.FileName != null && f.FileName.Contains(name)).AsEnumerable());
}
