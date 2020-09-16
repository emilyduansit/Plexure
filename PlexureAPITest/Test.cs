using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PlexureAPITest
{
    [TestFixture]
    public class Test
    {
        Service service;

        [OneTimeSetUp]
        public void Setup()
        {
            service = new Service();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            if (service != null)
            {
                service.Dispose();
                service = null;
            }
        }

        [Test]
        public void TEST_001_Login_With_Valid_User_Successful_Response()
        {
            var response = service.Login("Tester", "Plexure123");

            response.Expect(HttpStatusCode.OK);

            Assert.AreEqual(response.Entity.UserName, "Tester");
            Assert.AreEqual(response.Entity.UserId, 1);
            Assert.AreEqual(response.Entity.AccessToken, "37cb9e58-99db-423c-9da5-42d5627614c5");

        }

        [Test]
        public void TEST_002_Get_Points_For_Logged_In_User()
        {

            var points = service.GetPoints();

            points.Expect(HttpStatusCode.Accepted);
            Assert.AreEqual(points.Entity.UserId, 1);
         
        }

        [Test]
        public void TEST_003_Purchase_Product()
        {
            int productId = 1;
            var purchase = service.Purchase(productId);

            purchase.Expect(HttpStatusCode.Accepted);
            Assert.AreEqual(purchase.Entity.Points, 100);

        }

        [Test]
        public void TEST_008_Get_Points_Increment_For_Same_User()
        {

            var points1 = service.GetPoints();
            int productId = 1;
            var purchase = service.Purchase(productId);
            var points2 = service.GetPoints();

            points1.Expect(HttpStatusCode.Accepted);
            points2.Expect(HttpStatusCode.Accepted);
            Assert.AreEqual((points2.Entity.Value - points1.Entity.Value), 100);

        }

        [Test]
        public void TEST_009_Non_Support_Multiple_Purchase()
        {

            int productId = 1;
            var purchase = service.Purchase(productId);
            productId = 2;
            purchase = service.Purchase(productId);  
            
            purchase.Expect(HttpStatusCode.BadRequest);

        }

        [Test]
        public void TEST_010_Support_Repeatly_Purchase()
        {            
            int productId = 1;
            var purchase = service.Purchase(productId);
            productId = 1;
            purchase = service.Purchase(productId);
           
            purchase.Expect(HttpStatusCode.Accepted);

        }


        [Test]
        public void TEST_004_Login_Missing_Credential_Unsuccessful_Response()
        {
            var response = service.Login(null, null);

            response.Expect(HttpStatusCode.BadRequest);
            Assert.AreEqual(response.Error, "\"Error: Username and password required.\"");
        }

        [Test]
        [TestCase("Tester123", "Plexure123")]
        [TestCase("Tester",null)]
        [TestCase(null, "Plexure123")]
        [TestCase("abc", "abc")]
        public void TEST_005_Login_Dismatch_Unsuccessful_Response(string username, string password)
        {
            var response = service.Login(username,password);
       
            response.Expect(HttpStatusCode.Unauthorized);
            Assert.AreEqual(response.Error, "\"Error: Unauthorized\"");
        }

        [Test]
        public void TEST_006_Points_Unsuccessful_Response()
        {

            var points = service.GetPoints();

            if (points.StatusCode.Equals(HttpStatusCode.Unauthorized))
            {
                Assert.AreEqual(points.Error, "\"Error: Unauthorized\"");
            }
        }

        [Test]
        [TestCase(2)]
        [TestCase(-1)]
        [TestCase(1000)]
        [TestCase(-1000)]
        public void TEST_007_Purchase_Unsuccessful_Response(int productId)
        {           
            var purchase = service.Purchase(productId);

            purchase.Expect(HttpStatusCode.BadRequest);
            Assert.AreEqual(purchase.Error, "\"Error: Invalid product id\"");
        }  


    }
}
