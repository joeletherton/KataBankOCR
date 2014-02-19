using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using KataBankOCR;

namespace KataBankOCRTest
{
    [TestClass]
    public class KataBankOCRTest
    {
        public string[] GoodLines;
        public string[] BadLines;
        public string[] MixedLines;
        public string[] BadCharacterLines;

        public OCRProcessor Processor;

        public KataBankOCRTest()
        {
            Processor = new OCRProcessor();

            GoodLines = new string[]
            {
                " _  _  _  _  _  _  _  _  _ " + Environment.NewLine,
                "| || || || || || || || || |" + Environment.NewLine,
                "|_||_||_||_||_||_||_||_||_|" + Environment.NewLine,
                "                           " + Environment.NewLine,
                "                           " + Environment.NewLine,
                "  |  |  |  |  |  |  |  |  |" + Environment.NewLine,
                "  |  |  |  |  |  |  |  |  |" + Environment.NewLine,
                "                           " + Environment.NewLine,
                " _  _  _  _  _  _  _  _  _ " + Environment.NewLine,
                " _| _| _| _| _| _| _| _| _|" + Environment.NewLine,
                "|_ |_ |_ |_ |_ |_ |_ |_ |_ " + Environment.NewLine,
                "                           " + Environment.NewLine,
                " _  _  _  _  _  _  _  _  _ " + Environment.NewLine,
                " _| _| _| _| _| _| _| _| _|" + Environment.NewLine,
                " _| _| _| _| _| _| _| _| _|" + Environment.NewLine,
                "                           " + Environment.NewLine,
                "                           " + Environment.NewLine,
                "|_||_||_||_||_||_||_||_||_|" + Environment.NewLine,
                "  |  |  |  |  |  |  |  |  |" + Environment.NewLine,
                "                           " + Environment.NewLine,
                " _  _  _  _  _  _  _  _  _ " + Environment.NewLine,
                "|_ |_ |_ |_ |_ |_ |_ |_ |_ " + Environment.NewLine,
                " _| _| _| _| _| _| _| _| _|" + Environment.NewLine,
                "                           " + Environment.NewLine,
                " _  _  _  _  _  _  _  _  _ " + Environment.NewLine,
                "|_ |_ |_ |_ |_ |_ |_ |_ |_ " + Environment.NewLine,
                "|_||_||_||_||_||_||_||_||_|" + Environment.NewLine,
                "                           " + Environment.NewLine,
                " _  _  _  _  _  _  _  _  _ " + Environment.NewLine,
                "  |  |  |  |  |  |  |  |  |" + Environment.NewLine,
                "  |  |  |  |  |  |  |  |  |" + Environment.NewLine,
                "                           " + Environment.NewLine,
                " _  _  _  _  _  _  _  _  _ " + Environment.NewLine,
                "|_||_||_||_||_||_||_||_||_|" + Environment.NewLine,
                "|_||_||_||_||_||_||_||_||_|" + Environment.NewLine,
                "                           " + Environment.NewLine,
                " _  _  _  _  _  _  _  _  _ " + Environment.NewLine,
                "|_||_||_||_||_||_||_||_||_|" + Environment.NewLine,
                " _| _| _| _| _| _| _| _| _|" + Environment.NewLine,
                "                           " + Environment.NewLine
            };

            BadLines = new string[]
            {
                "abcdefghijklmnopqrstjkjgjuvwxyz" + Environment.NewLine,
                "abcdefghijklmnostuvwxyz" + Environment.NewLine,
                "abcdefghijklmnopqrswxyz" + Environment.NewLine,
                "abcdefghijklmnopfggfqrstuvwxyz" + Environment.NewLine
            };

            MixedLines = new string[]
            {
                "abcdefghijklmnopqrstuvwxyz1" + Environment.NewLine,
                "sdfgb" + Environment.NewLine,
                "abcdefghijklmnopqrstuvwdfgdfxyz" + Environment.NewLine
            };

            BadCharacterLines = new string[]
            {
                "    _  _  _  _  _  _  _  _ " + Environment.NewLine,
                "|_||_||_||_| _||_||_|  ||_|" + Environment.NewLine,
                "|_||_||_||_||_||_||_||_ |_|" + Environment.NewLine,
                "                           " + Environment.NewLine
            };

            Processor.Lines = GoodLines;
        }

        [TestMethod]
        public void Test_ValidateLinesGood()
        {
            Processor.Lines = GoodLines;
            Assert.IsTrue(Processor.AreLinesValid(0));
        }

        [TestMethod]
        public void Test_ValidateLinesBad()
        {
            Processor.Lines = BadLines;
            Assert.IsTrue(!Processor.AreLinesValid(0));
        }

        [TestMethod]
        public void Test_ValidateLinesMixed()
        {
            Processor.Lines = MixedLines;
            Assert.IsTrue(!Processor.AreLinesValid(0));

        }

        [TestMethod]
        public void Test_IsValidChecksum()
        {
            string validAccountNumber = "457508000";
            Assert.IsTrue(OCRProcessor.IsValidCheckSum(validAccountNumber));
        }

        [TestMethod]
        public void Test_IsInvalidChecksum()
        {
            string invalidAccountNumber = "123456788";
            Assert.IsTrue(!OCRProcessor.IsValidCheckSum(invalidAccountNumber));
        }

        [TestMethod]
        public void Test_InvalidCharactersInChecksum()
        {
            string invalidCharactersInAccountNumber = "1234?6789";
            Assert.IsTrue(!OCRProcessor.IsValidCheckSum(invalidCharactersInAccountNumber));
        }

        [TestMethod]
        public void Test_GetSliceMask_0_2()
        {
            Processor.Lines = GoodLines;
            var expected = 0;
            Assert.AreEqual(expected, Processor.GetSliceMask(0, 0, 2, 0));
        }

        [TestMethod]
        public void Test_GetSliceMask_1_0()
        {
            Processor.Lines = GoodLines;
            var expected = 8;
            Assert.AreEqual(expected, Processor.GetSliceMask(0, 0, 0, 1));
        }

        [TestMethod]
        public void Test_GetSliceMask_2_1()
        {
            Processor.Lines = GoodLines;
            var expected = 128;
            Assert.AreEqual(expected, Processor.GetSliceMask(0, 0, 1, 2));
        }

        [TestMethod]
        public void Test_GetCharacterMask_0()
        {
            Processor.Lines = GoodLines;
            var expected = 490;
            Assert.AreEqual(expected, Processor.GetCharacterMask(0, 7));
        }

        [TestMethod]
        public void Test_GetCharacterMask_1()
        {
            Processor.Lines = GoodLines;
            var expected = 288;
            Assert.AreEqual(expected, Processor.GetCharacterMask(4, 2));
        }

        [TestMethod]
        public void Test_GetCharacterMask_2()
        {
            Processor.Lines = GoodLines;
            var expected = 242;
            Assert.AreEqual(expected, Processor.GetCharacterMask(8, 5));
        }

        [TestMethod]
        public void Test_GetCharacterMask_3()
        {
            Processor.Lines = GoodLines;
            var expected = 434;
            Assert.AreEqual(expected, Processor.GetCharacterMask(12, 1));
        }

        [TestMethod]
        public void Test_GetCharacterMask_4()
        {
            Processor.Lines = GoodLines;
            var expected = 312;
            Assert.AreEqual(expected, Processor.GetCharacterMask(16, 6));
        }

        [TestMethod]
        public void Test_GetCharacterMask_5()
        {
            Processor.Lines = GoodLines;
            var expected = 410;
            Assert.AreEqual(expected, Processor.GetCharacterMask(20, 8));
        }

        [TestMethod]
        public void Test_GetCharacterMask_6()
        {
            Processor.Lines = GoodLines;
            var expected = 474;
            Assert.AreEqual(expected, Processor.GetCharacterMask(24, 3));
        }

        [TestMethod]
        public void Test_GetCharacterMask_7()
        {
            Processor.Lines = GoodLines;
            var expected = 290;
            Assert.AreEqual(expected, Processor.GetCharacterMask(28, 4));
        }

        [TestMethod]
        public void Test_GetCharacterMask_8()
        {
            Processor.Lines = GoodLines;
            var expected = 506;
            Assert.AreEqual(expected, Processor.GetCharacterMask(32, 3));
        }

        [TestMethod]
        public void Test_GetCharacterMask_9()
        {
            Processor.Lines = GoodLines;
            var expected = 442;
            Assert.AreEqual(expected, Processor.GetCharacterMask(36, 7));
        }

        [TestMethod]
        public void Test_GetCharacterMaskInvalid()
        {
            var expected = 9000;
            Processor.Lines = BadLines;
            Assert.AreEqual(expected, Processor.GetCharacterMask(0, 0));
        }

        [TestMethod]
        public void Test_GetCharacter0()
        {
            Processor.Lines = GoodLines;
            var expected = "0";
            Assert.AreEqual(expected, Processor.GetCharacter(0, 2));
        }

        [TestMethod]
        public void Test_GetCharacter1()
        {
            Processor.Lines = GoodLines;
            var expected = "1";
            Assert.AreEqual(expected, Processor.GetCharacter(4, 7));
        }

        [TestMethod]
        public void Test_GetCharacter2()
        {
            Processor.Lines = GoodLines;
            var expected = "2";
            Assert.AreEqual(expected, Processor.GetCharacter(8, 5));
        }

        [TestMethod]
        public void Test_GetCharacter3()
        {
            Processor.Lines = GoodLines;
            var expected = "3";
            Assert.AreEqual(expected, Processor.GetCharacter(12, 1));
        }

        [TestMethod]
        public void Test_GetCharacter4()
        {
            Processor.Lines = GoodLines;
            var expected = "4";
            Assert.AreEqual(expected, Processor.GetCharacter(16, 8));
        }

        [TestMethod]
        public void Test_GetCharacter5()
        {
            Processor.Lines = GoodLines;
            var expected = "5";
            Assert.AreEqual(expected, Processor.GetCharacter(20, 3));
        }

        [TestMethod]
        public void Test_GetCharacter6()
        {
            Processor.Lines = GoodLines;
            var expected = "6";
            Assert.AreEqual(expected, Processor.GetCharacter(24, 4));
        }

        [TestMethod]
        public void Test_GetCharacter7()
        {
            Processor.Lines = GoodLines;
            var expected = "7";
            Assert.AreEqual(expected, Processor.GetCharacter(28, 8));
        }

        [TestMethod]
        public void Test_GetCharacter8()
        {
            Processor.Lines = GoodLines;
            var expected = "8";
            Assert.AreEqual(expected, Processor.GetCharacter(32, 0));
        }

        [TestMethod]
        public void Test_GetCharacter9()
        {
            Processor.Lines = GoodLines;
            var expected = "9";
            Assert.AreEqual(expected, Processor.GetCharacter(36, 6));
        }

        [TestMethod]
        public void Test_GetCharacter_InvalidRow()
        {
            Processor.Lines = GoodLines;
            var expected = "?";
            Assert.AreEqual(expected, Processor.GetCharacter(2, 2));
        }

        [TestMethod]
        public void Test_GetAccountNumber_Good()
        {
            Processor.Lines = GoodLines;
            var expected = "000000000";
            Assert.AreEqual(expected, Processor.GetAccountNumber(0));
        }

        [TestMethod]
        public void Test_GetAccountNumber_Bad()
        {
            Processor.Lines = BadLines;
            var expected = "?????????";
            Assert.AreEqual(expected, Processor.GetAccountNumber(0));
        }

        [TestMethod]
        public void Test_GetAccountNumber_StrangeCharacters()
        {
            Processor.Lines = BadCharacterLines;
            var expected = "?888?88?8";
            Assert.AreEqual(expected, Processor.GetAccountNumber(0));
        }

        [TestMethod]
        public void Test_GetAccountNumberStatus_Good()
        {
            var expected = "";
            var accountNumber = "123456789";
            Assert.AreEqual(expected, OCRProcessor.GetAccountNumberStatus(accountNumber));
        }

        [TestMethod]
        public void Test_GetAccountNumberStatus_AMB()
        {
            var expected = " AMB";
            var accountNumber = "12345?789";
            Assert.AreEqual(expected, OCRProcessor.GetAccountNumberStatus(accountNumber));
        }

        [TestMethod]
        public void Test_GetAccountNumberStatus_ERR()
        {
            var expected = " ERR";
            var accountNumber = "123456788";
            Assert.AreEqual(expected, OCRProcessor.GetAccountNumberStatus(accountNumber));
        }

        [TestMethod]
        public void Test_GetPercentageComplete()
        {
            var expected = 26;
            var total = 104;
            var current = 28;

            Assert.AreEqual(expected, OCRProcessor.GetPercentageComplete(current, total));
        }

        [TestMethod]
        public void Test_PadShortLine_NoPaddingNeeded()
        {
            var source = "abcdefghijklmnopqrstuvwxyz1" + Environment.NewLine;
            var expected = "abcdefghijklmnopqrstuvwxyz1" + Environment.NewLine;

            Assert.AreEqual(expected, OCRProcessor.PadShortLine(source));
        }

        [TestMethod]
        public void Test_PadShortLine_PaddingNeeded()
        {
            var source = "abcdefghijklmnostuvwxyz" + Environment.NewLine;
            var expected = "abcdefghijklmnostuvwxyzxxxx";

            Assert.AreEqual(expected, OCRProcessor.PadShortLine(source));
        }
    }
}