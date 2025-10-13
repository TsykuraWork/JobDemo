using NUnit.Framework;

namespace TestProjectCesar
{
    public class Tests
    {
        cesar.cesar—ipherRUS CR = new cesar.cesar—ipherRUS();
        cesar.cesar—ipherENG CE = new cesar.cesar—ipherENG();
        string message1 = "œ–»¬≈“. Ã»–, œŒ ¿/ Ã»–-";
        string messageNULL = null;
        string message1result = "‘’Õ∆…◊. —Õ’, ‘”œ≈/ —Õ’-";
        string message1resultNEGATIVE = " Àƒ˝¿Õ. «ƒÀ,  …®˚/ «ƒÀ-";
        string message1ENG = "HELLO. WORLD, GOODBYE/ WORLD-";
        string message1ENGresult = "MJQQT. bTWQI, LTTIGdJ/ bTWQI-";
        string message1ENGresultNEGATIVE = "CzGGJ. RJMGy, BJJywTz/ RJMGy-";
        string messageMIXED = "œ–»¬≈“. Ã»–- HELLO+ WORLD/";
        string messageMIXEDrusRESULT = "‘’Õ∆…◊. —Õ’- HELLO+ WORLD/";
        string messageMIXEDengRESULT = "œ–»¬≈“. Ã»–- MJQQT+ bTWQI/";


        [Test]
        public void TestRUS1Encrypt()
        {
           
            Assert.AreEqual(message1result, CR.Encrypt(message1, 5));
        }

        [Test]
        public void TestRUS1Decrypt()
        {
            Assert.AreEqual(message1, CR.Decrypt(message1result, 5));
        }

       [Test]
        public void TestRUS2Encrypt()
        {

            Assert.AreEqual(message1resultNEGATIVE, CR.Encrypt(message1, -5));
        }

        [Test]
        public void TestRUS2Decrypt()
        {
            Assert.AreEqual(message1, CR.Decrypt(message1resultNEGATIVE, -5));
        }

        [Test]
        public void TestRUS3Encrypt()
        {

            Assert.AreEqual(message1result, CR.Encrypt(messageNULL, 5));
        }

        [Test]
        public void TestRUS3Decrypt()
        {
            Assert.AreEqual(message1, CR.Decrypt(messageNULL, 5));
        }

        [Test]
        public void TestENG1Encrypt()
        {

            Assert.AreEqual(message1ENGresult, CE.Encrypt(message1ENG, 5));
        }

        [Test]
        public void TestENG1Decrypt()
        {
            Assert.AreEqual(message1ENG, CE.Decrypt(message1ENGresult, 5));
        }

        [Test]
        public void TestENG2Encrypt()
        {

            Assert.AreEqual(message1ENGresultNEGATIVE, CE.Encrypt(message1ENG, -5));
        }

        [Test]
        public void TestENG2Decrypt()
        {
            Assert.AreEqual(message1ENG, CE.Decrypt(message1ENGresultNEGATIVE, -5));
        }

        [Test]
        public void TestENG3Encrypt()
        {

            Assert.AreEqual(message1ENGresult, CE.Encrypt(messageNULL, 5));
        }

        [Test]
        public void TestENG3Decrypt()
        {
            Assert.AreEqual(message1ENGresult, CE.Decrypt(messageNULL, 5));
        }

        [Test]
        public void TestRUSmixedEncrypt()
        {

            Assert.AreEqual(messageMIXEDrusRESULT, CR.Encrypt(messageMIXED, 5));
        }

        [Test]
        public void TestRUSmixedDecrypt()
        {
            Assert.AreEqual(messageMIXED, CR.Decrypt(messageMIXEDrusRESULT, 5));
        }

        [Test]
        public void TestENGmixedEncrypt()
        {

            Assert.AreEqual(messageMIXEDengRESULT, CE.Encrypt(messageMIXED, 5));
        }

        [Test]
        public void TestENGmixedDecrypt()
        {
            Assert.AreEqual(messageMIXED, CE.Decrypt(messageMIXEDengRESULT, 5));
        }

    }
}