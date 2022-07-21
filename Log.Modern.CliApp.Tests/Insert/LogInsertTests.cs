using Log.Data;
using Log.Modern.CliApp.TestApi;
using Xunit;
using XUnit.Helper;

namespace Log.Modern.CliApp.Tests;

[Collection("Serial2")]
[TestCaseOrderer(OrdererTypeName, OrdererAssemblyName)]
public class LogInsertTests
    : OrderTest
    , IClassFixture<LogFixture>
{
    private LogFixture fixture;

    public LogInsertTests(LogFixture fixture)
    {
        this.fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(LogInsertData.Test01)
        , MemberType= typeof(LogInsertData))]
    public void Test01(params string[] cmd)
    {
        fixture.RunCmd(fixture.Booter, cmd);
    }

    [Theory]
    [MemberData(nameof(TaskInsertData.Test02)
        , MemberType= typeof(TaskInsertData))]
    public void Test02(params string[] cmd)
    {
        var category = fixture.GetCategory(fixture.Uow, elementIndex: 0);
        var command = new List<string>(cmd);
        SetValue(command, "categoryid", category.Id.ToString());
        fixture.RunCmd(fixture.Booter, command.ToArray());
    }

    [Theory]
    [MemberData(nameof(LogInsertData.Test03)
        , MemberType= typeof(LogInsertData))]
    public void Test03(params string[] cmd)
    {
        fixture.RunCmd(fixture.Booter, cmd);
    }

    [Theory]
    [MemberData(nameof(LogInsertData.Test04)
        , MemberType= typeof(LogInsertData))]
    public void Test04(params string[] cmd)
    {
        fixture.AssertLogCount(fixture.Uow, 0);
        var task = fixture.GetTask(fixture.Uow, elementIndex: 0);
        var place = fixture.GetPlace(fixture.Uow, elementIndex: 0);
        var command = new List<string>(cmd);
        SetValue(command, "taskid", task.Id.ToString());
        SetValue(command, "placeid", place.Id.ToString());
        fixture.RunCmd(fixture.Booter, command.ToArray());
        fixture.AssertLogCount(fixture.Uow, 1);
        var data = fixture.GetLog(fixture.Uow, elementIndex: 0);
        fixture.AssertLog(
            new LogModel 
            {
                TaskId = task.Id
                , Description = "test"
                , PlaceId = place.Id
                , Start = new DateTime(2022, 7, 21, 17, 0, 0)
                , End = new DateTime(2022, 7, 21, 18, 0, 0)
            }
            , data);
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