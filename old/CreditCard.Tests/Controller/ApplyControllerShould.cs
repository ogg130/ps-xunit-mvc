using Castle.DynamicProxy.Generators.Emitters;
using CreditCards.Controllers;
using CreditCards.Core.Interfaces;
using CreditCards.Core.Model;
using CreditCards.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CreditCard.Tests.Controller
{
    public class ApplyControllerShould
    {

        private readonly Mock<ICreditCardApplicationRepository> _mockRepository;
        private readonly ApplyController _sut;

        public ApplyControllerShould()
        {
            _mockRepository = new Mock<ICreditCardApplicationRepository>();
            _sut = new ApplyController(_mockRepository.Object);

        }

        [Fact]
        public void returnViewForIndex()
        {
            var result = _sut.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ReturnViewWhenInvalidModelState()
        {
            // Add a model error
            _sut.ModelState.AddModelError("x", "Test Error");

            // Build expected object
            var application = new NewCreditCardApplicationDetails
            {
                FirstName = "Sarah"
            };

            // Call index method using expected object
            var result = await _sut.Index(application);

            // If result of assert is successful return the converted type
            var viewResult = Assert.IsType<ViewResult>(result);

            //  if successful, get credit card application details as model
            var model = Assert.IsType<NewCreditCardApplicationDetails>(viewResult.Model);

            // Assert that the expected value equals the first name in the returned model
            Assert.Equal(application.FirstName, model.FirstName);



        }


        [Fact]
        public async Task NotSaveApplicationWhenModelError()
        {
            // Add a model error
            _sut.ModelState.AddModelError("x", "Test Error");

            // Build expected object
            var application = new NewCreditCardApplicationDetails();

            // Call index method using expected object
            await _sut.Index(application);

            // If invalid model, controller should not save application to database
            // Verify that AddAsync has never been called
            _mockRepository.Verify(x => x.AddAsync(It.IsAny<CreditCardApplication>()), Times.Never);

        }

        [Fact]
        public async Task SaveApplicationWhenValidModel()
        {
            // Create temp application which moq will populate
            CreditCardApplication savedApplication = null;

            // Any credit card app should return a completed task and then when executed, 
            // run callback to save application that was pased into addasync method into 
            // savedApplication variable
            _mockRepository.Setup(x => x.AddAsync(It.IsAny<CreditCardApplication>())).
                Returns(Task.CompletedTask).
                Callback<CreditCardApplication>(x => savedApplication = x);

            // Build expected object
            var application = new NewCreditCardApplicationDetails
            {
                FirstName = "Sarah",
                LastName = "Smith",
                Age = 18,
                FrequentFlyerNumber = "012345-A",
                GrossAnnualIncome = 100_000
            };

            // Call index passing in expected values
            await _sut.Index(application);

            // Verify that addasync method was called one time
            _mockRepository.Verify(x => x.AddAsync(It.IsAny<CreditCardApplication>()), Times.Once);

            // Verify mapped values
            Assert.Equal(application.FirstName, savedApplication.FirstName);
            Assert.Equal(application.LastName, savedApplication.LastName);
            Assert.Equal(application.Age, savedApplication.Age);
            Assert.Equal(application.FrequentFlyerNumber, savedApplication.FrequentFlyerNumber);
            Assert.Equal(application.GrossAnnualIncome, savedApplication.GrossAnnualIncome);
        }

        [Fact]
        public async Task ReturnApplicationCompleteViewWhenValidModel()
        {
            // Build expected object
            var application = new NewCreditCardApplicationDetails
            {
                FirstName = "Sarah",
                LastName = "Smith",
                Age = 18,
                FrequentFlyerNumber = "012345-A",
                GrossAnnualIncome = 100_000
            };


            // Call index passing in expected values
            var result = await _sut.Index(application);

            // If result of assert is successful return the converted type / view result
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("ApplicationComplete", viewResult.ViewName);
        }
    }
}
