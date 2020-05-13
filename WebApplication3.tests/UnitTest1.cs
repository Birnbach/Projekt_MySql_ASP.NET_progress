using System;
using Xunit;
using WebApplication3.Baza;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data;
using WebApplication3.Models;
using System.Diagnostics;

namespace WebApplication3.tests
{
    public class UnitTest1
    {

        public static string ConnectionString = "SERVER=localhost" + ";" + "DATABASE=new_schema" + ";" + "UID=root" + ";" + "PASSWORD=1q2w3e4rPoi";

        private readonly Baza.ConnectionDB _ConnectionDB;

        public UnitTest1()
        {
            _ConnectionDB = new Baza.ConnectionDB(ConnectionString);
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }


        [Fact]
        public void checkMD5sum()
        {
            string test = "dadadada";
            string testMD5 = "6D2CF3FDAB44BDFC1990230F21E4C25D";
            Assert.True(Baza.ConnectionDB.calculateMD5sum(test) == testMD5, "MD5 checksum should be calculated correctly");
        }

        [Fact]
        public void testLogin()
        // test login functionality
        // after providing correct username and password, correct ID should be returned
        {
            signupForm testSignup = new signupForm();
            testSignup.username = "testuser";
            testSignup.password = "123456";
            testSignup.email = "test@email.dx";
            testSignup.phone = "123456789";
            testSignup.name = "Test User";

            userModel testModel = new userModel();
            testModel.username = "testuser";
            testModel.password = "123456";

            if (_ConnectionDB.checkIfUserExists("testuser") != -1) _ConnectionDB.removeUser((_ConnectionDB.checkIfUserExists("testuser")));

            int tempID = _ConnectionDB.createUser(testSignup);
            var result = _ConnectionDB.checkUserID(testModel);
            _ConnectionDB.removeUser(tempID);

            Assert.True(result == tempID, "There should be a test user in database!");
        }

        [Fact]
        public void testIncorrectPassword()
        // incorrect password in checkUserID should return -2
        {
            signupForm testSignup = new signupForm();
            testSignup.username = "testuser";
            testSignup.password = "123456";
            testSignup.email = "test@email.dx";
            testSignup.phone = "123456789";
            testSignup.name = "Test User";

            userModel testModel = new userModel();
            testModel.username = "testuser";
            testModel.password = "1234567";     // wrong password

            if (_ConnectionDB.checkIfUserExists("testuser") != -1) _ConnectionDB.removeUser((_ConnectionDB.checkIfUserExists("testuser")));

            int tempID = _ConnectionDB.createUser(testSignup);
            var result = _ConnectionDB.checkUserID(testModel);
            _ConnectionDB.removeUser(tempID);

            Assert.True(result == -2, "Incorrect password should return -2");
        }

        [Fact]
        public void testSignupUsernameConflict()
        // Attempt to create user with username that already exists should return -1
        {
            signupForm testSignup = new signupForm();
            testSignup.username = "testuser";
            testSignup.password = "123456";
            testSignup.email = "test@email.dx";
            testSignup.phone = "123456789";
            testSignup.name = "Test User";

            int ID = _ConnectionDB.createUser(testSignup);
            var result = _ConnectionDB.createUser(testSignup);
            _ConnectionDB.removeUser(ID);

            Assert.True(result == -1, "Username conflict should return -1");
        }
    }
}
