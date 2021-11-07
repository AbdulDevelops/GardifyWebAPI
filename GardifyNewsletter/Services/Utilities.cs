using System;

using System.Web;
using System.Text.RegularExpressions;

namespace GardifyNewsletter.Services
{

    public class Utilities
    {


        #region "Constructor"

        public Utilities()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #endregion

        #region "Public Methods"





        public static string ToSeoUrl(string g)
        {
            string separator = "-";
            string escapeSeparator = @"\";

            // Umlaute ersetzen   
            g = g.Replace("ü", "ue").Replace("Ü", "Ue").Replace("ä", "ae").Replace("Ä", "Ae").Replace("ö", "oe").Replace("Ö", "oe").Replace("ß", "ss");

            // Alle Zeichen bis auf [a-Z] und [0-9] durch Separator ersetzen   
            //g = Regex.Replace(g, "/[^a-z0-9_]/", separator);   

            g = Regex.Replace(g, "[^A-z0-9]", separator);

            // Auf max. 1 aufeinanderfolgenden Separator reduzieren   
            g = Regex.Replace(g, @"[\" + escapeSeparator + separator + "]+", separator);

            // Leerzeichen und Separator am Anfang und Ende entfernen   
            g = g.Trim().Trim(separator.ToCharArray());

            return g;
        }


        public static string Truncate(string input, int characterLimit)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }
            string output = Regex.Replace(input, "<.*?>", string.Empty);


            // Check if the string is longer than the allowed amount otherwise do nothing. 

            if (output.Length > characterLimit && characterLimit > 0)
            {


                // Cut the string down to the maximum number of characters.  


                output = output.Substring(0, characterLimit);


                // Check if the character right after the truncate point was a space  
                // if not, we are in the middle of a word and need to remove the rest of it  
                if (input.Substring(output.Length, 1) != " ")
                {
                    int LastSpace = output.LastIndexOf(" ");


                    // If we found a space then, cut back to that space.  
                    if (LastSpace != -1)
                    {
                        output = output.Substring(0, LastSpace);
                    }
                }
                // Finally, add the "..."  

                output = HttpUtility.HtmlDecode(output);
                output += "...";
            }
            return output;
        }

        #endregion

        public static string ExtractNumbers(string strValue, int chars)
        {
            string strNummer = string.Empty;

            if (strValue.Length == 0) return "";

            foreach (char numChar in strValue.ToCharArray())
            {
                if (Char.IsNumber(numChar)) strNummer += numChar.ToString();
            }

            if (strNummer == string.Empty) return "";
            if (chars > 0 && strNummer.Length > chars)
            { strNummer = strNummer.Substring(0, chars); }
            return strNummer;
        }



    }
}

