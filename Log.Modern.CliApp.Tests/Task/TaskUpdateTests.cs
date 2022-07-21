using Log.Modern.CliApp.TestApi;
using Xunit;
using XUnit.Helper;
using Task = Log.Data.Task;

namespace Log.Modern.CliApp.Tests;

[Collection(DbTests)]
[TestCaseOrderer(OrdererTypeName, OrdererAssemblyName)]
public class TaskUpdateTests
    : OrderTest
    , IClassFixture<LogFixture>
{
    private LogFixture fixture;

    public TaskUpdateTests(LogFixture fixture)
    {
        this.fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(TaskData.Insert01), MemberType= typeof(TaskData))]
    public void Test01(params string[] cmd)
    {
        fixture.RunCmd(fixture.Booter, cmd);
    }

    [Theory]
    [MemberData(nameof(TaskData.Insert02), MemberType= typeof(TaskData))]
    public void Test02(params string[] cmd)
    {
        var category = fixture.GetCategory(fixture.Uow, elementIndex: 0);
        var command = new List<string>(cmd);
        SetValue(command, "categoryid", category.Id.ToString());
        fixture.RunCmd(fixture.Booter, command.ToArray());
    }

    [Theory]
    [MemberData(nameof(TaskData.Update01), MemberType= typeof(TaskData))]
    public void Test03(params string[] cmd)
    {
        fixture.AssertTaskCount(fixture.Uow, 1);
        var task = fixture.GetTask(fixture.Uow, elementIndex: 0);
        var categoryOld = fixture.GetCategory(fixture.Uow, elementIndex: 0);
        fixture.AssertTask(
            new Task 
            {
                CategoryId = categoryOld.Id
                , Name = "jogging"
                , Description = "running exercise"
            }
            , task);
        var categoryNew = fixture.GetCategory(fixture.Uow, elementIndex: 1);
        var command = new List<string>(cmd);
        SetValue(command, "taskid", task.Id.ToString());
        SetValue(command, "categoryid", categoryNew.Id.ToString());
        fixture.RunCmd(fixture.Booter, command.ToArray());
        fixture.AssertTaskCount(fixture.Uow, 1);
        task = fixture.GetTask(fixture.Uow, elementIndex: 0);
        fixture.AssertTask(
            new Task 
            {
                CategoryId = categoryNew.Id
                , Name = "taking shower"
                , Description = "showering in walk in shower"
            }
            , task);
    }

    private void SetValue(
        List<string> cmd
        , string key
        , string value)
    {
        cmd[GetIndex(cmd, key)] = value;
    }

    private int GetIndex(List<string> cmd, string value)
    {
        return cmd.IndexOf(value);
    }
}