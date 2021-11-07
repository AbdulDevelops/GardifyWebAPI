package com.gardify.android.utils;

import android.content.Context;
import android.text.format.DateUtils;

import com.gardify.android.R;

import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;
import java.util.TimeZone;

public class TimeUtils {

    /**
     * Converts date string from one pattern to another returns string value.
     *
     * @param dateString date string to be reformatted
     * @param oldPattern old pattern of date
     * @param newPattern new pattern of date
     * @return See implementation for return details
     */
    public static String dateToString(String dateString, String oldPattern, String newPattern) {
        DateFormat originalFormat = new SimpleDateFormat(oldPattern, Locale.getDefault());
        DateFormat targetFormat = new SimpleDateFormat(newPattern, Locale.GERMANY);
        Date date = null;
        try {
            date = originalFormat.parse(dateString);
            return targetFormat.format(date);
        } catch (ParseException e) {
            e.printStackTrace();
        }
        return "0";
    }

    /**
     * Returns today's date in pattern used by Gardify
     */
    public static String todayDate(String pattern) {
        DateFormat df = new SimpleDateFormat(pattern);
        Date dateObj = new Date();
        return df.format(dateObj);
    }

    /**
     * Returns a SimpleDateFormat object that's adjusted to the timezone of the device.
     *
     * @param pattern Date output pattern for the format
     * @return Adjusted pattern format object
     */
    public static SimpleDateFormat getOutputFormatter(String pattern) {
        SimpleDateFormat formatter;
        formatter = new SimpleDateFormat(pattern);

        Calendar cal = Calendar.getInstance();
        TimeZone tz = cal.getTimeZone();
        //TimeZone old = TimeZone.getTimeZone("Europe/Amsterdam");
        formatter.setTimeZone(tz);

        return formatter;
    }

    /**
     *
     * @param context  Context
     * @param dateTime date string
     * @return Returns name (today, tomorrow) for a given date.
     */

    public static String checkDateToday(Context context, String dateTime, String oldPattern, String newPattern) {
        Date date = getDate(dateTime, oldPattern);

        if (isToday(date)) {
            return context.getResources().getString(R.string.all_today);
        } else if (isTomorrow(date)) {
            return context.getResources().getString(R.string.all_tomorrow);
        } else {
            return dateToString(dateTime, oldPattern, newPattern);
        }
    }

    private static boolean isToday(Date date) {
        if (date != null)
            return DateUtils.isToday(date.getTime());
        else
            return false;
    }

    private static boolean isTomorrow(Date date) {
        if (date != null)
            return DateUtils.isToday(date.getTime() - DateUtils.DAY_IN_MILLIS);
        else
            return false;
    }

    /**
     * @param dateTime String dateTime
     * @param pattern  Pattern of String dateTime
     * @return returns Date in specified pattern
     */
    private static Date getDate(String dateTime, String pattern) {
        SimpleDateFormat format = new SimpleDateFormat(pattern, Locale.getDefault());
        Date date = null;
        try {
            date = format.parse(dateTime);
        } catch (ParseException e) {
            e.printStackTrace();
        }
        return date;
    }
}
