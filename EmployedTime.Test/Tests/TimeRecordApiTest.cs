using EmployedTime.common.Models;
using EmployedTime.Functions.Entities;
using EmployedTime.Functions.Funtions;
using EmployedTime.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;


namespace EmployedTime.Test.Tests
{
    public class TimeRecordApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateTimeRecord_Should_Return_200()
        {
            //Arrange
            MockCloudTableTimeRecord mockTimeRecords = new MockCloudTableTimeRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            EmployedTimeEntity employedTimeEntity = TestFactory.GetTimeRecordRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(employedTimeEntity);

            //Act
            IActionResult response = await EmployedTimeAPI.CreateEmployedTime(request, mockTimeRecords, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void UpdateTimeRecord_Should_Return_200()
        {
            //Arrange
            MockCloudTableTimeRecord mockTimeRecords = new MockCloudTableTimeRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            EmployedTimeEntity employedTimeEntity = TestFactory.GetTimeRecordRequest();
            Guid timeRecordKey = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRecordKey, employedTimeEntity);

            //Act
            IActionResult response = await EmployedTimeAPI.UpdateEmployedTime(request, mockTimeRecords, timeRecordKey.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public async void DeleteTimeRecord_Should_Return_200()
        {
            //Arrange
            MockCloudTableTimeRecord mockTimeRecords = new MockCloudTableTimeRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            EmployedTimeEntity employedTimeEntity = TestFactory.GetEmployedTimeEntity();
            Guid timeRecordKey = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRecordKey);

            //Act
            IActionResult response = await EmployedTimeAPI.DeleteEmployedTimeById(request, employedTimeEntity, mockTimeRecords, timeRecordKey.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }



        [Fact]
        public async void GetAllTimeRecords_Should_Return_200()
        {
            //Arrange
            MockCloudTableTimeRecord mockTimeRecords = new MockCloudTableTimeRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            //Act
            IActionResult response = await EmployedTimeAPI.GetAllEmployedTime(request, mockTimeRecords, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

    }
}
