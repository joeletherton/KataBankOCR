﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace KataBankOCR
{
    public class OCRProcessor
    {
        public string[] Lines;
        protected BackgroundWorker Worker { get; set; }

        public OCRProcessor()
        {
        }

        public OCRProcessor(BackgroundWorker worker)
        {
            Worker = worker;
        }

        /// <summary>
        /// Performs the actual call. Simply
        /// cycles through the lines that 
        /// have been presented and outputs
        /// the account number status
        /// </summary>
        /// <param name="lines"></param>
        public void ProcessOCRFile(string[] lines)
        {
            Lines = lines;

            string accountNumber;

            for (int i = 0; i < lines.Length - 3; i += 4)
            {
                // Get the account number
                accountNumber = GetAccountNumber(i);

                // Report the account number to the interface
                if (Worker != null)
                {
                    var output = String.Format("Account Number: {0}{1}",
                        accountNumber, GetAccountNumberStatus(accountNumber));
                    Worker.ReportProgress(i / Lines.Length * 100, output);
                }
            }

        }

        /// <summary>
        /// This method is responsible for validating
        /// account numbers through the checksum
        /// mathematical formula
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public static bool IsValidCheckSum(string accountNumber)
        {
            if (accountNumber.Length != 9)
                return false;

            try
            {
                // Checksum determination per requirement
                // position names:  d9 d8 d7 d6 d5 d4 d3 d2 d1
                // checksum calculation:
                // (d1+2*d2+3*d3 +..+9*d9) mod 11 = 0

                var sum = 0;
                int coefficient = 1;
                int digit = 0;

                for (int i = 9; i > 0; i--)
                {
                    if (!int.TryParse(
                        accountNumber.Substring(i - 1, 1),
                        out digit))
                    {
                        return false;
                    }

                    sum += coefficient * digit;
                    coefficient++;
                }

                return sum % 11 == 0;
            }
                catch
            {
                return false;
            }
        }

        /// <summary>
        /// This method tests if a sequence of lines intended
        /// for a single account number are all of the proper
        /// length. There should be 4 lines and each should be 
        /// 27 characters. The offset indicates which row to 
        /// start on
        /// </summary>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public bool AreLinesValid(int offSet)
        {
            // There are 4 lines to check and
            // each should be at least 27 characters
            return !Lines.Skip(offSet).Take(4).Any(x => x.Length < 27);
        }

        /// <summary>
        /// Responsible for getting the account number string
        /// This method receives an index to begin the character
        /// search and returns a 9 digit string
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public string GetAccountNumber(int rowIndex)
        {
            if (Lines.Length <= rowIndex)
            {
                return "?????????";
            }

            // Fix the lines in case one is short
            if (!AreLinesValid(rowIndex))
            {
                Lines[rowIndex] = PadShortLine(Lines[rowIndex]);
                Lines[rowIndex + 1] = PadShortLine(Lines[rowIndex + 1]);
                Lines[rowIndex + 2] = PadShortLine(Lines[rowIndex + 2]);
                Lines[rowIndex + 3] = PadShortLine(Lines[rowIndex + 3]);
            }

            var result = new StringBuilder("");

            for (int i = 0; i < 9; i++)
            {
                result.Append(GetCharacter(rowIndex, i));
            }

            return result.ToString();
        }

        /// <summary>
        /// This method converts a character mask into a
        /// string character for insertion into an account number
        /// The rowStartIndex is self explanatory. The 
        /// characterStartIndex indicates the account number 
        /// character so it should not exceed 9
        /// </summary>
        /// <param name="rowStartIndex"></param>
        /// <param name="characterStartIndex"></param>
        /// <returns></returns>
        public string GetCharacter(int rowStartIndex, int characterStartIndex)
        {
            string response;

            // Just a little safety checking
            if (Lines == null || rowStartIndex >= Lines.Length || rowStartIndex % 4 != 0)
            {
               response = "?";
            }

            var character = GetCharacterMask(rowStartIndex, characterStartIndex);

            switch (character)
            {
                case (int)Characters.One:
                    response = "1";
                    break;
                case (int)Characters.Two:
                    response = "2";
                    break;
                case (int)Characters.Three:
                    response = "3";
                    break;
                case (int)Characters.Four:
                    response = "4";
                    break;
                case (int)Characters.Five:
                    response = "5";
                    break;
                case (int)Characters.Six:
                    response = "6";
                    break;
                case (int)Characters.Seven:
                    response = "7";
                    break;
                case (int)Characters.Eight:
                    response = "8";
                    break;
                case (int)Characters.Nine:
                    response = "9";
                    break;
                case (int)Characters.Zero:
                    response = "0";
                    break;
                default:
                    response = "?";
                    break;
            }
            
            return response;
        }

        /// <summary>
        /// This method is responsible for performing the addition
        /// of the slice masks to retrieve the numerical character mask
        /// </summary>
        /// <param name="rowStartIndex"></param>
        /// <param name="characterStartIndex"></param>
        /// <returns></returns>
        public int GetCharacterMask(int rowStartIndex, int characterStartIndex)
        {
            return
                GetSliceMask(rowStartIndex, characterStartIndex, 0, 0) +
                GetSliceMask(rowStartIndex, characterStartIndex, 1, 0) +
                GetSliceMask(rowStartIndex, characterStartIndex, 2, 0) +
                GetSliceMask(rowStartIndex, characterStartIndex, 0, 1) +
                GetSliceMask(rowStartIndex, characterStartIndex, 1, 1) +
                GetSliceMask(rowStartIndex, characterStartIndex, 2, 1) +
                GetSliceMask(rowStartIndex, characterStartIndex, 0, 2) +
                GetSliceMask(rowStartIndex, characterStartIndex, 1, 2) +
                GetSliceMask(rowStartIndex, characterStartIndex, 2, 2);
        }

        /// <summary>
        /// This method retrieves the numerical value of the requested
        /// slice. The slice is determined by the x,y coordinates in a
        /// 3x3 grid as defined by the exponential relationship of the mask
        /// </summary>
        /// <param name="rowStartIndex"></param>
        /// <param name="characterStartIndex"></param>
        /// <param name="sliceX"></param>
        /// <param name="sliceY"></param>
        /// <returns></returns>
        public int GetSliceMask(int rowStartIndex, int characterStartIndex, int sliceX, int sliceY)
        {
            // The character start index is the offset to begin collecting the character
            // in the row itself. Since all characters have a spacing of 3, the index of 
            // the character will be a factor of 3 maintaining a 0 based index. 
            var slice = Lines[rowStartIndex + sliceY][3 * characterStartIndex + sliceX];

            // Empty spaces are always valid, but their 
            // mask is always 0
            if (slice == ' ')
                return 0;

            var offSet = sliceX + 3 * sliceY;
            bool charIsValid = false;

            switch (offSet)
            {
                case 1:
                case 4:
                case 7:
                    charIsValid =  (slice == '_');
                    break;
                case 3:
                case 5:
                case 6:
                case 8:
                    charIsValid = (slice == '|');
                    break;
                default:
                    charIsValid = false;  // Should never hit, but if I had a nickel for every "should"...
                    break;
            }

            // if this character is not valid, make sure that the 
            // the result can not be accidentally valid
            return (charIsValid) ? Convert.ToInt32(Math.Pow(2, offSet)) : 1000;
        }

        /// <summary>
        /// Presentation method only. Exposes ambiguous or erroneous
        /// account numbers for display
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public static string GetAccountNumberStatus(string accountNumber)
        {
            // This could be done in a ternary fashion, but 
            // I think the explicit ifs are more readable.
            if (accountNumber.Contains("?")) 
                return " " + AccountNumberStatus.AMB.ToString();

            if (!IsValidCheckSum(accountNumber))
                return " " + AccountNumberStatus.ERR.ToString();

            return string.Empty;
        }

        /// <summary>
        /// Presentation method only. Determines the current 
        /// percentage to report to the UI
        /// </summary>
        /// <param name="current"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static int GetPercentageComplete(int current, int total)
        {
            return 100 * current / total;
        }

        /// <summary>
        /// Validation helper. This line guarantees that all lines
        /// will have 27 characters and that the characters will
        /// fail slice comparisons.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string PadShortLine(string source)
        {
            if (source.Length >= 27)
                return source;

            return source.Replace(Environment.NewLine, "").PadRight(27, 'x');
        }

        /// <summary>
        /// This enumeration defines the numeric
        /// masks for valid characters. Any return
        /// that does not match one of these masks
        /// should represent a ?
        /// </summary>
        public enum Characters
        {
            // Assignments of masks
            // to definable characters
            One = 288,
            Two = 242,
            Three = 434,
            Four = 312,
            Five = 410,
            Six = 474,
            Seven = 290,
            Eight = 506,
            Nine = 442,
            Zero = 490
        }

        public enum AccountNumberStatus
        {
            AMB,
            ERR
        }
    }
}
