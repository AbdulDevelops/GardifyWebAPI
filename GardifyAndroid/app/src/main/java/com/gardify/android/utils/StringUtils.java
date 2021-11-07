package com.gardify.android.utils;

import android.text.Html;
import android.text.Spanned;

/**
 * <p>
 * StringUtil collects together some string utility classes.
 * </p>
 */
public class StringUtils
{

    /**
     * @param text string with [k] tags
     * @return replaces [k][/k] tags with <i></i> tags
     */
    public static Spanned formatHtmlKTags(String text) {
        String formattedText = text.replace("[k]", "<i>")
                .replace("[/k]", "</i>");

        return Html.fromHtml(formattedText,Html.FROM_HTML_MODE_LEGACY);
    }

    /**
     * Limit the string to a certain number of characters, adding "..." if it was truncated
     *
     * @param value
     *        The string to limit.
     * @param length
     *        the length to limit to (as an int).
     * @return The limited string.
     */
    public static String limit(String value, int length)
    {
        StringBuilder buf = new StringBuilder(value);
        if (buf.length() > length)
        {
            buf.setLength(length);
            buf.append("...");
        }

        return buf.toString();
    }

    /**
     * @param monthNumber is given as input
     * @return returns abbreviated name of the month
     */
    public static String getShortMonthNameByNumber(int monthNumber) {
        String[] Months = {"Jan.", "Feb.", "März", "April", "Mai", "Juni", "Juli", "Aug.", "Sept.", "Okt.", "Nov.", "Dez."};
        return Months[monthNumber];
    }

}
